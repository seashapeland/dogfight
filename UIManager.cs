using TMPro; // ���� TextMeshPro �����ռ�
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;  // ���˵�
    public GameObject pauseMenu; // ��ͣ�˵�
    public GameObject scoreUi;   // ��������
    public GameObject playerUi;  // ��� UI

    public TMP_Text playerTimeText; // ��ҵ���ʱ�ı� (TextMeshPro)
    public TMP_Text scoreText;      // ����������ʾ���ı� (TextMeshPro)
    public Slider healthSlider; // Slider ���

    public GameObject green;
    public GameObject blue;
    public Image g0;
    public Image b0;

    // ��ʾ���˵�
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        scoreUi.SetActive(false);
        playerUi.SetActive(false);
    }

    // ��ʾ��ͣ�˵�
    public void ShowPauseMenu()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(true);
        scoreUi.SetActive(false);
        playerUi.SetActive(false);
    }

    // ��ʾ��������
    public void ShowScoreUI()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        scoreUi.SetActive(true);
        playerUi.SetActive(false);
    }

    // ��ʾ��� UI
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

    // ���� PlayerUI ��ʱ���ı�
    public void UpdatePlayerUiText(string text)
    {
        if (playerTimeText != null)
        {
            if (!playerTimeText.gameObject.activeSelf)
            {
                playerTimeText.gameObject.SetActive(true); // ����ı�δ���������
            }

            playerTimeText.text = text; // �����ı�����
        }
        else
        {
            Debug.LogError("PlayerTimeText δ���䣬���� Inspector ���ã�");
        }
    }

    // ���·��������еķ����ı�
    public void UpdateScoreUiText(string text)
    {
        if (scoreText != null)
        {
            scoreText.text = text;
        }
    }

    public void ReturnToStart()
    {
        // ���¼��ص�ǰ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
