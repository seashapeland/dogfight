using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject redEnemyPrefab;   // Red ���˵�Ԥ����
    public GameObject greenEnemyPrefab; // Green ���˵�Ԥ����
    public GameObject blueEnemyPrefab;  // Blue ���˵�Ԥ����
    public float respawnDelay = 3f;     // �����ӳ�ʱ��

    // �������ɸ��ʷֲ�����˳���Ӧ red, green, blue
    private float[] probabilities = { 0.4f, 0.3f, 0.3f };

    // ֪ͨ���������������µĵ���
    public void NotifyEnemyDeath(Vector3 position)
    {
        StartCoroutine(RespawnEnemy(position));
    }

    private IEnumerator RespawnEnemy(Vector3 position)
    {
        yield return new WaitForSeconds(respawnDelay);

        // ���ݸ���������ɵ���
        GameObject enemyPrefab = GetRandomEnemyPrefab();

        // ��ԭλ�������µĵ���
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        // ��ȡ MonsterAI ��������õ�������
        MonsterAI monsterAI = newEnemy.GetComponent<MonsterAI>();
        if (monsterAI != null)
        {
            if (enemyPrefab == redEnemyPrefab)
            {
                monsterAI.enemyType = MonsterAI.EnemyType.Red;
            }
            else if (enemyPrefab == greenEnemyPrefab)
            {
                monsterAI.enemyType = MonsterAI.EnemyType.Green;
            }
            else if (enemyPrefab == blueEnemyPrefab)
            {
                monsterAI.enemyType = MonsterAI.EnemyType.Blue;
            }
        }
        newEnemy.GetComponent<MonsterAI>().manager = this; // ���ù���������
    }

    private GameObject GetRandomEnemyPrefab()
    {
        float randomValue = Random.Range(0f, 1f); // ����� [0, 1)
        float cumulativeProbability = 0f;

        if (probabilities.Length != 3)
        {
            Debug.LogError("Probability array must match the number of enemy types.");
            return null;
        }

        // ���ݸ��ʷ��ض�Ӧ��Ԥ����
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                switch (i)
                {
                    case 0: return redEnemyPrefab;
                    case 1: return greenEnemyPrefab;
                    case 2: return blueEnemyPrefab;
                }
            }
        }

        // Ĭ�Ϸ��� Red �����Է����ʳ���
        return redEnemyPrefab;
    }
}
