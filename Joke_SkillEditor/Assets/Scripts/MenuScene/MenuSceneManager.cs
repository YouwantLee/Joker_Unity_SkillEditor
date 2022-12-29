using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
public class MenuSceneManager : LogicManagerBase<MenuSceneManager>
{
    void Start()
    {
        UIManager.Instance.Show<UI_MenuSceneMenuWindow>();
    }
}
