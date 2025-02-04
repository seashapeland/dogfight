using UnityEngine;

public class Apple : MonoBehaviour
{
    public int healAmount = 50; // �ظ�������ֵ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ����Ƿ�����Ҵ���
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount); // ������ҵĻ�Ѫ����
                Debug.Log($"��Ҵ���ƻ�����ָ��� {healAmount} ������ֵ��");
            }

            Destroy(gameObject); // �ݻ�ƻ��
        }
    }
}
