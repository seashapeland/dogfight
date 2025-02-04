using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public Transform cam;

    public float sprintMultiplier = 1.5f;      // 加速倍数
    public float slowWalkMultiplier = 0.5f;    // 慢速行走倍数
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Animator animator;
    private string currentState;

    const string IDLE_BATTLE = "Idle_Battle";
    const string RUNFORWARDBATTLE = "RunForwardBattle";
    const string WALKFORWARDBATTLE = "WalkForwardBattle";
    const string ATTACK01 = "Attack01";       // 攻击动画1
    const string ATTACK02 = "Attack02";       // 攻击动画2
    const string DEFEND = "Defend";           // 防御动画
    const string HIT = "GetHit";              // 受击动画名称
    const string DIE = "Die";                 // 死亡动画名称

    private bool isAttacking = false;
    private bool isDefending = false;
    private bool isDead = false;              // 是否死亡
    private bool isGettingHit = false;        // 是否正在播放受击动画

    private float gravity = -9.81f;
    private Vector3 velocity;

    // 攻击相关
    public Transform attackPoint;          // 攻击点位置
    public float attackRange = 1.5f;       // 攻击范围
    public int attackDamage = 10;          // 攻击伤害
    public LayerMask enemyLayers;          // 检测敌人层级

    // 受击相关
    public int maxHealth = 10000;            // 最大生命值
    private int currentHealth;             // 当前生命值

    public GameManager gameManager;
    public PlayerHealthBar healthBar;
    public PlayerStatusEffects playerStatusEffects;


    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;         // 初始化生命值
        healthBar.InitializeHealthBar(currentHealth);
    }

    void ChangeAnimationState(string newState)
    {
        // 阻止同一个动画状态再次打断自己
        if (currentState == newState) return;

        // 播放新的动画状态
        animator.Play(newState);
        currentState = newState;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return; // 如果玩家已经死亡，不再处理任何逻辑

        // 应用重力
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 确保角色保持在地面
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 如果正在攻击或受击，优先播放动画，不做其他动作
        if (isAttacking || isGettingHit) return;

        // 防御动作检测，按住空格键持续防御
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isDefending)
            {
                isDefending = true;
                ChangeAnimationState(DEFEND);
            }
            return; // 防御状态下不进行其他动作
        }
        else
        {
            // 松开空格键，结束防御
            if (isDefending)
            {
                isDefending = false;
                ChangeAnimationState(IDLE_BATTLE); // 回到待机状态
            }
        }

        // 移动逻辑
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            bool isWalkingSlowly = Input.GetKey(KeyCode.LeftAlt);

            if (isSprinting)
            {
                controller.Move(moveDir.normalized * speed * sprintMultiplier * Time.deltaTime);
                ChangeAnimationState(RUNFORWARDBATTLE);
            }
            else if (isWalkingSlowly)
            {
                controller.Move(moveDir.normalized * speed * slowWalkMultiplier * Time.deltaTime); // 使用慢速行走倍数
                ChangeAnimationState(WALKFORWARDBATTLE);
            }
            else
            {
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
                ChangeAnimationState(RUNFORWARDBATTLE);
            }
        }
        else
        {
            ChangeAnimationState(IDLE_BATTLE);
        }

        // 攻击动作
        if (Input.GetMouseButtonDown(0)) // 鼠标左键攻击
        {
            StartCoroutine(PerformAttack(ATTACK01));
        }
        else if (Input.GetMouseButtonDown(1)) // 鼠标右键攻击
        {
            StartCoroutine(PerformAttack(ATTACK02));
        }
    }

    private IEnumerator PerformAttack(string attackAnimation)
    {
        isAttacking = true;
        ChangeAnimationState(attackAnimation);

        // 检测攻击范围内的敌人
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("击中敌人：" + enemy.name);
            enemy.GetComponent<MonsterAI>()?.TakeDamage(attackDamage);
        }

        // 等待攻击动画的完成时间
        yield return new WaitForSeconds(0.75f);

        isAttacking = false;
    }

    public void Heal(int amount)
    {
        healthBar.Heal(amount);
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // 确保生命值不会超过最大值
        Debug.Log($"玩家生命值恢复：当前生命值为 {currentHealth}");
    }
    public void TakeDamage(int damage)
    {
        // 如果已经死亡、正在受击或处于防御状态，不处理伤害
        if (isDead || isGettingHit || isDefending)
        {
            if (isDefending)
            {
                Debug.Log("玩家正在防御，未受到伤害！");
            }
            return;
        }

        healthBar.TakeDamage(damage);
        currentHealth -= damage;
        Debug.Log($"玩家受到伤害：{damage}，剩余血量：{currentHealth}");

        if (currentHealth > 0)
        {
            StartCoroutine(PlayHitAnimation());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator PlayHitAnimation()
    {
        isGettingHit = true;
        ChangeAnimationState(HIT); // 播放受击动画

        // 假设受击动画时长为 0.5 秒
        yield return new WaitForSeconds(0.5f);

        isGettingHit = false;
    }

    private void Die()
    {
        if (isDead) return; // 防止重复调用
        isDead = true;

        Debug.Log("玩家死亡！");
        ChangeAnimationState(DIE); // 播放死亡动画

        // 启动协程等待动画播放完毕后再显示ScoreUI
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        // 获取动画播放时间
        float deathAnimationDuration = GetAnimationDuration(DIE);

        // 等待动画播放完成
        yield return new WaitForSeconds(deathAnimationDuration);

        // 调用 GameManager 的结束倒计时逻辑
        gameManager.EndCountdown();
    }

    // 获取动画长度的辅助函数
    private float GetAnimationDuration(string animationName)
    {
        if (animator == null || string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("Animator 或动画名称无效，返回默认时长");
            return 1f; // 返回默认时长，避免出错
        }

        // 获取当前动画剪辑
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length; // 返回动画时长
            }
        }

        Debug.LogWarning($"未找到动画 {animationName}，返回默认时长");
        return 1f; // 如果未找到，返回默认时长
    }

    public void AddAttackBoost(int boostAmount, float duration, int maxBoost)
    {
        int totalBoost = Mathf.Clamp(attackDamage + boostAmount, 0, maxBoost); // 防止超过上限
        attackDamage = totalBoost;

        // 更新 PlayerStatusEffects 的 UI
        int currentStages = Mathf.CeilToInt((attackDamage - 25) / 25) + 1; // 根据提升计算UI阶段
        playerStatusEffects.UpdateGreenBar(currentStages);
        Debug.Log($"当前攻击力提升：{attackDamage}");
        StartCoroutine(RemoveAttackBoostAfterTime(boostAmount, duration));
    }

    private IEnumerator RemoveAttackBoostAfterTime(int boostAmount, float duration)
    {
        yield return new WaitForSeconds(duration);

        attackDamage -= boostAmount;
        attackDamage = Mathf.Max(0, attackDamage); // 防止小于0

        int currentStages = Mathf.CeilToInt((attackDamage - 25) / 25) + 1; // 根据提升计算UI阶段
        playerStatusEffects.UpdateGreenBar(currentStages);
        Debug.Log($"攻击力增强效果结束，当前攻击力提升：{attackDamage}");
    }

    public void AddSpeedBoost(float boostAmount, float duration, float maxBoost)
    {
        float totalBoost = Mathf.Clamp(speed + boostAmount, 0, maxBoost); // 防止超过上限
        speed = totalBoost;

        int currentStages = Mathf.CeilToInt((speed - 6) / 2) + 1; // 计算当前增益阶段
        playerStatusEffects.UpdateBlueBar(currentStages);
        Debug.Log($"当前速度提升：{speed}");
        StartCoroutine(RemoveSpeedBoostAfterTime(boostAmount, duration));
    }

    private IEnumerator RemoveSpeedBoostAfterTime(float boostAmount, float duration)
    {
        yield return new WaitForSeconds(duration);

        speed -= boostAmount;
        speed = Mathf.Max(0, speed); // 防止小于0

        int currentStages = Mathf.CeilToInt((speed - 6) / 2) + 1; // 计算当前增益阶段
        playerStatusEffects.UpdateBlueBar(currentStages);
        Debug.Log($"速度增强效果结束，当前速度提升：{speed}");
    }


    // 在场景中显示攻击范围，方便调试
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void ForceIdleState()
    {
        // 停止玩家移动
        if (controller != null)
        {
            controller.Move(Vector3.zero);
        }

        // 强制进入 Idle 动画状态
        ChangeAnimationState(IDLE_BATTLE);
    }

}
