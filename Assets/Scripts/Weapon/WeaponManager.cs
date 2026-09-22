using Assets.Scripts.Item;
using Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Scripts.Weapon.Firearms;

public class WeaponManager : MonoBehaviour
{
    public CharacterManager CharacterManagerScript;

    public Firearms MainWeapon;
    public Firearms SecondaryWeapon;
    public Text AmmoCountTextLabel;

    public AudioSource TakeOutWeaponAudioSource;//取出武器音效组件
    [SerializeField] protected FirearmsAudioData CurrentFirearmsAudioData;

    private Firearms currentCarriedWeapon;
    private IEnumerator waitingForHolsterEndCoroutine;
    private IEnumerator waitingForTakeOutEndCoroutine;

    public List<Firearms> Arms = new List<Firearms>();

    public Transform WorldCameraTransform;
    public float RaycastMaxDistance = 2;
    public LayerMask CheckLayerMask;//碰撞的层次

    public Text daba;

    [SerializeField] protected bool IsHolster;
    [SerializeField] protected bool IsTakeOut;

    private GameObject RaycastHitGameObject;
    private GameObject CrossHair;
    private GameManager GameManagerScript;

    public AudioSource gameManagerAudioSource;
    private void Start()
    {
        CharacterManagerScript = FindObjectOfType<CharacterManager>();
        GameManagerScript = FindObjectOfType<GameManager>();

        CrossHair = GameObject.Find("CrossHair");
        //Debug.Log(message: $"Current weapon is null? {currentCarriedWeapon == null}");
        if (MainWeapon)
        {
            MainWeapon = currentCarriedWeapon;
            CharacterManagerScript.SetupAnimator(currentCarriedWeapon.GunAnimator);

            CurrentFirearmsAudioData = currentCarriedWeapon.GunFirearmsAudioData;
            TakeOutWeaponAuido();

            currentCarriedWeapon.Aiming(false);
        }
    }

    

    public void TakeOutWeaponAuido()//播放当前拾取武器的取出武器音效
    {
        TakeOutWeaponAudioSource.clip = CurrentFirearmsAudioData.TakeOutWeapon;
        TakeOutWeaponAudioSource.Play();
    }

    private void UpdateAmmoInfo(int _ammo, int _remaningAmmo)
    {
        AmmoCountTextLabel.text = _ammo + "/" + _remaningAmmo;
    }

    private void Update()
    {
        Check();

        if (!currentCarriedWeapon) return;

        SwapWeapon();
        ControlWeapon();
        
        //AmmoCountTextLabel.enabled = true;
        UpdateAmmoInfo(currentCarriedWeapon.GetCurrentAmmo,currentCarriedWeapon.GetCurrentMaxAmmoCarried);

    }
    //Ctrl+K/D/C

    private void ControlWeapon()
    {
        ///控制射击模块
        if (Input.GetMouseButton(0))
        {
            if (IsHolster == true) return;
            if (IsTakeOut == true) return;
            
            currentCarriedWeapon.GunTrigger(true);//按下扳机
            if (currentCarriedWeapon.tag == "HandGun")//如果当前拾取武器是手枪，那么射击一次后打开保险不允许按下扳机使枪支进行连射的行为发生
            {
                currentCarriedWeapon.GunTriggerGuardChoose = GunTriggerGuard.CantShoot;
            }

            if (currentCarriedWeapon.tag == "AssualtRifle")//如果当前拾取武器是步枪，那么射击一次后打开保险不允许按下扳机使枪支进行连射的行为发生
            {
                if(currentCarriedWeapon.GunTriggerGuardChoose == GunTriggerGuard.OneShoot)//如果当前保险状态是单发
                {
                    currentCarriedWeapon.GunTriggerGuardChoose = GunTriggerGuard.CantShoot;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            currentCarriedWeapon.GunTrigger(false);//松开扳机

            if (currentCarriedWeapon.tag == "HandGun")//如果当前拾取武器是手枪，那么射击一次后松开扳机时关闭保险使枪支可以继续射击
            {
                currentCarriedWeapon.GunTriggerGuardChoose = GunTriggerGuard.Shoot;
            }

            if (currentCarriedWeapon.tag == "AssualtRifle")//如果当前拾取武器是步枪，那么射击一次后将枪支保险返回到可以单发射击的状态
            {
                if (currentCarriedWeapon.GunTriggerGuardChoose == GunTriggerGuard.CantShoot)//判断是否打开单发射击保险
                {
                    currentCarriedWeapon.GunTriggerGuardChoose = GunTriggerGuard.OneShoot;
                }
            }
        }

        ///控制换弹模块
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentCarriedWeapon.Aiming(false);
            currentCarriedWeapon.ReloadAmmo();
        }

        ///
        if (Input.GetKeyDown(KeyCode.F))
        {
            //currentCarriedWeapon.GunAnimator.SetTrigger(name: "knife");
        }

        ///瞄准模块
        if (Input.GetMouseButtonDown(1))
        {
            //TODO:瞄准
            currentCarriedWeapon.Aiming(true);//按下瞄准
            
            CrossHair.SetActive(false);
            
        }
        if (Input.GetMouseButtonUp(1))
        {
            //TODO：退出瞄准到腰射
            currentCarriedWeapon.Aiming(false);//松开瞄准
            
            CrossHair.SetActive(true);
           
        }

        ///
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (currentCarriedWeapon.tag == "AssualtRifle")
            {
                currentCarriedWeapon.GunTriggerGuardChoose = 
                    currentCarriedWeapon.IsOneShoot ? GunTriggerGuard.Shoot : GunTriggerGuard.OneShoot;
                currentCarriedWeapon.IsOneShoot = !currentCarriedWeapon.IsOneShoot;
            }
        }

    }

