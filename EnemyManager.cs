using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject redEnemyPrefab;   // Red 敌人的预制体
    public GameObject greenEnemyPrefab; // Green 敌人的预制体
    public GameObject blueEnemyPrefab;  // Blue 敌人的预制体
    public float respawnDelay = 3f;     // 重生延迟时间

    // 敌人生成概率分布，按顺序对应 red, green, blue
    private float[] probabilities = { 0.4f, 0.3f, 0.3f };

    // 通知敌人死亡并生成新的敌人
    public void NotifyEnemyDeath(Vector3 position)
    {
        StartCoroutine(RespawnEnemy(position));
    }

    private IEnumerator RespawnEnemy(Vector3 position)
    {
        yield return new WaitForSeconds(respawnDelay);

        // 根据概率随机生成敌人
        GameObject enemyPrefab = GetRandomEnemyPrefab();

        // 在原位置生成新的敌人
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        // 获取 MonsterAI 组件并设置敌人类型
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
        newEnemy.GetComponent<MonsterAI>().manager = this; // 设置管理器引用
    }

    private GameObject GetRandomEnemyPrefab()
    {
        float randomValue = Random.Range(0f, 1f); // 随机数 [0, 1)
        float cumulativeProbability = 0f;

        if (probabilities.Length != 3)
        {
            Debug.LogError("Probability array must match the number of enemy types.");
            return null;
        }

        // 根据概率返回对应的预制体
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

        // 默认返回 Red 敌人以防概率出错
        return redEnemyPrefab;
    }
}
