using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.ShootGame
{
    public class TargetCollider : MonoBehaviour
    {
        public Animation TargetAnimation;
        public bool beenHit = false;
        public bool isHit = false;

        public AudioSource TargetAudio;
        public AudioClip HitSound;
        public AudioClip ResetSound;

        public Mat MatScript;
        public void Start()
        {
            MatScript = FindObjectOfType<Mat>();
            TargetAnimation = transform.parent.transform.parent.GetComponent<Animation>();
        }
        public void Update()
        {
            if (MatScript.mat == false)
            {
                beenHit = false;
            }
            if (isHit == false && beenHit == true && MatScript.mat == true)
            {
                StartCoroutine(Hit());
            }
        }
        // Update is called once per frame
        IEnumerator Hit()
        {
            isHit = true;
            TargetAudio.clip = HitSound;
            TargetAudio.Play();
            TargetAnimation.Play("down");
            MatScript.HitNumber++;
            beenHit = false;
            yield return new WaitForSeconds(MatScript.ResetTime);
            TargetAudio.clip = ResetSound;
            TargetAudio.Play();
            TargetAnimation.Play("up");            
            isHit = false;
            if (MatScript.HitNumber > 0)
            {
                MatScript.HitNumber--;
            }
        }
    }
}