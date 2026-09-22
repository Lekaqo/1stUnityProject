using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramerateCounter : MonoBehaviour
{
    [Tooltip("Delay between updates of the displayed framerate value显示的帧率值更新之间的延迟")]
    public float pollingTime = 0.5f;
    [Tooltip("The text field displaying the framerate显示帧率的文本字段")]
    public TextMeshProUGUI uiText;

    public GameObject HUD1;
    public GameObject HUD2;

    float m_AccumulatedDeltaTime = 0f;//累积时间
    int m_AccumulatedFrameCount = 0;

    void Start()
    {

    }
    void Update()
    {
        m_AccumulatedDeltaTime += Time.deltaTime;
        m_AccumulatedFrameCount++;

        if (m_AccumulatedDeltaTime >= pollingTime)//累积时间超过0.5秒后刷新一次帧率数据
        {
            int framerate = Mathf.RoundToInt((float)m_AccumulatedFrameCount / m_AccumulatedDeltaTime);
            uiText.text = "fps "+framerate.ToString();

            m_AccumulatedDeltaTime = 0f;
            m_AccumulatedFrameCount = 0;
        }
    }
}
