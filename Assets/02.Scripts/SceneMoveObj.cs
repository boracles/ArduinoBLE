using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMoveObj : MonoBehaviour
{
    private int sceneInfo;

    private void Start()
    {
        sceneInfo = GetComponent<SceneInfo>().sceneInfo;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("ZONE")) return;
        SceneMgr.instance.MoveScene(sceneInfo);
        BLE.instance.SendData(sceneInfo);
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("ZONE"))
        {
            Destroy(gameObject, 0);
        }
    }
}
