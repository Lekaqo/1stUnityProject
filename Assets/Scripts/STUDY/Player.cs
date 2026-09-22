using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform m_transform;//定义player物体的坐标组件附加物体
    CharacterController m_ch;//定义角色控制器组件

    public float m_moveSpeed = 5.0f;
    float m_gravity = 20.0f;

    private bool canJump=true;//设置跳跃动作触发开关
    private bool canMove=true;
    //private bool airMove;

    private CollisionFlags flags;
    public float my_jumpSpeed = 10.0f;
    public Vector2 MaxminAngle;

    private Vector3 my_moveDirection =Vector3.zero;

    Transform myCameraTransform;
    Vector3 my_cameraRot;
    float my_cameraHeight = 1.4f;

   

    void Start()
    {
        m_transform = this.transform;//获得角色控制器坐标数据
        m_ch = this.GetComponent<CharacterController>();//获取角色控制器组件并授权给m_ch

        myCameraTransform = Camera.main.transform;//找到主摄像机的坐标
        Vector3 pos = m_transform.position;//角色控制器坐标数据赋值给pos
        pos.y += my_cameraHeight;//设定摄相机高度

        myCameraTransform.position = pos;//主摄相机坐标信息赋值给pos vector3数据类型
        m_transform.rotation = myCameraTransform.rotation; //主摄相机跟随player物体旋转
        my_cameraRot = myCameraTransform.eulerAngles;
        //myCameraTransform.rotation = Quaternion.Euler(my_cameraRot.x, my_cameraRot.y, z:0);

        Cursor.lockState = CursorLockMode.Locked;//锁定鼠标
    }

   

    void Update()
    {
        Control();
    }

    void FixedUpdate()
    {
        Jump();
    }

    void Control()
    {
        Vector3 camRot = myCameraTransform.eulerAngles;
        camRot.x = 0;
        camRot.z = 0;
        myCameraTransform.eulerAngles = camRot;

        Vector3 pos = m_transform.position;
        pos.y += my_cameraHeight;
        myCameraTransform.position = pos;

        float rh = Input.GetAxis("Mouse X");//获取鼠标水平方向移动距离
        float rv = Input.GetAxis("Mouse Y");//获取鼠标垂直方向移动距离
        my_cameraRot.x -= rv;
        my_cameraRot.y += rh;
        myCameraTransform.eulerAngles = my_cameraRot;
        m_transform.rotation = Quaternion.Euler(x: 0, y: my_cameraRot.y, z: 0);


        if (canMove)    
        {
            float xm = 0, ym = 0, zm = 0;
            if (Input.GetKey(KeyCode.W))
            {
                zm += m_moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                zm -= m_moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                xm -= m_moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                xm += m_moveSpeed * Time.deltaTime;
            }

            m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));
            //使角色控制器的前进方向跟随相机的转动而更新
        }
    }
  
    void Jump()
    {
        my_moveDirection = m_transform.TransformDirection(my_moveDirection);
        // 空格键控制跳跃    
        if (Input.GetButton("Jump") && canJump)
        {
            canJump = false;
            
            my_moveDirection.y = my_jumpSpeed;//使物体向y轴上方移动跳跃速度移动的距离            

            if (Input.GetKey(KeyCode.W) && Input.GetButton("Jump"))
            {
                canMove = true;
            }
            else if(Input.GetKey(KeyCode.S) && Input.GetButton("Jump"))
            {
                canMove = true;
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetButton("Jump"))
            {
                canMove = true;
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetButton("Jump"))
            {
                canMove = true;
            }
            else
            {
                canMove = false;
            }

        }
        
        if (!canJump)
        {
            //模拟物理,开始下降
            my_moveDirection.y -= m_gravity * Time.deltaTime;
            flags = m_ch.Move(my_moveDirection * Time.deltaTime);//为player物体坠落方向打上标签
           
            //人物碰撞到下面了
            if (flags == CollisionFlags.CollidedBelow)
            {
                canJump = true;
                canMove = true;
            }
        }
    }
    

}
