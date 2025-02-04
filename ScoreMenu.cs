using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMenu : MonoBehaviour
{
    void Start()
    {
        PauseBackground();
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
