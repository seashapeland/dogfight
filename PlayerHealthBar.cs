using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Slider 组件
    public Image fillImage; // 填充条的图片

    private int maxHealth; // 最大血量
    private int currentHealth; // 当前血量
    private Color defaultColor = new Color(0.4745f, 1f, 0.3294f); // 正常血量时的颜色
    private Color lowHealthColor = new Color(1f, 0.33f, 0.33f); // FF5454 转为 RGB (1.0, 0.33, 0.33)

    // 初始化血条
    public void InitializeHealthBar(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        UpdateHealthBarColor(); // 初始化颜色
    }

    // 玩家受到伤害
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 防止血量小于0
        healthSlider.value = currentHealth;

        UpdateHealthBarColor(); // 更新颜色

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 玩家回血
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 防止血量超过最大值
        healthSlider.value = currentHealth;

        UpdateHealthBarColor(); // 更新颜色
    }

    // 更新血条颜色
    private void UpdateHealthBarColor()
    {
        if (fillImage != null)
        {
            if (currentHealth <= maxHealth / 4)
            {
                fillImage.color = lowHealthColor; // 低血量时变为红色
            }
            else
            {
                fillImage.color = defaultColor; // 正常血量时为绿色
            }
        }
    }

    // 玩家死亡逻辑
    private void Die()
    {
        Debug.Log("玩家死亡！");
        // 添加其他死亡逻辑
    }
}
