using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepListener : MonoBehaviour
{
    public FootstepAudioData FootstepAudioData;
    public AudioSource FootstepAudioSource;

    private CharacterController characterController;
    private Transform footstepTransform;

    private float nextPlayTime;
    public LayerMask LayerMask;//用来隔绝Linecast方法
    
    public enum State//做一个开关
    {
        Idle,
        Walk,
        Sprinting,
        Crouching,
        Others
    }
    
    public State characterState;

    //Q:角色发出声音的必备条件
    //A:角色移动或者发生较大幅度动作的时候发出声音


    //Q.如何检测角色是否有移动
    //A:用Physic API检测


    //Q:如何实现角色踩踏位置的对应材质的声音
    //A:用Physic API检测


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepTransform = transform;
    }

    private void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
            
            if (characterController.velocity.normalized.magnitude >= 0.1f)
            {
                if (characterController.velocity.magnitude > 1)
                {
                    nextPlayTime += Time.deltaTime;
                }

                if (characterController.velocity.magnitude >= 4)
                {
                    characterState = State.Sprinting;
                }
                else if(characterController.velocity.magnitude < 4)
                {
                    characterState = State.Walk;
                }
                

                bool tmp_IsHit = Physics.Linecast
                (
                    footstepTransform.position,
                    footstepTransform.position + Vector3.down * 
                    (characterController.height / 2 + characterController.skinWidth - characterController.center.y),
                    out RaycastHit tmp_HitInfo, LayerMask
                );

                /*
                Debug.DrawLine(footstepTransform.position,footstepTransform.position +
                Vector3.down * (characterController.height / 2 + characterController.skinWidth - characterController.center.y),
                Color.red,0.25f);
                */

                if (tmp_IsHit)
                {
                    //TODO:检测类型
                    foreach (var tmp_AudioElement in FootstepAudioData.FootstepAudios)
                    {
                        if (tmp_HitInfo.collider.CompareTag(tmp_AudioElement.Tag))//比较角色控制器触碰到的物体标签
                        {                                                       
                            float tmp_Delay = 0;
                            switch(characterState)
                            {
                                case State.Idle:
                                    tmp_Delay = float.MaxValue;
                                    break;
                                case State.Walk:
                                    tmp_Delay = tmp_AudioElement.Delay;
                                    break;
                                case State.Sprinting:
                                    tmp_Delay = tmp_AudioElement.SprintingDelay;
                                    break;
                                case State.Crouching:
                                    tmp_Delay = tmp_AudioElement.CrouchingDelay;
                                    break;
                                case State.Others:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            if (nextPlayTime >= tmp_Delay)
                            {
                                //TODO:播放移动声音
                                int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                int tmp_AudioIndex = UnityEngine.Random.Range(0, tmp_AudioCount);
                                AudioClip tmp_FootstepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                FootstepAudioSource.clip = tmp_FootstepAudioClip;
                                FootstepAudioSource.Play();
                                nextPlayTime = 0;
                                break;
                            }
                        }
                    }
                }

            }
            else
            {
                characterState = State.Idle;
                nextPlayTime = 0;
                FootstepAudioSource.Stop();
            }



        }
    }
}
