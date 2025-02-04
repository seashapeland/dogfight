using UnityEngine;

public class GreenApple : MonoBehaviour
{
    public int attackBoostAmount = 25; // 增加的攻击力
    public float boostDuration = 10f; // 增强效果持续时间
    public int maxAttackBoost = 100;   // 最大攻击力加成上限

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 检查是否是玩家触碰
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"玩家触碰绿苹果，攻击力增强 {attackBoostAmount} 点，持续 {boostDuration} 秒！");
                player.AddAttackBoost(attackBoostAmount, boostDuration, maxAttackBoost); // 调用玩家的增强方法
            }

            Destroy(gameObject); // 摧毁苹果
        }
    }
}
