using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour
{
    [Tooltip("Root GameObject of the menu used to toggle its activation用于切换其激活的菜单的根游戏对象")]
    public GameObject menuRoot;
    [Tooltip("Master volume when menu is open菜单打开时的主音量")]
    [Range(0.001f, 1f)]
    public float volumeWhenMenuOpen = 0.5f;
    [Tooltip("Slider component for look sensitivity用于外观灵敏度的滑块组件")]
    public Slider lookSensitivitySlider;
    [Tooltip("Toggle component for shadows切换阴影组件")]
    public Toggle shadowsToggle;
    [Tooltip("Toggle component for invincibility切换无敌组件")]
    public Toggle invincibilityToggle;
    [Tooltip("Toggle component for framerate display用于帧率显示的切换组件")]
    public Toggle framerateToggle;
    [Tooltip("GameObject for the controls控件的游戏对象")]

    //public GameObject controlImage;

    //public PlayerInputHandler m_PlayerInputsHandler;
    // Health m_PlayerHealth;
    public FramerateCounter m_FramerateCounter;

    private CharacterManager PlayerScript;
    private WeaponManager WeaponManagerScript;
    void Start()
    {
        //m_PlayerInputsHandler = FindObjectOfType<PlayerInputHandler>();

        //m_PlayerHealth = m_PlayerInputsHandler.GetComponent<Health>();

        PlayerScript = FindObjectOfType<CharacterManager>();
        WeaponManagerScript = FindObjectOfType<WeaponManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_FramerateCounter = FindObjectOfType<FramerateCounter>();

        menuRoot.SetActive(false);

        framerateToggle.isOn = m_FramerateCounter.uiText.gameObject.activeSelf;
        framerateToggle.onValueChanged.AddListener(OnFramerateCounterChanged);
    }

    void Update()
    {
        //if (!menuRoot.activeSelf && Input.GetMouseButtonDown(0))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}

        if (Input.GetButtonDown(GameConstants.k_ButtonNamePauseMenu)
            || (menuRoot.activeSelf && Input.GetButtonDown(GameConstants.k_ButtonNameCancel)))
        {

            SetPauseMenuActivation(!menuRoot.activeSelf);

        }
    }

    public void ClosePauseMenu()
    {
        SetPauseMenuActivation(false);
    }

    public void SetPauseMenuActivation(bool active)
    {
        menuRoot.SetActive(active);
        m_FramerateCounter.HUD1.gameObject.SetActive(!active);
        m_FramerateCounter.HUD2.gameObject.SetActive(!active);

        if (menuRoot.activeSelf)
        {         
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
           // AudioUtility.SetMasterVolume(volumeWhenMenuOpen);

            EventSystem.current.SetSelectedGameObject(null);

            if (PlayerScript.characterAnimator)
            {
                PlayerScript.characterAnimator.enabled = false;
            }

            PlayerScript.enabled = false;
            WeaponManagerScript.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;

            if (PlayerScript.characterAnimator)
            {
                PlayerScript.characterAnimator.enabled = true;
            }
            PlayerScript.enabled = true;
            WeaponManagerScript.enabled = true;
            //AudioUtility.SetMasterVolume(1);
        }

    }
    public void OnFramerateCounterChanged(bool newValue)
    {
        m_FramerateCounter.uiText.gameObject.SetActive(newValue);
    }

}
