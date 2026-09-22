using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public int Powers = 0;
    public AudioClip PowerPick;
    public AudioSource gameManagerAudioSource;

    public bool Esc = false;
    public bool GameWin = false;
    private bool GameWin1 = false;

    public GameObject Player;
    private CharacterManager PlayerScript;
    private WeaponManager WeaponManagerScript;

    public Text GameInfo;
    public float GameTimer;
    private float GameTimer1;

    public GameObject AttitudeUI;
    private Image attitudeGUI;
    public Sprite[] attitude_GUI;

    private void Start()
    {   
        PlayerScript = Player.GetComponent<CharacterManager>();
        WeaponManagerScript = Player.GetComponent<WeaponManager>();
        GameInfo.text = "游戏胜利条件:拾取建筑物中的燃料,按G键查看任务";
        attitudeGUI = AttitudeUI.GetComponent<Image>();
    }

    private void Update()
    {
        attitudeGUI.sprite = attitude_GUI[PlayerScript.AttitudeUI];

        GameTimer1 = GameTimer1 + Time.deltaTime;
        if (GameTimer1 > 8f)
        {
            GameInfo.text = "";
            GameTimer1 = 0;
        }

        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Esc = !Esc;
            if (Esc == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (PlayerScript.characterAnimator)
                {
                    PlayerScript.characterAnimator.enabled = false;
                }

                PlayerScript.enabled = false;
                WeaponManagerScript.enabled = false;

            }
            else if (Esc == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (PlayerScript.characterAnimator)
                {
                    PlayerScript.characterAnimator.enabled = true;
                }
                PlayerScript.enabled = true;
                WeaponManagerScript.enabled = true;
            }
        }
        */

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (GameWin1 == false)
            {
                GameInfo.text = "游戏胜利条件:拾取建筑物中的燃料，按G键查看任务";
                GameTimer = GameTimer + Time.deltaTime;
                if (GameTimer > 4f)
                {
                    GameInfo.text = "";
                    GameTimer = 0;
                }
            }
            else if (GameWin1 == true)
            {
                GameInfo.text = "游戏胜利,按Esc键退出游戏";
                GameTimer = GameTimer + Time.deltaTime;
                if (GameTimer > 3f)
                {
                    GameInfo.text = "";
                    GameTimer = 0;
                }
            }
        }

        if(GameWin == true)
        {
            GameInfo.text = "游戏胜利,按Esc键退出游戏";
            GameTimer = GameTimer + Time.deltaTime;
            if (GameTimer > 3f)
            {
                GameInfo.text = "";
                GameTimer = 0;
                GameWin1 = true;
                GameWin = false;
            }
        }
    }


}
