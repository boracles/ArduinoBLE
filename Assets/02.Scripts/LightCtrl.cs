using System;
using UnityEngine;
using ArduinoBluetoothAPI;

public class LightCtrl : MonoBehaviour
{
    private BLE bleMgr;
    
    public Light directionalLight;
    public Light spotLight;
    public Renderer bulbMeshRenderer;
    private Material bulbMat;

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

    void Start()
    {
        bleMgr = BLE.instance;
        bulbMat = bulbMeshRenderer.material;
    }
    
    
    void OnMessageArrival()
    {
        // Directional Light의 값에 대입해준다. 
        // 0 - 880 > 1-0 : 조도 센서가 있는 환경에서의 값을 비교해서 수치를 조정한다. 
           
        float _intensity = 1 - ((float.Parse(bleMgr.message)) / 880);
                
        //Debug.Log($" 조도 = {bleMgr.message}");
        directionalLight.intensity = _intensity*2.0f;
        spotLight.intensity = _intensity*12.0f;
        bulbMat.SetColor("_EmissionColor", new Color(11.98431f, 11.98431f, 11.98431f)*_intensity*4.0f); 
    }
}