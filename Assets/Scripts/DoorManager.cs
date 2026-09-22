using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorManager : MonoBehaviour
{
    public GameManager GameManagerScript;
    public Light DoorLight;

    public Animation DoorAnimation;
   // public AudioSource DoorAudio;
    public AudioClip door_open_sound;
    public AudioClip door_shut_sound;
    public bool DoorOpenApply = false;

    public Text daba;

    private bool DoorIsOpen;
    public float DoorTimer;
    public float DoorOpenTime = 3;

    //仪表盘引用变量和材质数组
    public Renderer chargeMeter;
    public Texture2D[] chargeMeter_Texture;

    //电池引用变量和材质数组
    public GameObject PowerGUI;
    private Image powerGUI;
    public Sprite[] power_GUI;
    
    private void Start()
    {

        GameManagerScript = FindObjectOfType<GameManager>();
        DoorAnimation = transform.parent.GetComponent<Animation>();
        powerGUI = PowerGUI.GetComponent<Image>();
    }

    private void Update()
    {

        if(GameManagerScript.Powers >0)
        {
            powerGUI.enabled = true;
            powerGUI.sprite = power_GUI[GameManagerScript.Powers];
            chargeMeter.material.mainTexture = chargeMeter_Texture[GameManagerScript.Powers];

        }
        if (GameManagerScript.Powers == 4)
        {
            DoorLight.color = Color.green;
            powerGUI.enabled = false;
        }

        if (DoorIsOpen)
        {
            DoorTimer = DoorTimer + Time.deltaTime;
            if (DoorTimer > DoorOpenTime)
            {
                Door(false, door_shut_sound, "doorshut");
                DoorTimer = 0;

            }
        }

        if (GameManagerScript.Powers == 4 && DoorOpenApply == true)
        {
            DoorCheck();
            DoorOpenApply = false;
        }
        else if(GameManagerScript.Powers < 4 && DoorOpenApply == true)
        {
            
            daba.text = "需要4个能量源才能开门,当前拥有" + GameManagerScript.Powers + "个";
            DoorTimer = DoorTimer + Time.deltaTime;
            if(DoorTimer > 2f)
            {
                daba.text = "";
                DoorOpenApply = false;
                DoorTimer = 0;
            }
        }
    }

    private void DoorCheck()
    {
        if(!DoorIsOpen)
        {
            Door(true, door_open_sound, "dooropen");
        }
    }

    private void Door(bool doorcheck,AudioClip a_clip,string anim_name)
    {
        DoorIsOpen = doorcheck;

        AudioSource.PlayClipAtPoint(a_clip,transform.position);
        DoorAnimation.Play(anim_name);
    }









}
