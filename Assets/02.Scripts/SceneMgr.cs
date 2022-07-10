using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr instance = null;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);    
        }
        DontDestroyOnLoad(gameObject);
    }

    public void MoveScene(int sceneInfo)
    {
        switch (sceneInfo)
        {
            case 0:
                SceneManager.LoadScene("3_Humidifier");
                break;
            case 1:
                SceneManager.LoadScene("3_Underwater");
                break;
            case 2:
                SceneManager.LoadScene("4_FanMotor");
                break;
        }
    }
}
