using System.Collections;
using UnityEngine;

public class ArenaEntrance : MonoBehaviour
{
    public Animator doorAnimator;        // �ŵ� Animator ������
    public GameObject doorObject;        // �ŵ� GameObject
    public Camera mainCamera;            // ������ͷ
    public Camera doorCamera;            // �ŵ�ר������ͷ
    public AudioSource backgroundMusic;  // �������� AudioSource
    public AudioSource fightMusic;       // ս������ AudioSource
    public float fadeDuration = 1f;      // ���ֵ��뵭����ʱ��
    public float cameraFocusDuration = 3f; // �Ŷ�������ʱ����ͷͣ����ʱ��

    public GameManager gameManager; // ���� GameManager
    public UIManager uiManager;

    private bool hasTriggered = false;   // �Ƿ��Ѿ��������߼�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            StartCoroutine(HandleArenaEntrance());
            if (gameManager != null)
            {
                gameManager.StartCountdown(); // ��ʼ����ʱ
            }
            if(uiManager != null)
            {
                uiManager.ShowBar();
                uiManager.ShowMul();
            }
            hasTriggered = true; // ���Ϊ�Ѵ���
        }
    }

    private IEnumerator HandleArenaEntrance()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false; // ������Ҳ���
            playerController.ForceIdleState();
        }

        // �����ŵ� BoxCollider
        BoxCollider doorCollider = doorObject.GetComponent<BoxCollider>();
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        // �л���������ͷ
        if (doorCamera != null && mainCamera != null)
        {
            mainCamera.enabled = false;
            doorCamera.enabled = true;
        }

        // �����Ŷ���
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Rise");
        }

        

        // ƽ���л�����
        if (backgroundMusic != null && fightMusic != null)
        {
            StartCoroutine(SmoothMusicTransition());
        }

        // ͣ������ͷһ��ʱ�䣨��ѡ��
        yield return new WaitForSeconds(cameraFocusDuration);

        // �л�������ͷ
        if (doorCamera != null && mainCamera != null)
        {
            doorCamera.enabled = false;
            mainCamera.enabled = true;
            // ����������ҿ���
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
        else
        {
            Debug.LogError("MainCamera �� doorCamera δ��ȷ���䣡");
            yield break; // ֹͣЭ��
        }

        
    }


    private IEnumerator SmoothMusicTransition()
    {
        float elapsed = 0f;

        // �������ͱ�����������
        while (elapsed < fadeDuration)
        {
            backgroundMusic.volume = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        backgroundMusic.Stop();
        backgroundMusic.volume = 1f; // ���ñ������������Ա��´�ʹ��

        // ��ʼ����ս�����ֲ������������
        fightMusic.Play();
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            fightMusic.volume = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fightMusic.volume = 1f; // ȷ��ս������������ȫ�ָ�
    }

    // �����Ҫ�ڶ��������������ٴδ������������һ�����ñ�־���߼�
    public void ResetTrigger()
    {
        hasTriggered = false; // ���ñ�־�������ٴδ���
    }
}
