using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;   // 血条预制体
    public Transform healthBarPosition; // 血条的世界位置（通常是敌人头部）
    private Slider healthBar;           // 引用血条组件

    private int maxHealth = 100;        // 最大生命值
    private int currentHealth;          // 当前生命值
    public Camera targetCamera;         // 在 Inspector 面板中手动分配摄像机
    private Transform player; // 引用玩家的 Transform
    public float maxDisplayDistance = 45f; // 血条显示的最大距离

    private void Start()
    {
        currentHealth = maxHealth;

        // 实例化血条
        GameObject healthBarObject = Instantiate(healthBarPrefab, healthBarPosition.position, Quaternion.identity);
        healthBarObject.transform.SetParent(GameObject.Find("Canvas").transform, false); // 设置为 UI 的子对象

        // 获取 Slider 组件
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

        // 检查敌人与玩家之间的距离
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // 如果距离超过最大显示距离，则直接隐藏血条
        if (distanceToPlayer > maxDisplayDistance)
        {
            healthBar.gameObject.SetActive(false);
            return;
        }

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = targetCamera.WorldToScreenPoint(healthBarPosition.position);

        // 检查屏幕坐标是否在屏幕范围内
        bool isVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;

        // 设置血条的可见性
        healthBar.gameObject.SetActive(isVisible);

        // 如果可见，则更新血条位置
        if (isVisible)
        {
            healthBar.transform.position = screenPosition;
        }
    }

    // 受到伤害时更新血条
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Destroy(healthBar.gameObject); // 同时销毁血条
        }
    }
}