    private void Check()
    {
        bool tmp_IsItem = Physics.Raycast
        (
            origin: WorldCameraTransform.position,
            direction: WorldCameraTransform.forward,
            out RaycastHit tmp_RaycastHit,
            RaycastMaxDistance,
            CheckLayerMask
        );

        if (tmp_IsItem)
        {
            daba.text = "按E键交互";
            RaycastHitGameObject = tmp_RaycastHit.collider.gameObject;
            //Debug.Log(tmp_RaycastHit.collider.name);

            if (Input.GetKeyDown(KeyCode.E))
            {
                daba.text = "";
                bool tmp_HasItem = tmp_RaycastHit.collider.TryGetComponent(out BaseItem tmp_BaseItem);
                if (tmp_HasItem)
                {         
                    PickupWeapon(tmp_BaseItem);
                    PickupAttachment(tmp_BaseItem);              
                }

                bool tmp_DoorManagerScript = tmp_RaycastHit.collider.TryGetComponent(out DoorManager DoorManagerScript);
                if (tmp_DoorManagerScript)
                {
                    DoorManagerScript.DoorOpenApply = true;
                }

                if (tmp_RaycastHit.collider.tag == "Power")
                {
                    Destroy(RaycastHitGameObject);
                    GameManagerScript.Powers = GameManagerScript.Powers + 1;
                    gameManagerAudioSource.clip = GameManagerScript.PowerPick;
                    gameManagerAudioSource.Play();
                }

                if (tmp_RaycastHit.collider.tag == "ExplosivePick")
                {
                    Destroy(RaycastHitGameObject);
                    GameManagerScript.GameWin = true;

                }
            }
        }
        else
        {
            daba.text = "";
        }

    }

    private void PickupWeapon(BaseItem _baseItem)
    {
        if (!(_baseItem is FirearmsItem tmp_FirearmsItem)) return;
        foreach (Firearms tmp_Arm in Arms)
        {
            if (tmp_FirearmsItem.ArmsName.CompareTo(tmp_Arm.name) != 0) continue;
            switch (tmp_FirearmsItem.CurrentFirearmsType)
            {
                case FirearmsItem.FirearmsType.AssultRefile:
                    MainWeapon = tmp_Arm;
                    break;
                case FirearmsItem.FirearmsType.HandGun:
                    SecondaryWeapon = tmp_Arm;
                    break;
            }
            SetupCarriedWeapon(tmp_Arm);
            Destroy(RaycastHitGameObject);
        }
    }

