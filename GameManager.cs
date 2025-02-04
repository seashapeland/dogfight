using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;     // UI ������
    public int countdownTime = 180; // ����ʱ������3:00 �� 180 ��
    public AudioSource fightMusic;       // ս������ AudioSource

    private int enemyKillCount = 0;   // ��һ�ɱ��������

    private bool isCountingDown = false;
    private bool isPaused = false; // �Ƿ�����ͣ״̬

    private void Update()
    {
        // ��� ESC ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // �ָ���Ϸ
            }
            else
            {
                PauseGame(); // ��ͣ��Ϸ
            }
        }
    }

    // ��ͣ��Ϸ�߼�
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // ��ͣ��Ϸʱ��
        uiManager.ShowPauseMenu(); // ��ʾ��ͣ����
        Cursor.lockState = CursorLockMode.None; // �������
        Cursor.visible = true; // ��ʾ���
        Debug.Log("Game Paused!");
    }

    // �ָ���Ϸ�߼�
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // �ָ���Ϸʱ��
        uiManager.ShowPlayerUI(); // ��ʾ���UI
        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false; // �������
        Debug.Log("Game Resumed!");
    }

    // ��һ�ɱ����ʱ���õĺ���
    public void OnEnemyKilled()
    {
        enemyKillCount++; // ���ӻ�ɱ����
        Debug.Log($"Enemy killed! Total: {enemyKillCount}");
    }

    // ��ʼ����ʱ
    public void StartCountdown()
    {
        if (!isCountingDown)
        {
            isCountingDown = true;
            StartCoroutine(CountdownTimer());
        }
    }

    private IEnumerator CountdownTimer()
    {
        int timeRemaining = countdownTime;

        // ����ʱ�߼�
        while (timeRemaining > 0)
        {
            int minutes = timeRemaining / 60;
            int seconds = timeRemaining % 60;
            uiManager.UpdatePlayerUiText($"{minutes:00} : {seconds:00}");
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        // ����ʱ�������߼�
        EndCountdown();
    }

    public void EndCountdown()
    {
        isCountingDown = false;
        uiManager.UpdateScoreUiText($"{enemyKillCount}");
        uiManager.ShowScoreUI(); // ��ʾ��������
        fightMusic.Stop();
    }
}
