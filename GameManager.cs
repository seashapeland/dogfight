using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;     // UI 管理器
    public int countdownTime = 180; // 倒计时秒数，3:00 即 180 秒
    public AudioSource fightMusic;       // 战斗音乐 AudioSource

    private int enemyKillCount = 0;   // 玩家击杀敌人数量

    private bool isCountingDown = false;
    private bool isPaused = false; // 是否处于暂停状态

    private void Update()
    {
        // 检测 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // 恢复游戏
            }
            else
            {
                PauseGame(); // 暂停游戏
            }
        }
    }

    // 暂停游戏逻辑
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 暂停游戏时间
        uiManager.ShowPauseMenu(); // 显示暂停界面
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true; // 显示鼠标
        Debug.Log("Game Paused!");
    }

    // 恢复游戏逻辑
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 恢复游戏时间
        uiManager.ShowPlayerUI(); // 显示玩家UI
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false; // 隐藏鼠标
        Debug.Log("Game Resumed!");
    }

    // 玩家击杀敌人时调用的函数
    public void OnEnemyKilled()
    {
        enemyKillCount++; // 增加击杀计数
        Debug.Log($"Enemy killed! Total: {enemyKillCount}");
    }

    // 开始倒计时
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

        // 倒计时逻辑
        while (timeRemaining > 0)
        {
            int minutes = timeRemaining / 60;
            int seconds = timeRemaining % 60;
            uiManager.UpdatePlayerUiText($"{minutes:00} : {seconds:00}");
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        // 倒计时结束后逻辑
        EndCountdown();
    }

    public void EndCountdown()
    {
        isCountingDown = false;
        uiManager.UpdateScoreUiText($"{enemyKillCount}");
        uiManager.ShowScoreUI(); // 显示分数界面
        fightMusic.Stop();
    }
}
