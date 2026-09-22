using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public class AssualtRifle : Firearms
    {
        private CharacterManager CharacterMouseLook;
        private IEnumerator ReloadAmmoCheckerCoroutine;

        protected override void Awake()
        {
            base.Awake();
            CharacterMouseLook = FindObjectOfType<CharacterManager>();

            ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
        }

        protected override void Shooting()
        {
            if (GunTriggerGuardChoose == GunTriggerGuard.CantShoot) return;

            if (CurrentAmmo <= 0) return;//当前子弹数小于等于零时返回不执行
            if (!IsAllowShooting()) return;

            CreateBullet();

            MuzzleParticle.Play();
            CasingPraticle.Play();

            GunAnimator.Play(stateName: "Fire", IsAiming ? 1 : 0, normalizedTime: 0);

            FirearmsShootingAudioSource.clip = GunFirearmsAudioData.ShootingAudio;
            FirearmsShootingAudioSource.Play();

            LastFireTime = Time.time;//上次开枪时间

            CharacterMouseLook.FringForTest();

            CurrentAmmo -= 1;

        }

        protected override void Reload()
        {
            GunAnimator.SetLayerWeight(layerIndex:2, weight:1);//设置动画管理器中装弹动画的优先级为1
            GunAnimator.SetTrigger(CurrentAmmo>0?"ReloadLeft":"ReloadOutOf");
            //大于0播放左边换弹无拉枪栓声音，不大于零播放右边换弹有拉枪栓声音
            FirearmsReloadAudioSource.clip = CurrentAmmo > 0 ? GunFirearmsAudioData.ReloadLeft : GunFirearmsAudioData.ReloadOutOf;
            FirearmsReloadAudioSource.Play();

            if (ReloadAmmoCheckerCoroutine == null)
            {
                ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(ReloadAmmoCheckerCoroutine);
            }
            else
            {
                ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StopCoroutine(ReloadAmmoCheckerCoroutine);
                ReloadAmmoCheckerCoroutine = null;
                ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(ReloadAmmoCheckerCoroutine);
            }
        }

        protected void CreateBullet()//创建子弹预制体
        {
            GameObject tmp_Bullet = _Instance.GetObj("BulletPrefab");       
            tmp_Bullet.transform.position = MuzzlePoint.position;
            tmp_Bullet.transform.rotation = MuzzlePoint.rotation;

            //GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);//在枪焰特效播放的位置生成子弹预制体
            //tmp_Bullet.AddComponent<Rigidbody>();//给子弹添加重力组件    

            Bullet tmp_BulletScript = tmp_Bullet.GetComponent<Bullet>();
            tmp_BulletScript.ImpactPrefab = BulletImpactPrefab;
            tmp_BulletScript.ImpactAudioData = ImpactAudioData;
            tmp_BulletScript.BulletSpeed = 500f;//设置步枪子弹速度
            tmp_BulletScript.Damage = 10f;//设置步枪子弹伤害
            tmp_BulletScript.damageSource = CharacterMouseLook.gameObject;
            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();//赋予子弹散射角度
            

        }


    }

}

/*
用Var类型预先不用知道变量的类型；根据你给变量赋值来判定变量属于什么类型；

如var a =1； 则a是整型，var a = “qwer”；则a是字符型，但使用Var类型要注意：

1：必须在定义时初始化，即不能先定义后初始化，如：var a；a = 1；这样是不允许的

2：一旦初始化完成，不能再给变量赋与初始化不同的变量

3：var类型的变量必须是局部变量
*/
