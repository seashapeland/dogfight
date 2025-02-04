using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenu : MonoBehaviour
{
    public AudioSource backgroundMusic; // 背景音乐 AudioSource
    public UIManager uiManager;     // UI 管理器

    void Start()
    {
        uiManager.ShowMainMenu();

        PauseBackground(); // 暂停背景逻辑
    }

    // 开始游戏
    public void StartGame()
    {

        uiManager.ShowPlayerUI();
        ResumeBackground(); // 恢复背景逻辑
        backgroundMusic.Play(); // 播放背景音乐

        // 如果需要切换场景，可以加载主游戏场景
        // SceneManager.LoadScene("GameScene"); // 可选
    }

    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("退出游戏！");
        Application.Quit();
    }

    // 暂停背景
    private void PauseBackground()
    {
        Time.timeScale = 0; // 暂停所有的物理和动画更新
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true; // 显示鼠标
    }

    // 恢复背景
    private void ResumeBackground()
    {
        Time.timeScale = 1; // 恢复所有的物理和动画更新
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false; // 隐藏鼠标
    }
}
