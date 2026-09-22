using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 脚本生命周期/必然事件/消息 Message
/// </summary>

public class Lifecycle : MonoBehaviour
{
    //脚本：.cs的文本文件 类文件
    //附加到游戏物体中，定义游戏对象行为指令的代码。


    //序列化字段 作用：在编辑器中显示私有变量
    [SerializeField]
    private int a = 100;

    //作用:在编辑器中隐藏字段
    [HideInInspector]
    public int b;

    [Range(0, 100)]
    public int c;

    //属性：在编辑器中不能显示、通常脚本中不写
    public int A
    {
        get
        {
            return this.a;
        }
        set
        {
            this.a = value;
        }
    }

    public Lifecycle()

    {
        Debug.Log("构造函数");
        //不要在脚本中写构造函数
        //不能在子线程中访问主线程成员
        //b=Time.time;
    }

    private int d = Random.Range(1,101);

    //**********初始阶段***********
    //执行时机：创建游戏对象--> 立即执行1次（早于Start）
    //作用：初始化
    private void Awake()
    {
        Debug.Log("Awake--"+Time.time+"--"+this.name);
    }

    //执行时机：创建游戏对象-->脚本启用-->才执行1次
    //作用：初始化
    private void Start()
    {
        //int a = 1;


        Debug.Log("Start--" + Time.time + "--" + this.name);
        print("ok");
    }
    /*
    Awake唤醒：
    当物体载入时立即调用1次；常用于在游戏开始前进行初始化，可以判断当满足某种条件执行此脚本 this.enable=true.

    OnEnable当可用：
    每当脚本对象启用时调用。

    Start开始：
    物体载入且脚本对象启用时被调用1次.常用于数据或游戏逻辑初始化，执行时机晚于Awake。
    */

    //**********物理阶段**********
    //执行时机：每隔固定时间执行1次。（时间可以修改）
    //适用性：对物体做物理操作（移动、旋转......）,不会受到渲染影响
    private void FixedUpdate()
    {
        //渲染时间不固定（每帧渲染量不同、机器性能不同）
        Debug.Log(Time.time);
    }

    /*
    FixedUpdate固定更新：
    脚本启用后，固定时间被调用，适用于对游戏对象做物理操作。例如移动等。
    设置更新频率："Edit"--> "Project Setting"-->"Time"-->"Fixed Timestop"值，默认为0.02s
    
    OnCollisionXXX碰撞：
    当满足碰撞条件时被调用。
    OnTriggerXXX触发：
    当满足触发条件时被调用。

    */

    //执行时机：渲染帧执行，执行间隔不固定
    //适用性：处理游戏逻辑
    private void Update()
    {
        //单帧调试： 启用调试 运行场景 暂停游戏 加断点 单帧执行 结束调试
        //int a = 1;
        //int b = 2;
        //int c = a + b;
        //调试过程中输入代码：
        //右键——快速监视
        //查看“即时窗口”
        //time + Time.time;
    }



    /*
    Update更新：
    脚本启用后，每次渲染场景时调用，频率与设备性能及渲染量有关。
    
    LateUpdate延迟更新：
    在Update函数被调用后执行，适用于跟随逻辑


    输入事件
    OnMouseEnter鼠标移入
    鼠标移入当前Collider时调用。

    OnMouseOver鼠标经过
    鼠标经过当前Collider时调用。

    OnMouseExit鼠标离开
    鼠标离开当前Collider时调用。

    OnMouseDown鼠标按下
    鼠标按下当前Collider时调用。

    OnMouseUp鼠标抬起
    鼠标在当前Collider上抬起时调用。

    */


    /*
    场景渲染
    OnBecameVisible当可见：
    当Mesh Renderer在任何相机上可见时调用。

    OnBecameInvisible当不可见：
    当Mesh Renderer在任何相机上都不可用时调用。

    */

    /*
    结束阶段
    OnDisable当不可用：
    对象变为不可用或附属游戏对象非激活状态时此函数被调用。

    OnDestroy当销毁：
    当脚本销毁或附属的游戏对象被销毁时调用。

    OnApplicationQuit当程序结束：
    应用程序退出被调用。
    */

    //观看到Unity脚本1-06

    /*
    使用Unity编辑器

    将程序投入到实际运行中，通过开发工具进行测试，修正逻辑错误的过程。
    1.控制台调试
    Debug.Log(变量);
    print(变量);
    2.定义共有变量，程序运行后再检测面板查看数据

    调试步骤：
    （1）在可能出现的行添加断点
    （2）启动调试
    （3）在Unity中Play场景


    */
}
