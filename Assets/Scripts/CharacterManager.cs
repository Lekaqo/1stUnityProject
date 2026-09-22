using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed;//行走速度
    public float SprintingSpeed;//冲刺速度
    public float WalkSpeedWhenCrouched;
    public float SprintingSpeedWhenCrouched;
    public float CrouchHeight = 1f;
    public float Gravity;//重力变量
    private float originHeight;//玩家预制体原始高度

    [Header("Jump")]
    public float jumpHeight;//跳跃高度

    [Header("Mouse")]
    [Tooltip("鼠标速度")]
    public float MouseSensitivity;//鼠标速度
    [Tooltip("-x限制向上看的角度，y限制向下看的角度")]
    public Vector2 MaxminAngle;

    [SerializeField] protected bool isCrouched;
    [SerializeField] protected bool isJumpded;

    public Animator characterAnimator;

    [Header("Recoil")]
    [Tooltip("枪械后坐力参数")]
    public AnimationCurve RecoilCurve;//后坐曲线
    public Vector2 RecoilRange;//反冲范围
    public float RecoilFadeOutTime;//后坐力淡出时间

    private float currentRecoilTime;
    private Vector2 currentRecoil;
    private CameraSpring cameraSpring;

    private Transform cameraTransform;
    private Vector3 cameraRotation;
    private Transform characterTransform;
    private CharacterController characterController;
    private Vector3 moveDirection;

    private float tmp_MouseX = 0;
    private float tmp_MouseY = 0;

    private float velocity;//速度
    //private IEnumerator DoCrouchCoroutine;

    [Header("Fall Damage")]
    [Tooltip("玩家从地图上掉下时瞬间死亡的高度Height at which the player dies instantly when falling off the map")]
    public float killHeight = -50f;
    [Tooltip("跌落伤害Whether the player will recieve damage when hitting the ground at high speed")]
    public bool recievesFallDamage;
    [Tooltip("Minimun fall speed for recieving fall damage")]
    public float minSpeedForFallDamage = 10f;
    [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
    public float maxSpeedForFallDamage = 30f;
    [Tooltip("Damage recieved when falling at the mimimum speed")]
    public float fallDamageAtMinSpeed = 10f;
    [Tooltip("Damage recieved when falling at the maximum speed")]
    public float fallDamageAtMaxSpeed = 50f;

    public bool isDead { get; private set; }
    public Vector3 characterVelocity { get; set; }
    public Vector3 m_LatestImpactSpeed;

    Health m_Health;

    public int AttitudeUI;

    public bool MouseLock;
    public bool Fly;
    private void Start()
    {
        characterTransform = transform;

        //cameraTransform = transform.Find("Assault_Rifle_01_Arms");

        characterController = GetComponent<CharacterController>();

        originHeight = characterController.height;

        cameraSpring = GetComponentInChildren<CameraSpring>();

        m_Health = GetComponent<Health>();

        //Cursor.lockState = CursorLockMode.Locked;//锁定鼠标
        //Cursor.visible = false;

        MouseLock = false;
        Fly = false;


    }

    private void CalculateRecoilOffset()//计算枪械后坐力
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFraction = currentRecoilTime / RecoilFadeOutTime;
        float tmp_RecoilValue = RecoilCurve.Evaluate(tmp_RecoilFraction);        
        currentRecoil = Vector2.Lerp(a:Vector2.zero, b:currentRecoil, tmp_RecoilValue);
    }


    public void FringForTest()//震屏
    {
        currentRecoil += RecoilRange;

        cameraSpring.StartCameraSpring();

        currentRecoilTime = 0;
        
    }

    private void Update()
    {
        FPSmouseLook();
        MouseController();
    }

    private void FixedUpdate()
    {
        FPSmoveMent();

        if (transform.position.y < killHeight)//玩家transform的y值小于设定的瞬时死亡高度时，执行死亡程序
        {
            m_Health.Kill();
        }
    }

    /// <summary>
    /// 控制鼠标锁定与释放
    /// </summary>
    private void MouseController()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MouseLock = !MouseLock;
        }

        if (MouseLock == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    private void FPSmouseLook()
    {
        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            tmp_MouseX = Input.GetAxis("Mouse X");
            tmp_MouseY = Input.GetAxis("Mouse Y");

            cameraRotation.x -= tmp_MouseY * MouseSensitivity;
            cameraRotation.y += tmp_MouseX * MouseSensitivity;

            CalculateRecoilOffset();//计算后坐力
            //Debug.Log(currentRecoil);
            cameraRotation.x -= currentRecoil.y;//震动屏幕
            cameraRotation.y += currentRecoil.x;//震动屏幕

            cameraRotation.x = Mathf.Clamp(value: cameraRotation.x, min: MaxminAngle.x, max: MaxminAngle.y);//限制相机旋转俯仰角
            characterTransform.rotation = Quaternion.Euler(x: cameraRotation.x, y: cameraRotation.y, z: 0f);//角色控制器的欧拉角等于摄相机的欧拉角四元数
        }
    }


    private void FPSmoveMent()
    {
        ///***********局部变量**********///
        float tmp_Hrizontal = Input.GetAxis("Horizontal");
        float tmp_Vetyical = Input.GetAxis("Vertical");

        float tmp_CurrentSpeed = WalkSpeed;//当前速度等于行走速度

        //获取到角色控制器移动时的速度数据赋值给动画管理器的变量，以此判断动画播放选择
        //Debug.Log(characterController.velocity.magnitude);
        if (characterAnimator)
        {
            velocity = 0;
            var tmp_Velocity = characterController.velocity;
            tmp_Velocity.y = 0;

            velocity = isJumpded ? 0 : tmp_Velocity.magnitude;//如果跳跃为true,向动画控制器的"Velocity"传递0值以返回待机状态动画
            characterAnimator.SetFloat(name: "Velocity", value: velocity, dampTime: 0.25f, deltaTime: Time.deltaTime);
        }
        ///*************************判断角色是否在地面上**************************///
        if (characterController.isGrounded)
        {
            isJumpded = false;//当角色落到地面时跳跃bool返回false值

            fallDamage();

            moveDirection = characterTransform.TransformDirection(new Vector3(x: tmp_Hrizontal, y: 0, z: tmp_Vetyical)).normalized;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouched = !isCrouched;

                var tmp_CurrentHeight = isCrouched ? CrouchHeight : originHeight;

                if (isCrouched == true)
                {
                    AttitudeUI = 1;
                    StartCoroutine(DoCrouch(tmp_CurrentHeight));
                }
                else
                {
                    AttitudeUI = 0;
                    StartCoroutine(DoCrouch(tmp_CurrentHeight));
                }
            }

            if (isCrouched == false)
            {
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;
                //判断是否按下Shift键，是则为SprintingSpeed, 否则为WalkSpeed
            }
            else if (isCrouched == true)
            {
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
            }


            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = CalculateJumpHeightSpeed();//用物理公式计算向上跳跃的高度
                isJumpded = true;
            }           
        }

        if (Input.GetKey(KeyCode.F))//F键盘开关重力
        {
            Fly = !Fly;
        }

        if (Fly == false)
        {
            moveDirection.y -= Gravity * Time.deltaTime;//模拟重力
        }
        else
        {
            moveDirection = characterTransform.TransformDirection(new Vector3(x: tmp_Hrizontal, y: 0, z: tmp_Vetyical)).normalized;
            if (Input.GetKey(KeyCode.Q))
            {
                moveDirection.y = 2 * Gravity * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.E))
            {
                moveDirection.y -= 2 * Gravity * Time.deltaTime;
            }
        }

        characterController.Move(motion: tmp_CurrentSpeed * Time.deltaTime * moveDirection);
    }

    private IEnumerator DoCrouch(float _targht)
    {
        float tmp_CurrentHeight = 0;
        while (Mathf.Abs(f: characterController.height - _targht) > 0.1f)
        {
            yield return null;
            characterController.height = Mathf.SmoothDamp
            (current: characterController.height, target: _targht, currentVelocity: ref tmp_CurrentHeight, smoothTime: Time.deltaTime * 5);
        }  
    }

    /// <summary>
    /// 用公式计算跳跃
    /// </summary>
    /// <returns></returns>
    private float CalculateJumpHeightSpeed()
    {
        return Mathf.Sqrt(f:2 * Gravity * jumpHeight);
    }

    internal void SetupAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }

    void fallDamage()
    {
        float fallSpeed = -Mathf.Min(characterVelocity.y, m_LatestImpactSpeed.y);
        float fallSpeedRatio = (fallSpeed - minSpeedForFallDamage) / (maxSpeedForFallDamage - minSpeedForFallDamage);
        if (recievesFallDamage && fallSpeedRatio > 0f)
        {
            float dmgFromFall = Mathf.Lerp(fallDamageAtMinSpeed, fallDamageAtMaxSpeed, fallSpeedRatio);
            m_Health.TakeDamage(dmgFromFall, null);

            // fall damage SFX
            //audioSource.PlayOneShot(fallDamageSFX);
        }
        else
        {
            // land SFX
            //audioSource.PlayOneShot(landSFX);
        }
    }

}