    private void PickupAttachment(BaseItem _baseItem)
    {
        if (!(_baseItem is AttachmentItem tmp_AttachmentItem)) return;

        switch (tmp_AttachmentItem.CurrentAttachmentType)
        {
            case AttachmentItem.AttachmentType.Scope:
                foreach (ScopeInfo tmp_ScopeInfo in currentCarriedWeapon.ScopeInfos)
                {
                    if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.ItemName) != 0)
                    {
                        tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                        continue;
                    }

                    tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                    currentCarriedWeapon.BaseIronSight.ScopeGameObject.SetActive(false);
                    currentCarriedWeapon.SetupCarriedScope(tmp_ScopeInfo);
                    
                }

                break;
            case AttachmentItem.AttachmentType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (MainWeapon == null) return;
            if (currentCarriedWeapon == MainWeapon) return;//如果当前拾取武器是主武器返回不执行
            if (currentCarriedWeapon.gameObject.activeInHierarchy)
            {
                IsHolster = true;
                currentCarriedWeapon.CanAim = false;
                currentCarriedWeapon.GunAnimator.SetTrigger(name: "Holster");//触发收枪动画
                StartWaitingForHolsterEndCoroutine();//执行检查换枪动画是否结束协同程序
                
            }
            else
            {
                SetupCarriedWeapon(MainWeapon);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (SecondaryWeapon == null) return;
            if (currentCarriedWeapon == SecondaryWeapon) return;//如果当前拾取武器是主武器
            if (currentCarriedWeapon.gameObject.activeInHierarchy)
            {
                IsHolster = true;
                StartWaitingForHolsterEndCoroutine();
                currentCarriedWeapon.GunAnimator.SetTrigger(name: "Holster");
            }
            else
            {
                SetupCarriedWeapon(SecondaryWeapon);
            }
        }      
    }

    private void StartWaitingForHolsterEndCoroutine()
    {
        if (waitingForHolsterEndCoroutine == null)
            waitingForHolsterEndCoroutine = WaitingForHolsterEnd();
        StartCoroutine(waitingForHolsterEndCoroutine);//不为空直接执行
    }

    private IEnumerator WaitingForHolsterEnd()//检查收枪动画是否结束
    {
        while(true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo =
                currentCarriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(layerIndex:0);

            if (tmp_AnimatorStateInfo.IsTag("holster"))
            {
                
                if (tmp_AnimatorStateInfo.normalizedTime>= 0.9f)//当收枪动画执行完毕
                {
                    IsHolster = false;
                    currentCarriedWeapon.CanAim = true;
                    var tmp_TargetWeapon= 
                        currentCarriedWeapon == MainWeapon ? SecondaryWeapon : MainWeapon;
                    SetupCarriedWeapon(tmp_TargetWeapon);
                    waitingForHolsterEndCoroutine = null;
                    
                    yield break;
                }

            }

            yield return null;
        }
       
    }

    private void StartWaitingForTakeOutEndCoroutine()
    {
        if (waitingForTakeOutEndCoroutine == null)
            waitingForTakeOutEndCoroutine = WaitingForTakeOutEnd();
        StartCoroutine(waitingForTakeOutEndCoroutine);//不为空直接执行
    }

    private IEnumerator WaitingForTakeOutEnd()//检查取枪动画是否结束
    {
        while (true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo =
                currentCarriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(layerIndex: 0);

            if (tmp_AnimatorStateInfo.IsTag("TakeOut"))
            {

                if (tmp_AnimatorStateInfo.normalizedTime >= 0.9f)//当取枪动画执行完毕
                {
                    IsTakeOut = false;
                    currentCarriedWeapon.CanAim = true;

                    waitingForTakeOutEndCoroutine = null;

                    yield break;
                }
                else
                {
                    IsTakeOut = true;
                    currentCarriedWeapon.CanAim = false;
                }

            }

            yield return null;
        }

    }

    private void SetupCarriedWeapon(Firearms _targetWeapon)
    {
        if(currentCarriedWeapon)//如果当前拾取武器不为空
        currentCarriedWeapon.gameObject.SetActive(false);
        currentCarriedWeapon = _targetWeapon;
        currentCarriedWeapon.gameObject.SetActive(true);

        StartWaitingForTakeOutEndCoroutine();//执行等待取枪动画完成协同程序
        CharacterManagerScript.SetupAnimator(currentCarriedWeapon.GunAnimator);

        CurrentFirearmsAudioData = currentCarriedWeapon.GunFirearmsAudioData;
        TakeOutWeaponAuido();

        currentCarriedWeapon.Aiming(false);
    }


}
