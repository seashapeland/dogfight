using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenu : MonoBehaviour
{
    public AudioSource backgroundMusic; // �������� AudioSource
    public UIManager uiManager;     // UI ������

    void Start()
    {
        uiManager.ShowMainMenu();

        PauseBackground(); // ��ͣ�����߼�
    }

    // ��ʼ��Ϸ
    public void StartGame()
    {

        uiManager.ShowPlayerUI();
        ResumeBackground(); // �ָ������߼�
        backgroundMusic.Play(); // ���ű�������

        // �����Ҫ�л����������Լ�������Ϸ����
        // SceneManager.LoadScene("GameScene"); // ��ѡ
    }

    // �˳���Ϸ
    public void QuitGame()
    {
        Debug.Log("�˳���Ϸ��");
        Application.Quit();
    }

    // ��ͣ����
    private void PauseBackground()
    {
        Time.timeScale = 0; // ��ͣ���е�����Ͷ�������
        Cursor.lockState = CursorLockMode.None; // �������
        Cursor.visible = true; // ��ʾ���
    }

    // �ָ�����
    private void ResumeBackground()
    {
        Time.timeScale = 1; // �ָ����е�����Ͷ�������
        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false; // �������
    }
}
