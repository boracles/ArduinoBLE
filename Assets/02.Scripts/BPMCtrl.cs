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

    private static readonly int Bpm = Shader.PropertyToID("BPM");
    
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
        bpm = float.Parse(bleMgr.message);
        
        if (bpm > 1.0f)
        {
            wallMat.SetFloat(Bpm, bpm/200.0f);
        }
        else switch (bpm)
        {
            case 1.0f:
                bulb.SetActive(true);
                break;
            case 0.0f:
                bulb.SetActive(false);
                break;
        }
            
    }
}