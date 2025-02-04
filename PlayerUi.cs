using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEffects : MonoBehaviour
{
    public Transform greenBarParent; // green������
    public Transform blueBarParent;  // blue������
    public Color activeGreenColor = new Color(0.31f, 0.78f, 0.33f); // ����ʱ��ɫ
    public Color activeBlueColor = new Color(0.25f, 0.44f, 0.91f);  // ����ʱ��ɫ
    public Color32 defaultColor = new Color32(255, 255, 255, 80);

    private int maxStages = 3; // ���׶�
    private float attackMultiplier = 1f; // ��ǰ��������
    private float speedMultiplier = 1f;  // ��ǰ�ٶȱ���
    private float maxMultiplier = 4f;    // ����� (��ʼ���� + 3)

    private void Start()
    {
        UpdateBar(greenBarParent, attackMultiplier, activeGreenColor);
        UpdateBar(blueBarParent, speedMultiplier, activeBlueColor);
    }


    // ����UI�ķ���
    public void UpdateGreenBar(float multiplier)
    {
        attackMultiplier = Mathf.Clamp(multiplier, 1f, maxMultiplier); // ���Ʊ���
        UpdateBar(greenBarParent, attackMultiplier, activeGreenColor);
    }

    public void UpdateBlueBar(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 1f, maxMultiplier); // ���Ʊ���
        UpdateBar(blueBarParent, speedMultiplier, activeBlueColor);
    }

    private void UpdateBar(Transform barParent, float multiplier, Color activeColor)
    {
        for (int i = 0; i < maxStages; i++)
        {
            Image stageImage = barParent.GetChild(i).GetComponent<Image>();
            if (stageImage != null)
            {
                if (i < multiplier - 1)
                {
                    stageImage.color = activeColor; // ������ɫ
                }
                else
                {
                    stageImage.color = defaultColor; // �ָ���ɫ
                }
            }
        }
    }

    // ����UI������Ч����ʱ��
    public void ResetGreenBar()
    {
        attackMultiplier = 1f;
        UpdateGreenBar(attackMultiplier);
    }

    public void ResetBlueBar()
    {
        speedMultiplier = 1f;
        UpdateBlueBar(speedMultiplier);
    }
}
