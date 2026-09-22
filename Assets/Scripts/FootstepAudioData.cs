using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Footstep Audio Data")]
public class FootstepAudioData : ScriptableObject
{
    public List<FootstepAudio> FootstepAudios = new List<FootstepAudio>();//声明音效类List集合
}


[System.Serializable]
public class FootstepAudio
{
    public string Tag;//音效标签
    public List<AudioClip> AudioClips = new List<AudioClip>();//声明音效List集合
    public float Delay;//播放延迟
    public float SprintingDelay;//冲刺延迟
    public float CrouchingDelay;//下蹲延迟
}






