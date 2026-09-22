using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2D : MonoBehaviour
{
    public Transform ThisTransform;
    public Rigidbody2D ThisBody;
    public float m_moveSpeed = 5.0f;
    public float JumpSpeed = 10f;
    public bool IsMoving;
    public bool IsJumping;
    public bool IsFalling;

    public float CurrentRigidbodyPositionX;
    public float CurrentRigidbodyPositionY;

    // Start is called before the first frame update
    void Start()
    {
        ThisTransform = transform;
        ThisBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        float Left = ThisTransform.position.x;
        float Right = ThisTransform.position.y;

        if (Input.GetKey(KeyCode.A))
        {
            Left -= m_moveSpeed * Time.deltaTime;
            CurrentRigidbodyPositionX = ThisBody.position.x;
            //按下A键时获取一次当前Rigidbody2D的position的x值赋值给CurrentRigidbodyPositionX
        }
        if (Input.GetKey(KeyCode.D))
        {
            Left += m_moveSpeed * Time.deltaTime;
            CurrentRigidbodyPositionX = ThisBody.position.x;
            //按下D键时获取一次当前Rigidbody2D的position的x值赋值给CurrentRigidbodyPositionX
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Right += JumpSpeed * Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CurrentRigidbodyPositionY = ThisBody.position.y;
            //松开空格键时获取一次当前Rigidbody2D的position的y值赋值给CurrentRigidbodyPositionY
        }

        ThisTransform.position = new Vector2(Left, Right);

        //Debug.Log(ThisBody.velocity.magnitude);
        //通过Debug发现当前Rigidbody2D的velocity的magnitude值小于0.1时，物体已经落地，
        //此时获取一次当前Rigidbody2D的position的y值赋值给CurrentRigidbodyPositionY
        if (ThisBody.velocity.magnitude < 0.1) CurrentRigidbodyPositionY = ThisBody.position.y;

        IsMoving = ThisBody.position.x == CurrentRigidbodyPositionX ? true : false;
        //比对当前Rigidbody2D的position的x值是否等于刚才已赋值的CurrentRigidbodyPositionX？是则说明在移动,IsMoving设为true;

        IsJumping = ThisBody.position.y > CurrentRigidbodyPositionY ? true : false;
        //比对当前Rigidbody2D的position的y值是否大于刚才已赋值的CurrentRigidbodyPositionY？是则说明在跳跃,IsJumping设为true;

        IsFalling = ThisBody.position.y < CurrentRigidbodyPositionY ? true : false;
        //比对当前Rigidbody2D的position的y值是否小于刚才已赋值的CurrentRigidbodyPositionY？是则说明在下坠,IsFalling设为true;
    }
}