using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName = "";

    InGameMenuManager m_InGameMenuManager;

    private void Start()
    {
        m_InGameMenuManager = FindObjectOfType<InGameMenuManager>();
    }
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject
            && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
        {
            LoadTargetScene();
            ExitGame();
            OpenInGameMenu();
        }
    }

    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenInGameMenu()
    {
        m_InGameMenuManager.SetPauseMenuActivation(m_InGameMenuManager.menuRoot.activeSelf);
    }


}
