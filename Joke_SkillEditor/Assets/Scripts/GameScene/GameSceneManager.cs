using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
public class GameSceneManager : LogicManagerBase<GameSceneManager>
{
    #region ≤‚ ‘¬ﬂº≠
    public bool IsTest;
    public bool IsCreateArchive;
    #endregion
    private void Start()
    {
        #region ≤‚ ‘¬ﬂº≠
        if (IsTest)
        {
            if (IsCreateArchive)
            {
                DataManager.CreateArchive();
            }
            else
            {
                DataManager.LoadCurrentArchive();
            }
        }
        #endregion
        // ≥ı ºªØΩ«…´
        Player_Controller.Instance.Init();
    }
}
