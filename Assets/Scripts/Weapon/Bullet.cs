using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Scripts.ShootGame;

namespace Scripts.Weapon
{

    public class Bullet: MonoBehaviour
    {
        public float BulletSpeed;
        public float Damage;

        public GameObject damageSource;

        public GameObject ImpactPrefab;
        public ImpactAudioData ImpactAudioData;
        //private Rigidbody BulletRigidbody;
        private Transform BulletTransform;
        private Vector3 prevPosition;
        private GameObject BulletThis;
        private TrailRenderer TrailRendererThis;

        private void Start()
        {
            //BulletRigidbody = GetComponent<Rigidbody>();
            TrailRendererThis = GetComponent<TrailRenderer>();
            BulletTransform = transform;
            prevPosition = BulletTransform.position;
            BulletThis = gameObject;
        }

        private void OnEnable()
        {
            StartCoroutine(DelayDisable(4f));
        }
        IEnumerator DelayDisable(float time)       
        {
            yield return new WaitForSeconds(time);
            TrailRendererThis.Clear();//清除子弹拖尾特效再回收子弹
            Firearms._Instance.RecycleObj(BulletThis);
        }
        private void Update()
        {
            prevPosition = BulletTransform.position;
            BulletTransform.Translate(x: 0, y: 0, z: BulletSpeed * Time.deltaTime);         
            //BulletRigidbody.velocity = BulletSpeed * Time.fixedTime * BulletTransform.forward;
            if (
                    !Physics.Raycast
                    (
                        origin: prevPosition,
                        direction: (BulletTransform.position - prevPosition).normalized,//单位化向量
                        out RaycastHit tmp_Hit,
                        maxDistance: (BulletTransform.position - prevPosition).magnitude//向量长度
                    )
                )
            return;

            Vector3 FirePoint = prevPosition;//射线发射的原点位置
            Vector3 HitPoint = tmp_Hit.point;//射线击中物体的位置
            Vector3 IncomeVector = HitPoint - FirePoint;//入射向量（射线角度）= 被击中位置 - 发射原点
            IncomeVector = IncomeVector.normalized;//将入射向量变为单位化向量

            //尝试获取到目标物体的Rigidbody组件
            bool TargetRigidbody = tmp_Hit.collider.TryGetComponent(out Rigidbody TargetGameObjectRigidbody);
            if (TargetRigidbody)
            {                
                TargetGameObjectRigidbody.AddForceAtPosition(IncomeVector * 2000f * Time.deltaTime, HitPoint);
                //根据入射向量生成物理力             
            }

            bool targetCollider = tmp_Hit.collider.TryGetComponent(out TargetCollider Collider);
            if (targetCollider)
            {
                Collider.beenHit = true;
            }

            
            //尝试获取到目标物体的Health脚本
            bool TargetDamageable = tmp_Hit.collider.TryGetComponent(out Damageable TargetGameObjectDamageable);
            if (TargetDamageable)
            {
                TargetGameObjectDamageable.InflictDamage(Damage, false, damageSource);
            }

            bool TargetExplosive = tmp_Hit.collider.TryGetComponent(out ExplosiveBarrelScript TargetExplosiveBarrelScript);
            if (TargetExplosive)
            {
                TargetExplosiveBarrelScript.explode = true;
            }

            //Debug.DrawRay(start:BulletTransform.position, dir:BulletTransform.forward, Color.red, duration:0.1f);
            //Debug.Log(tmp_hit.collider.name);

            var tmp_BulletEffect = Instantiate
            (
                ImpactPrefab, 
                tmp_Hit.point, 
                Quaternion.LookRotation(forward:tmp_Hit.normal, upwards: Vector3.up)
            );

            Destroy(tmp_BulletEffect, 3f);

            //子弹碰撞物体音效
            var tmp_TagsWithAudios = ImpactAudioData.ImpactTagsWithAudios.Find
                (match: (tmp_AudioData) => { return tmp_AudioData.Tag.Equals(tmp_Hit.collider.tag); });
            if (tmp_TagsWithAudios == null) return;
            int tmp_Length = tmp_TagsWithAudios.ImpactAudioClips.Count;
            AudioClip tmp_AudioClip = tmp_TagsWithAudios.ImpactAudioClips[Random.Range(0, tmp_Length)];
            AudioSource.PlayClipAtPoint(tmp_AudioClip, tmp_Hit.point);//在碰撞位置播放碰撞音效


            if (tmp_Hit.collider.CompareTag(tmp_Hit.collider.tag))
            {               
                if(tmp_Hit.collider.tag == "Metal" && tmp_Hit.collider.transform.localScale.x > 0.5)
                {
                    Firearms._Instance.RecycleObj(BulletThis);
                    //Destroy(BulletThis, 5f);
                }

                if (tmp_Hit.collider.tag == "Wood" && tmp_Hit.collider.transform.localScale.x > 1)
                {
                    Firearms._Instance.RecycleObj(BulletThis);
                    //Destroy(BulletThis, 5f);
                }
            }

        }

        private void OnDrawGizmos()
        {
            //Gizmos.DrawCube(center:transform.position, size: new Vector3(x:0.1f, y:0.1f, z:0.25F));
        }

    }

}
