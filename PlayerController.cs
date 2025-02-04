using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public Transform cam;

    public float sprintMultiplier = 1.5f;      // ���ٱ���
    public float slowWalkMultiplier = 0.5f;    // �������߱���
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Animator animator;
    private string currentState;

    const string IDLE_BATTLE = "Idle_Battle";
    const string RUNFORWARDBATTLE = "RunForwardBattle";
    const string WALKFORWARDBATTLE = "WalkForwardBattle";
    const string ATTACK01 = "Attack01";       // ��������1
    const string ATTACK02 = "Attack02";       // ��������2
    const string DEFEND = "Defend";           // ��������
    const string HIT = "GetHit";              // �ܻ���������
    const string DIE = "Die";                 // ������������

    private bool isAttacking = false;
    private bool isDefending = false;
    private bool isDead = false;              // �Ƿ�����
    private bool isGettingHit = false;        // �Ƿ����ڲ����ܻ�����

    private float gravity = -9.81f;
    private Vector3 velocity;

    // �������
    public Transform attackPoint;          // ������λ��
    public float attackRange = 1.5f;       // ������Χ
    public int attackDamage = 10;          // �����˺�
    public LayerMask enemyLayers;          // �����˲㼶

    // �ܻ����
    public int maxHealth = 10000;            // �������ֵ
    private int currentHealth;             // ��ǰ����ֵ

    public GameManager gameManager;
    public PlayerHealthBar healthBar;
    public PlayerStatusEffects playerStatusEffects;


    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;         // ��ʼ������ֵ
        healthBar.InitializeHealthBar(currentHealth);
    }

    void ChangeAnimationState(string newState)
    {
        // ��ֹͬһ������״̬�ٴδ���Լ�
        if (currentState == newState) return;

        // �����µĶ���״̬
        animator.Play(newState);
        currentState = newState;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return; // �������Ѿ����������ٴ����κ��߼�

        // Ӧ������
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ȷ����ɫ�����ڵ���
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ������ڹ������ܻ������Ȳ��Ŷ�����������������
        if (isAttacking || isGettingHit) return;

        // ����������⣬��ס�ո����������
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isDefending)
            {
                isDefending = true;
                ChangeAnimationState(DEFEND);
            }
            return; // ����״̬�²�������������
        }
        else
        {
            // �ɿ��ո������������
            if (isDefending)
            {
                isDefending = false;
                ChangeAnimationState(IDLE_BATTLE); // �ص�����״̬
            }
        }

        // �ƶ��߼�
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
                controller.Move(moveDir.normalized * speed * slowWalkMultiplier * Time.deltaTime); // ʹ���������߱���
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

        // ��������
        if (Input.GetMouseButtonDown(0)) // ����������
        {
            StartCoroutine(PerformAttack(ATTACK01));
        }
        else if (Input.GetMouseButtonDown(1)) // ����Ҽ�����
        {
            StartCoroutine(PerformAttack(ATTACK02));
        }
    }

    private IEnumerator PerformAttack(string attackAnimation)
    {
        isAttacking = true;
        ChangeAnimationState(attackAnimation);

        // ��⹥����Χ�ڵĵ���
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("���е��ˣ�" + enemy.name);
            enemy.GetComponent<MonsterAI>()?.TakeDamage(attackDamage);
        }

        // �ȴ��������������ʱ��
        yield return new WaitForSeconds(0.75f);

        isAttacking = false;
    }

    public void Heal(int amount)
    {
        healthBar.Heal(amount);
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // ȷ������ֵ���ᳬ�����ֵ
        Debug.Log($"�������ֵ�ָ�����ǰ����ֵΪ {currentHealth}");
    }
    public void TakeDamage(int damage)
    {
        // ����Ѿ������������ܻ����ڷ���״̬���������˺�
        if (isDead || isGettingHit || isDefending)
        {
            if (isDefending)
            {
                Debug.Log("������ڷ�����δ�ܵ��˺���");
            }
            return;
        }

        healthBar.TakeDamage(damage);
        currentHealth -= damage;
        Debug.Log($"����ܵ��˺���{damage}��ʣ��Ѫ����{currentHealth}");

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
        ChangeAnimationState(HIT); // �����ܻ�����

        // �����ܻ�����ʱ��Ϊ 0.5 ��
        yield return new WaitForSeconds(0.5f);

        isGettingHit = false;
    }

    private void Die()
    {
        if (isDead) return; // ��ֹ�ظ�����
        isDead = true;

        Debug.Log("���������");
        ChangeAnimationState(DIE); // ������������

        // ����Э�̵ȴ�����������Ϻ�����ʾScoreUI
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        // ��ȡ��������ʱ��
        float deathAnimationDuration = GetAnimationDuration(DIE);

        // �ȴ������������
        yield return new WaitForSeconds(deathAnimationDuration);

        // ���� GameManager �Ľ�������ʱ�߼�
        gameManager.EndCountdown();
    }

    // ��ȡ�������ȵĸ�������
    private float GetAnimationDuration(string animationName)
    {
        if (animator == null || string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("Animator �򶯻�������Ч������Ĭ��ʱ��");
            return 1f; // ����Ĭ��ʱ�����������
        }

        // ��ȡ��ǰ��������
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length; // ���ض���ʱ��
            }
        }

        Debug.LogWarning($"δ�ҵ����� {animationName}������Ĭ��ʱ��");
        return 1f; // ���δ�ҵ�������Ĭ��ʱ��
    }

    public void AddAttackBoost(int boostAmount, float duration, int maxBoost)
    {
        int totalBoost = Mathf.Clamp(attackDamage + boostAmount, 0, maxBoost); // ��ֹ��������
        attackDamage = totalBoost;

        // ���� PlayerStatusEffects �� UI
        int currentStages = Mathf.CeilToInt((attackDamage - 25) / 25) + 1; // ������������UI�׶�
        playerStatusEffects.UpdateGreenBar(currentStages);
        Debug.Log($"��ǰ������������{attackDamage}");
        StartCoroutine(RemoveAttackBoostAfterTime(boostAmount, duration));
    }

    private IEnumerator RemoveAttackBoostAfterTime(int boostAmount, float duration)
    {
        yield return new WaitForSeconds(duration);

        attackDamage -= boostAmount;
        attackDamage = Mathf.Max(0, attackDamage); // ��ֹС��0

        int currentStages = Mathf.CeilToInt((attackDamage - 25) / 25) + 1; // ������������UI�׶�
        playerStatusEffects.UpdateGreenBar(currentStages);
        Debug.Log($"��������ǿЧ����������ǰ������������{attackDamage}");
    }

    public void AddSpeedBoost(float boostAmount, float duration, float maxBoost)
    {
        float totalBoost = Mathf.Clamp(speed + boostAmount, 0, maxBoost); // ��ֹ��������
        speed = totalBoost;

        int currentStages = Mathf.CeilToInt((speed - 6) / 2) + 1; // ���㵱ǰ����׶�
        playerStatusEffects.UpdateBlueBar(currentStages);
        Debug.Log($"��ǰ�ٶ�������{speed}");
        StartCoroutine(RemoveSpeedBoostAfterTime(boostAmount, duration));
    }

    private IEnumerator RemoveSpeedBoostAfterTime(float boostAmount, float duration)
    {
        yield return new WaitForSeconds(duration);

        speed -= boostAmount;
        speed = Mathf.Max(0, speed); // ��ֹС��0

        int currentStages = Mathf.CeilToInt((speed - 6) / 2) + 1; // ���㵱ǰ����׶�
        playerStatusEffects.UpdateBlueBar(currentStages);
        Debug.Log($"�ٶ���ǿЧ����������ǰ�ٶ�������{speed}");
    }


    // �ڳ�������ʾ������Χ���������
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void ForceIdleState()
    {
        // ֹͣ����ƶ�
        if (controller != null)
        {
            controller.Move(Vector3.zero);
        }

        // ǿ�ƽ��� Idle ����״̬
        ChangeAnimationState(IDLE_BATTLE);
    }

}
