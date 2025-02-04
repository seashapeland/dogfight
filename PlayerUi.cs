using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEffects : MonoBehaviour
{
    public Transform greenBarParent; // green父物体
    public Transform blueBarParent;  // blue父物体
    public Color activeGreenColor = new Color(0.31f, 0.78f, 0.33f); // 激活时绿色
    public Color activeBlueColor = new Color(0.25f, 0.44f, 0.91f);  // 激活时蓝色
    public Color32 defaultColor = new Color32(255, 255, 255, 80);

    private int maxStages = 3; // 最大阶段
    private float attackMultiplier = 1f; // 当前攻击倍数
    private float speedMultiplier = 1f;  // 当前速度倍数
    private float maxMultiplier = 4f;    // 最大倍数 (初始倍数 + 3)

    private void Start()
    {
        UpdateBar(greenBarParent, attackMultiplier, activeGreenColor);
        UpdateBar(blueBarParent, speedMultiplier, activeBlueColor);
    }


    // 更新UI的方法
    public void UpdateGreenBar(float multiplier)
    {
        attackMultiplier = Mathf.Clamp(multiplier, 1f, maxMultiplier); // 限制倍数
        UpdateBar(greenBarParent, attackMultiplier, activeGreenColor);
    }

    public void UpdateBlueBar(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 1f, maxMultiplier); // 限制倍数
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
                    stageImage.color = activeColor; // 激活颜色
                }
                else
                {
                    stageImage.color = defaultColor; // 恢复灰色
                }
            }
        }
    }

    // 重置UI（例如效果超时后）
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
