using UnityEngine;

public class GreenApple : MonoBehaviour
{
    public int attackBoostAmount = 25; // ���ӵĹ�����
    public float boostDuration = 10f; // ��ǿЧ������ʱ��
    public int maxAttackBoost = 100;   // ��󹥻����ӳ�����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ����Ƿ�����Ҵ���
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"��Ҵ�����ƻ������������ǿ {attackBoostAmount} �㣬���� {boostDuration} �룡");
                player.AddAttackBoost(attackBoostAmount, boostDuration, maxAttackBoost); // ������ҵ���ǿ����
            }

            Destroy(gameObject); // �ݻ�ƻ��
        }
    }
}
