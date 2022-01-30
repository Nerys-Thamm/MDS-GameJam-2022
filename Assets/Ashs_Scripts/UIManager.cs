using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    SceneManager m_Scenemanager;

    public void NextLEvel(string _Levelname)
    {
        SceneManager.LoadScene(_Levelname);
    }
}
