using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public NavMeshAgent nav;
    public Transform target;

    public enum EnemyType { Red, Green, Blue } // �����������
    public EnemyType enemyType;               // ��ǰ��������

    public float attackRange = 2f;             // ������Χ
    public float detectionRange = 10f;         // ׷�ٷ�Χ
    public float attackCooldown = 1f;          // ������ȴʱ��
    public float stateLockDuration = 1f;       // ״̬����ʱ��

    public EnemyManager manager;               // ���ù�����
    public GameManager gameManager;

    private Vector3 initialPosition;           // ��ʼλ��

    public int maxHealth = 100;                // �������ֵ
    private int currentHealth;                 // ��ǰ����ֵ

    public int attackDamage = 20;              // �����˺�

    private const string runAnimation = "RunForwardBattle";   // ׷����������
    private const string attackAnimation = "Attack01";        // ������������
    private const string idleAnimation = "Idle_Battle";       // ������������
    private const string hitAnimation = "GetHit";             // �ܻ���������
    private const string dieAnimation = "Die";                // ������������

    private Animator animator;
    private string currentState;               // ��ǰ����״̬
    private bool isDead = false;               // �Ƿ�����
    private bool isGettingHit = false;         // �Ƿ����ڲ����ܻ�����
    private float lastAttackTime = 0f;         // ��һ�ι�����ʱ��
    private float stateLockTime = 0f;          // ״̬������ʱ

    // ƻ��
    public GameObject redApplePrefab;
    public GameObject greenApplePrefab;
    public GameObject blueApplePrefab;
    public float appleDestroyDelay = 20f; // ƻ�������ӳ�ʱ��

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
        initialPosition = transform.position;  // ��¼��ʼλ��
        currentStateAI = State.Idle;
        ChangeAnimationState(idleAnimation);

        // ��̬�ҵ� EnemyHealthBar ���
        healthBar = GetComponent<EnemyHealthBar>();

        // ���ݵ�������ִ���ض���ʼ���߼�
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

        // ���״̬����ʱ��δ�������򱣳ֵ�ǰ״̬
        if (Time.time < stateLockTime)
        {
            return;
        }

        // ״̬������ͬ״̬
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

    // ����״̬����
    private void HandleIdleState(float distanceToTarget)
    {
        nav.isStopped = true;
        ChangeAnimationState(idleAnimation);

        // �����ҽ����ⷶΧ���л���׷��״̬
        if (distanceToTarget <= detectionRange)
        {
            SwitchState(State.Chase);
        }
    }

    // ׷��״̬����
    private void HandleChaseState(float distanceToTarget)
    {
        nav.isStopped = false;
        nav.SetDestination(target.position);
        ChangeAnimationState(runAnimation);

        // �����ҽ��빥����Χ���л�������״̬
        if (distanceToTarget <= attackRange)
        {
            nav.isStopped = true;
            SwitchState(State.Attack);
        }
        // �������뿪��ⷶΧ���л�������״̬
        else if (distanceToTarget > detectionRange)
        {
            SwitchState(State.Idle);
        }
    }

    // ����״̬����
    private void HandleAttackState(float distanceToTarget)
    {
        nav.isStopped = true;

        // �����ȴʱ���ѵ������й���
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
            lastAttackTime = Time.time; // ���¹���ʱ��
        }

        // �����ҳ���������Χ���л���׷��״̬
        if (distanceToTarget > attackRange)
        {
            SwitchState(State.Chase);
        }
    }

    // �л�״̬����������ʱ��
    private void SwitchState(State newState)
    {
        currentStateAI = newState;
        stateLockTime = Time.time + stateLockDuration; // ������ǰ״̬������һ��ʱ��
    }

    private IEnumerator PerformAttack()
    {
        ChangeAnimationState(attackAnimation); // ���Ź�������

        // �ȴ��������һ��ʱ�䣨���蹥�������ڶ������ŵ�ǰ 0.5 �룩
        yield return new WaitForSeconds(0.5f);

        // ��⹥����Χ�ڵ�Ŀ��
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"Boss ������ң���� {attackDamage} ���˺�");
                player.TakeDamage(attackDamage); // ���������˺�
            }
        }

        // �ȴ�������ȫ����
        yield return new WaitForSeconds(0.25f);
    }

    // �ܻ��߼�
    public void TakeDamage(int damage)
    {
        if (isDead || isGettingHit) return; // ����Ѿ������������ܻ���������

        healthBar.TakeDamage(damage);
        currentHealth -= damage;
        Debug.Log($"Boss �ܵ��˺���{damage}��ʣ��Ѫ����{currentHealth}");

        if (currentHealth > 0)
        {
            StartCoroutine(PlayHitAnimation());
        }
        else
        {
            Die();
        }
    }

    // �����ܻ�����
    private IEnumerator PlayHitAnimation()
    {
        isGettingHit = true;
        ChangeAnimationState(hitAnimation); // �����ܻ�����

        // �����ܻ�����ʱ��Ϊ 0.5 ��
        yield return new WaitForSeconds(0.5f);

        isGettingHit = false;
    }

    // �����߼�
    private void Die()
    {
        isDead = true;
        Debug.Log("Boss ������");

        gameManager.OnEnemyKilled();
        // ֹͣ NavMeshAgent
        nav.isStopped = true;
        manager.NotifyEnemyDeath(initialPosition); // ֪ͨ������

        // ������������
        ChangeAnimationState(dieAnimation);

        // ��¼����ʱ�ĵ�ǰλ��
        Vector3 deathPosition = transform.position;

        // �ӳ�����ƻ��
        SpawnApple(deathPosition);

        // �ӳ�����
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

    // �л�����״̬�ĺ����������ظ�����ͬһ������
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return; // ��ֹ�ظ�����ͬһ����

        animator.Play(newState);
        currentState = newState;
    }
}
