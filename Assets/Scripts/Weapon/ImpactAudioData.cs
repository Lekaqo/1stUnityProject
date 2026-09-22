using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Impact Audio Data")]//在创建物体菜单提供此选项
    public class ImpactAudioData:ScriptableObject
    {
        public List<ImpactTagsWithAudio> ImpactTagsWithAudios;
    }

    [System.Serializable]
    public class ImpactTagsWithAudio
    {
        public string Tag;
        public List<AudioClip> ImpactAudioClips;

    }

}
