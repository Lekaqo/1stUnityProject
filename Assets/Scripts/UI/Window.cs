using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    private bool WindowSwitch;
    public Rect windowRect = new Rect(250, 150, 240, 150);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            WindowSwitch = !WindowSwitch;
        }
    }
    void OnGUI()
    {
        if (WindowSwitch)
        {
            windowRect = GUI.Window(0, windowRect, DoMyWindow, "菜单");
            
        }
    }

    void DoMyWindow(int windowID)
    {

        if (GUI.Button(new Rect(70, 70, 100, 20), "退出游戏"))
        {
            Application.Quit();
        }
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));


    }

}
