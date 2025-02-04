using TMPro; // 引入 TextMeshPro 命名空间
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;  // 主菜单
    public GameObject pauseMenu; // 暂停菜单
    public GameObject scoreUi;   // 分数界面
    public GameObject playerUi;  // 玩家 UI

    public TMP_Text playerTimeText; // 玩家倒计时文本 (TextMeshPro)
    public TMP_Text scoreText;      // 分数界面显示的文本 (TextMeshPro)
    public Slider healthSlider; // Slider 组件

    public GameObject green;
    public GameObject blue;
    public Image g0;
    public Image b0;

    // 显示主菜单
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        scoreUi.SetActive(false);
        playerUi.SetActive(false);
    }

    // 显示暂停菜单
    public void ShowPauseMenu()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(true);
        scoreUi.SetActive(false);
        playerUi.SetActive(false);
    }

    // 显示分数界面
    public void ShowScoreUI()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        scoreUi.SetActive(true);
        playerUi.SetActive(false);
    }

    // 显示玩家 UI
    public void ShowPlayerUI()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        scoreUi.SetActive(false);
        playerUi.SetActive(true);
    }

    public void ShowBar()
    {
        healthSlider.gameObject.SetActive(true);
    }

    public void ShowMul()
    { 
        green.gameObject.SetActive(true);
        blue.gameObject.SetActive(true);
        g0.gameObject.SetActive(true);
        b0.gameObject.SetActive(true);
    }

    // 更新 PlayerUI 的时间文本
    public void UpdatePlayerUiText(string text)
    {
        if (playerTimeText != null)
        {
            if (!playerTimeText.gameObject.activeSelf)
            {
                playerTimeText.gameObject.SetActive(true); // 如果文本未激活，激活它
            }

            playerTimeText.text = text; // 更新文本内容
        }
        else
        {
            Debug.LogError("PlayerTimeText 未分配，请检查 Inspector 设置！");
        }
    }

    // 更新分数界面中的分数文本
    public void UpdateScoreUiText(string text)
    {
        if (scoreText != null)
        {
            scoreText.text = text;
        }
    }

    public void ReturnToStart()
    {
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
