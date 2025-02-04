using UnityEngine;

public class BlueApple : MonoBehaviour
{
    public float speedBoostAmount = 2f; // 增加的速度
    public float boostDuration = 10f;  // 增强效果持续时间
    public float maxSpeedBoost = 10f;   // 最大速度加成上限

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 检查是否是玩家触碰
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"玩家触碰蓝苹果，速度增强 {speedBoostAmount}，持续 {boostDuration} 秒！");
                player.AddSpeedBoost(speedBoostAmount, boostDuration, maxSpeedBoost); // 调用玩家的增强方法
            }

            Destroy(gameObject); // 摧毁苹果
        }
    }
}
