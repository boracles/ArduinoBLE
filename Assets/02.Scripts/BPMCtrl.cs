using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Serialization;

public class BPMCtrl : MonoBehaviour
{
    private BLE bleMgr;
    public Material wallMat;
    public GameObject bulb;

    [FormerlySerializedAs("BPM")]
    [Header("BPM 50 - 240")]
    [Range(50, 240)]
    [SerializeField] private float bpm = 100.0f;

    private static readonly int flowTime = Shader.PropertyToID("FlowTime");
    private static readonly int flowIntensity = Shader.PropertyToID("FlowIntensity");
    
    private void OnEnable()
    {
        // 이벤트 발생시 수행할 함수 연결
        BLE.OnMessageArrival += OnMessageArrival;
    }

    private void OnDisable()
    {
        // 기존에 연결된 함수 해제
        BLE.OnMessageArrival -= OnMessageArrival;
    }

    private void Start()
    {
        bleMgr = BLE.instance;
    }

    private void OnMessageArrival()
    {
        bpm = int.Parse(bleMgr.message);
        
        if (bpm > 1)
        {
            wallMat.SetFloat(flowTime, bpm/240);
            wallMat.SetFloat(flowIntensity, bpm/24);
        }
        else switch (bpm)
        {
            case 1:
                bulb.SetActive(true);
                break;
            case 0:
                bulb.SetActive(false);
                break;
        }
            
    }
}