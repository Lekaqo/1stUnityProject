using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scripts.Weapon
{
    public abstract class Firearms: MonoBehaviour, IWeapon
    {

        public Transform MuzzlePoint;
        public Transform CasingPoint;

        public ParticleSystem MuzzleParticle;
        public ParticleSystem CasingPraticle;

        public AudioSource FirearmsShootingAudioSource;//射击音效组件
        public AudioSource FirearmsReloadAudioSource;//换弹音效组件

        public FirearmsAudioData GunFirearmsAudioData;
        public ImpactAudioData ImpactAudioData;

        public Camera EyeCamera;
        public Camera GunCamera;

        public float SpreadAngle;//子弹散布角度

        public int AmmoInMag;//设置弹夹子弹数
        public int MaxAmmoCarried;
        public float FireRate;//一份钟射击次数
        public GameObject BulletPrefab;
        public GameObject BulletImpactPrefab;

        public enum GunTriggerGuard//做一个开关
        {
            OneShoot,
            Shoot,
            CantShoot
        }

        [SerializeField] internal GunTriggerGuard GunTriggerGuardChoose;

        [SerializeField] internal bool IsOneShoot;
        [SerializeField] internal bool CanAim;
        
        public int GetCurrentAmmo => CurrentAmmo;
        public int GetCurrentMaxAmmoCarried => CurrentMaxAmmoCarried;

        [SerializeField] internal Animator GunAnimator;

        public List<ScopeInfo> ScopeInfos;
        public ScopeInfo BaseIronSight;
        protected ScopeInfo rigoutScopeInfo;

        public AnimatorStateInfo GunStateInfo;

        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;
        protected float LastFireTime;

        protected float OriginFOV;
        protected float EyeOriginFOV;
        protected float GunOriginFOV;
        private Vector3 originalEyePosition;
        protected Transform gunCameraTransform;

        protected bool IsAiming;//是否瞄准

        private IEnumerator DoAimCoroutine;

        public static Firearms _Instance;//实例化脚本
        private Dictionary<string, List<GameObject>> pool;// 对象池
        private Dictionary<string, GameObject> prefabs;// 预设体

        protected virtual void Awake()
        {
            _Instance = this;

            pool = new Dictionary<string, List<GameObject>>();
            prefabs = new Dictionary<string, GameObject>();

            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();

            EyeOriginFOV = EyeCamera.fieldOfView;
            GunOriginFOV = GunCamera.fieldOfView;

            gunCameraTransform = GunCamera.transform;
            originalEyePosition = gunCameraTransform.localPosition;

            DoAimCoroutine = DoAim();
            CanAim = true;
            GunTriggerGuardChoose = GunTriggerGuard.Shoot;//初始关闭枪械扳机保险允许射击

            rigoutScopeInfo = BaseIronSight;

        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        public GameObject GetObj(string objName)
        {
            //结果对象
            GameObject result = null;
            //判断是否有该名字的对象池
            if (pool.ContainsKey(objName))
            {           
                if (pool[objName].Count > 0)//对象池里有对象
                {                   
                    result = pool[objName][0];//获取结果                
                    result.SetActive(true);//激活对象                   
                    pool[objName].Remove(result);//从池中移除该对象                   
                    return result;//返回结果
                }
            }
            //如果没有该名字的对象池或者该名字对象池没有对象
            GameObject prefab = null;
            //如果已经加载过该预设体
            if (prefabs.ContainsKey(objName))
            {
                prefab = prefabs[objName];
            }
            else     //如果没有加载过该预设体
            {                
                prefab = Resources.Load<GameObject>("Prefabs/" + objName);//加载预设体               
                prefabs.Add(objName, prefab);//更新字典
            }
            result = Instantiate(prefab); //生成         
            result.name = objName;//改名（去除 Clone）
            return result;//结果
        }

        /// <summary>
        /// 回收对象到对象池
        /// </summary>
        public void RecycleObj(GameObject obj)
        {
            //设置为非激活
            obj.SetActive(false);
            //判断是否有该对象的对象池

            if (pool.ContainsKey(obj.name))
            {
                //放置到该对象池
                pool[obj.name].Add(obj);
            }
            else
            {
                //创建该类型的池子，并将对象放入
                pool.Add(obj.name, new List<GameObject>(){obj});
            }

        }
        
        public void DoAttack()//运行枪械射击代码
        {            
            Shooting();      
        }
        protected abstract void Shooting();
        protected abstract void Reload();

        internal void Aiming(bool _isAming)//枪械瞄准功能模块
        {
            if (CanAim == false) return;
            IsAiming = _isAming;
            GunAnimator.SetBool(name: "Aim", IsAiming);
            if (DoAimCoroutine == null)
            {
                DoAimCoroutine = DoAim();
                StartCoroutine(DoAimCoroutine);
            }
            else
            {
                StopCoroutine(DoAimCoroutine);
                DoAimCoroutine = null;
                DoAimCoroutine = DoAim();
                StartCoroutine(DoAimCoroutine);
            }
        }
        internal void GunTrigger(bool IsHoldingTrigger)//枪械扳机功能模块
        {
            
            if (!IsHoldingTrigger) return;//如果值为false, 返回不执行

            DoAttack();
                     
        }
        internal void ReloadAmmo()//枪械装弹功能模块
        {
            GunTriggerGuardChoose = GunTriggerGuard.CantShoot;//换弹开始开启枪械扳机保险不允许射击
            Reload();
        }
        internal void gunTriggerGuards()//枪械扳机保险功能模块
        {

            switch (GunTriggerGuardChoose)
            {
                case GunTriggerGuard.OneShoot:
                 
                    break;
                case GunTriggerGuard.Shoot:
               
                    break;
                case GunTriggerGuard.CantShoot:
              
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal bool IsAllowShooting()//射击间隔
        {
            //AK47的射速
            //715 1m
            //1m= 60s
            //715/60s = 11.7
            //1s/11.7//一秒11.7发
            return Time.time - LastFireTime > 1 / FireRate;
        }

        protected Vector3 CalculateSpreadOffset()//计算子弹散射角度功能模块
        {
            float tmp_SpreadPercent = SpreadAngle / EyeCamera.fieldOfView;//计算散射角度

            return tmp_SpreadPercent * UnityEngine.Random.insideUnitCircle;
        }

        protected IEnumerator DoAim()//瞄准功能实现模块
        {           
            while (true)
            {
                yield return null;

                float tmp_CurrentFOV = 0;
                EyeCamera.fieldOfView =
                    Mathf.SmoothDamp(
                    current: EyeCamera.fieldOfView,
                    target: IsAiming ? rigoutScopeInfo.EyeFov : EyeOriginFOV,
                    currentVelocity: ref tmp_CurrentFOV,
                    smoothTime: Time.deltaTime * 2);

                float tmp_GunCurrentFOV = 0;
                GunCamera.fieldOfView =
                    Mathf.SmoothDamp(GunCamera.fieldOfView,
                        IsAiming ? rigoutScopeInfo.GunFov : GunOriginFOV,
                        ref tmp_GunCurrentFOV,
                        Time.deltaTime * 2);

                Vector3 tmp_RefPosition = Vector3.zero;
                gunCameraTransform.localPosition = Vector3.SmoothDamp(gunCameraTransform.localPosition,
                    IsAiming ? rigoutScopeInfo.GunCameraPosition : originalEyePosition,
                    ref tmp_RefPosition,
                    Time.deltaTime * 2);
            }
        }

        protected IEnumerator CheckReloadAmmoAnimationEnd()//检查装弹动画播放是否结束功能模块
        {

            while (true)
            {
                yield return null;
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(layerIndex: 2);

                if (GunStateInfo.IsTag("ReloadAmmo"))
                {
                    CanAim = false;
                    GunTriggerGuardChoose = GunTriggerGuard.CantShoot;//换弹开始，开启枪械扳机保险不允许射击
                    if (GunStateInfo.normalizedTime > 0.9f)
                    {
                        int tmp_NeedAmmoCount = AmmoInMag - CurrentAmmo;
                        int tmp_RemainingAmmo = CurrentMaxAmmoCarried - tmp_NeedAmmoCount;
                        if (tmp_RemainingAmmo <= 0)
                        {
                            CurrentAmmo += CurrentMaxAmmoCarried;
                        }
                        else
                        {
                            CurrentAmmo = AmmoInMag;
                        }
                        CurrentMaxAmmoCarried = tmp_RemainingAmmo <= 0 ? 0 : tmp_RemainingAmmo;
     
                        if (gameObject.tag == "AssualtRifle")
                        {
                            GunTriggerGuardChoose = IsOneShoot ? GunTriggerGuard.OneShoot : GunTriggerGuard.Shoot;
                        }
                        else
                        {
                            GunTriggerGuardChoose = GunTriggerGuard.Shoot;
                        }

                        CanAim = true;
                        yield break;
                    }

                }

            }

        }

        internal void SetupCarriedScope(ScopeInfo _scopeInfo)
        {
            rigoutScopeInfo = _scopeInfo;
        }

        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName;
            public GameObject ScopeGameObject;
            public float EyeFov;
            public float GunFov;
            public Vector3 GunCameraPosition;
        }

    }





}