using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   
    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadScene(string _Levelname)
    {
        SceneManager.LoadScene(_Levelname);
    }
    
}
