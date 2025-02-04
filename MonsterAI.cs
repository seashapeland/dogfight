using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public NavMeshAgent nav;
    public Transform target;

    public enum EnemyType { Red, Green, Blue } // 定义敌人类型
    public EnemyType enemyType;               // 当前敌人类型

    public float attackRange = 2f;             // 攻击范围
    public float detectionRange = 10f;         // 追踪范围
    public float attackCooldown = 1f;          // 攻击冷却时间
    public float stateLockDuration = 1f;       // 状态锁定时间

    public EnemyManager manager;               // 引用管理器
    public GameManager gameManager;

    private Vector3 initialPosition;           // 初始位置

    public int maxHealth = 100;                // 最大生命值
    private int currentHealth;                 // 当前生命值

    public int attackDamage = 20;              // 攻击伤害

    private const string runAnimation = "RunForwardBattle";   // 追击动画名称
    private const string attackAnimation = "Attack01";        // 攻击动画名称
    private const string idleAnimation = "Idle_Battle";       // 待机动画名称
    private const string hitAnimation = "GetHit";             // 受击动画名称
    private const string dieAnimation = "Die";                // 死亡动画名称

    private Animator animator;
    private string currentState;               // 当前动画状态
    private bool isDead = false;               // 是否死亡
    private bool isGettingHit = false;         // 是否正在播放受击动画
    private float lastAttackTime = 0f;         // 上一次攻击的时间
    private float stateLockTime = 0f;          // 状态锁定计时

    // 苹果
    public GameObject redApplePrefab;
    public GameObject greenApplePrefab;
    public GameObject blueApplePrefab;
    public float appleDestroyDelay = 20f; // 苹果销毁延迟时间

    private EnemyHealthBar healthBar;

    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    private State currentStateAI;

    void Start()
    {
        if (nav == null)
        {
            nav = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        currentHealth = maxHealth;
        initialPosition = transform.position;  // 记录初始位置
        currentStateAI = State.Idle;
        ChangeAnimationState(idleAnimation);

        // 动态找到 EnemyHealthBar 组件
        healthBar = GetComponent<EnemyHealthBar>();

        // 根据敌人类型执行特定初始化逻辑
        switch (enemyType)
        {
            case EnemyType.Red:
                Debug.Log("This is a Red enemy!");
                break;
            case EnemyType.Green:
                Debug.Log("This is a Green enemy!");
                break;
            case EnemyType.Blue:
                Debug.Log("This is a Blue enemy!");
                break;
        }

    }

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 如果状态锁定时间未结束，则保持当前状态
        if (Time.time < stateLockTime)
        {
            return;
        }

        // 状态机处理不同状态
        switch (currentStateAI)
        {
            case State.Idle:
                HandleIdleState(distanceToTarget);
                break;

            case State.Chase:
                HandleChaseState(distanceToTarget);
                break;

            case State.Attack:
                HandleAttackState(distanceToTarget);
                break;
        }
    }

    // 空闲状态处理
    private void HandleIdleState(float distanceToTarget)
    {
        nav.isStopped = true;
        ChangeAnimationState(idleAnimation);

        // 如果玩家进入检测范围，切换到追踪状态
        if (distanceToTarget <= detectionRange)
        {
            SwitchState(State.Chase);
        }
    }

    // 追踪状态处理
    private void HandleChaseState(float distanceToTarget)
    {
        nav.isStopped = false;
        nav.SetDestination(target.position);
        ChangeAnimationState(runAnimation);

        // 如果玩家进入攻击范围，切换到攻击状态
        if (distanceToTarget <= attackRange)
        {
            nav.isStopped = true;
            SwitchState(State.Attack);
        }
        // 如果玩家离开检测范围，切换到空闲状态
        else if (distanceToTarget > detectionRange)
        {
            SwitchState(State.Idle);
        }
    }

    // 攻击状态处理
    private void HandleAttackState(float distanceToTarget)
    {
        nav.isStopped = true;

        // 如果冷却时间已到，进行攻击
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
            lastAttackTime = Time.time; // 更新攻击时间
        }

        // 如果玩家超出攻击范围，切换到追踪状态
        if (distanceToTarget > attackRange)
        {
            SwitchState(State.Chase);
        }
    }

    // 切换状态并设置锁定时间
    private void SwitchState(State newState)
    {
        currentStateAI = newState;
        stateLockTime = Time.time + stateLockDuration; // 锁定当前状态，持续一定时间
    }

    private IEnumerator PerformAttack()
    {
        ChangeAnimationState(attackAnimation); // 播放攻击动画

        // 等待动画完成一定时间（假设攻击触发在动画播放的前 0.5 秒）
        yield return new WaitForSeconds(0.5f);

        // 检测攻击范围内的目标
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"Boss 攻击玩家，造成 {attackDamage} 点伤害");
                player.TakeDamage(attackDamage); // 对玩家造成伤害
            }
        }

        // 等待动画完全结束
        yield return new WaitForSeconds(0.25f);
    }

    // 受击逻辑
    public void TakeDamage(int damage)
    {
        if (isDead || isGettingHit) return; // 如果已经死亡或正在受击，不处理

        healthBar.TakeDamage(damage);
        currentHealth -= damage;
        Debug.Log($"Boss 受到伤害：{damage}，剩余血量：{currentHealth}");

        if (currentHealth > 0)
        {
            StartCoroutine(PlayHitAnimation());
        }
        else
        {
            Die();
        }
    }

    // 播放受击动画
    private IEnumerator PlayHitAnimation()
    {
        isGettingHit = true;
        ChangeAnimationState(hitAnimation); // 播放受击动画

        // 假设受击动画时长为 0.5 秒
        yield return new WaitForSeconds(0.5f);

        isGettingHit = false;
    }

    // 死亡逻辑
    private void Die()
    {
        isDead = true;
        Debug.Log("Boss 死亡！");

        gameManager.OnEnemyKilled();
        // 停止 NavMeshAgent
        nav.isStopped = true;
        manager.NotifyEnemyDeath(initialPosition); // 通知管理器

        // 播放死亡动画
        ChangeAnimationState(dieAnimation);

        // 记录死亡时的当前位置
        Vector3 deathPosition = transform.position;

        // 延迟生成苹果
        SpawnApple(deathPosition);

        // 延迟销毁
        Destroy(gameObject, 2f);
    }

    private void SpawnApple(Vector3 spawnPosition)
    {
        GameObject applePrefabToSpawn = null;

        switch (enemyType)
        {
            case EnemyType.Red:
                applePrefabToSpawn = redApplePrefab;
                break;
            case EnemyType.Green:
                applePrefabToSpawn = greenApplePrefab;
                break;
            case EnemyType.Blue:
                applePrefabToSpawn = blueApplePrefab;
                break;
        }

        if (applePrefabToSpawn != null)
        {
            GameObject apple = Instantiate(applePrefabToSpawn, spawnPosition, Quaternion.identity);
            Destroy(apple, appleDestroyDelay);
        }
    }

    // 切换动画状态的函数，避免重复播放同一个动画
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return; // 防止重复播放同一动画

        animator.Play(newState);
        currentState = newState;
    }
}
