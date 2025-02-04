using System.Collections;
using UnityEngine;

public class ArenaEntrance : MonoBehaviour
{
    public Animator doorAnimator;        // 门的 Animator 控制器
    public GameObject doorObject;        // 门的 GameObject
    public Camera mainCamera;            // 主摄像头
    public Camera doorCamera;            // 门的专用摄像头
    public AudioSource backgroundMusic;  // 背景音乐 AudioSource
    public AudioSource fightMusic;       // 战斗音乐 AudioSource
    public float fadeDuration = 1f;      // 音乐淡入淡出的时间
    public float cameraFocusDuration = 3f; // 门动画播放时摄像头停留的时间

    public GameManager gameManager; // 引用 GameManager
    public UIManager uiManager;

    private bool hasTriggered = false;   // 是否已经触发过逻辑

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            StartCoroutine(HandleArenaEntrance());
            if (gameManager != null)
            {
                gameManager.StartCountdown(); // 开始倒计时
            }
            if(uiManager != null)
            {
                uiManager.ShowBar();
                uiManager.ShowMul();
            }
            hasTriggered = true; // 标记为已触发
        }
    }

    private IEnumerator HandleArenaEntrance()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false; // 禁用玩家操作
            playerController.ForceIdleState();
        }

        // 激活门的 BoxCollider
        BoxCollider doorCollider = doorObject.GetComponent<BoxCollider>();
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        // 切换到门摄像头
        if (doorCamera != null && mainCamera != null)
        {
            mainCamera.enabled = false;
            doorCamera.enabled = true;
        }

        // 播放门动画
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Rise");
        }

        

        // 平滑切换音乐
        if (backgroundMusic != null && fightMusic != null)
        {
            StartCoroutine(SmoothMusicTransition());
        }

        // 停留摄像头一段时间（可选）
        yield return new WaitForSeconds(cameraFocusDuration);

        // 切回主摄像头
        if (doorCamera != null && mainCamera != null)
        {
            doorCamera.enabled = false;
            mainCamera.enabled = true;
            // 重新启用玩家控制
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
        else
        {
            Debug.LogError("MainCamera 或 doorCamera 未正确分配！");
            yield break; // 停止协程
        }

        
    }


    private IEnumerator SmoothMusicTransition()
    {
        float elapsed = 0f;

        // 渐渐降低背景音乐音量
        while (elapsed < fadeDuration)
        {
            backgroundMusic.volume = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        backgroundMusic.Stop();
        backgroundMusic.volume = 1f; // 重置背景音乐音量以备下次使用

        // 开始播放战斗音乐并渐渐提高音量
        fightMusic.Play();
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            fightMusic.volume = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fightMusic.volume = 1f; // 确保战斗音乐音量完全恢复
    }

    // 如果需要在动画结束后允许再次触发，可以添加一个重置标志的逻辑
    public void ResetTrigger()
    {
        hasTriggered = false; // 重置标志，允许再次触发
    }
}
