using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMenu : MonoBehaviour
{
    void Start()
    {
        PauseBackground();
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
