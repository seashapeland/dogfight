using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Slider ���
    public Image fillImage; // �������ͼƬ

    private int maxHealth; // ���Ѫ��
    private int currentHealth; // ��ǰѪ��
    private Color defaultColor = new Color(0.4745f, 1f, 0.3294f); // ����Ѫ��ʱ����ɫ
    private Color lowHealthColor = new Color(1f, 0.33f, 0.33f); // FF5454 תΪ RGB (1.0, 0.33, 0.33)

    // ��ʼ��Ѫ��
    public void InitializeHealthBar(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        UpdateHealthBarColor(); // ��ʼ����ɫ
    }

    // ����ܵ��˺�
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ��ֹѪ��С��0
        healthSlider.value = currentHealth;

        UpdateHealthBarColor(); // ������ɫ

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ��һ�Ѫ
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ��ֹѪ���������ֵ
        healthSlider.value = currentHealth;

        UpdateHealthBarColor(); // ������ɫ
    }

    // ����Ѫ����ɫ
    private void UpdateHealthBarColor()
    {
        if (fillImage != null)
        {
            if (currentHealth <= maxHealth / 4)
            {
                fillImage.color = lowHealthColor; // ��Ѫ��ʱ��Ϊ��ɫ
            }
            else
            {
                fillImage.color = defaultColor; // ����Ѫ��ʱΪ��ɫ
            }
        }
    }

    // ��������߼�
    private void Die()
    {
        Debug.Log("���������");
        // ������������߼�
    }
}
