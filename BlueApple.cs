using UnityEngine;

public class BlueApple : MonoBehaviour
{
    public float speedBoostAmount = 2f; // ���ӵ��ٶ�
    public float boostDuration = 10f;  // ��ǿЧ������ʱ��
    public float maxSpeedBoost = 10f;   // ����ٶȼӳ�����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ����Ƿ�����Ҵ���
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"��Ҵ�����ƻ�����ٶ���ǿ {speedBoostAmount}������ {boostDuration} �룡");
                player.AddSpeedBoost(speedBoostAmount, boostDuration, maxSpeedBoost); // ������ҵ���ǿ����
            }

            Destroy(gameObject); // �ݻ�ƻ��
        }
    }
}
