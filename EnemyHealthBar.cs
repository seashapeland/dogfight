using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;   // Ѫ��Ԥ����
    public Transform healthBarPosition; // Ѫ��������λ�ã�ͨ���ǵ���ͷ����
    private Slider healthBar;           // ����Ѫ�����

    private int maxHealth = 100;        // �������ֵ
    private int currentHealth;          // ��ǰ����ֵ
    public Camera targetCamera;         // �� Inspector ������ֶ����������
    private Transform player; // ������ҵ� Transform
    public float maxDisplayDistance = 45f; // Ѫ����ʾ��������

    private void Start()
    {
        currentHealth = maxHealth;

        // ʵ����Ѫ��
        GameObject healthBarObject = Instantiate(healthBarPrefab, healthBarPosition.position, Quaternion.identity);
        healthBarObject.transform.SetParent(GameObject.Find("Canvas").transform, false); // ����Ϊ UI ���Ӷ���

        // ��ȡ Slider ���
        healthBar = healthBarObject.GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the player has the correct tag.");
        }
    }

    private void Update()
    {
        if (healthBar == null || targetCamera == null || healthBarPosition == null || player == null)
        {
            Debug.LogWarning("Missing components for health bar functionality.");
            return;
        }

        // �����������֮��ľ���
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // ������볬�������ʾ���룬��ֱ������Ѫ��
        if (distanceToPlayer > maxDisplayDistance)
        {
            healthBar.gameObject.SetActive(false);
            return;
        }

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = targetCamera.WorldToScreenPoint(healthBarPosition.position);

        // �����Ļ�����Ƿ�����Ļ��Χ��
        bool isVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;

        // ����Ѫ���Ŀɼ���
        healthBar.gameObject.SetActive(isVisible);

        // ����ɼ��������Ѫ��λ��
        if (isVisible)
        {
            healthBar.transform.position = screenPosition;
        }
    }

    // �ܵ��˺�ʱ����Ѫ��
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Destroy(healthBar.gameObject); // ͬʱ����Ѫ��
        }
    }
}
