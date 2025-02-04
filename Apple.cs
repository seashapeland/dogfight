using UnityEngine;

public class Apple : MonoBehaviour
{
    public int healAmount = 50; // 回复的生命值

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 检查是否是玩家触碰
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount); // 调用玩家的回血方法
                Debug.Log($"玩家触碰苹果，恢复了 {healAmount} 点生命值！");
            }

            Destroy(gameObject); // 摧毁苹果
        }
    }
}
