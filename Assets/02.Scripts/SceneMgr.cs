using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr instance = null;
    
    private static readonly int SceneChange = Animator.StringToHash("SceneChange");

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);    
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
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
