using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using UnityEngine.Android;
using TMPro;

public class BLE : MonoBehaviour
{
    private BluetoothHelper helper;

    // 싱글턴 인스턴스 선언
    public static BLE instance = null;
    
    // 블루투스로 들어오는 값을 연결할 변수
    public TMP_Text bleMsg;
    public string message;
    private string tmp;
    
    // 델리게이트 선언
    public delegate void MessageHandler();
    // 이벤트 선언
    public static event MessageHandler OnMessageArrival;
    
    // 스크립트가 실행되면 가장 먼저 호출되는 유니티 이벤트 함수
    private void Awake()
    {
        // instance가 할당되지 않았을 경우
        if (instance == null)
        {
            instance = this;
        }
        // instance에 할당된 클래스의 인스턴스가 다를 경우
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // 다른씬으로 넘어가더라도 삭제하지 않고 유지
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        try
        {
            BluetoothHelper.BLE = true;
            helper = BluetoothHelper.GetInstance("ETTOI");
            helper.OnScanEnded += OnScanEnded;
            helper.OnConnected += OnConnected;
            helper.OnDataReceived += OnMessageReceived;
            helper.OnConnectionFailed += OnConnectionFailed;

            helper.ScanNearbyDevices();
        
            helper.setTerminatorBasedStream("\n");
        
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        catch (Exception e)
        {
            WriteMsg(e.Message);
        }
        
    }

    private void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {
        if (helper.isDevicePaired())
        {
            helper.Connect();
        }
        else
        {
            helper.ScanNearbyDevices();
        }
    }

    private void WriteMsg(string msg)
    {
        tmp += $"> {msg} \n";
        bleMsg.text = tmp;
    }

    // 아두이노로부터 도착한 데이터 처리
    private void OnMessageReceived(BluetoothHelper helper)
    {
        message = helper.Read();
        WriteMsg($"들어온 값 : {message}");

        OnMessageArrival();
    }
    
    void OnConnected(BluetoothHelper helper)
    {
        WriteMsg($"블루투스 연결 완료");

        try
        {
            helper.StartListening();
        }
        catch (Exception e)
        {
            WriteMsg(e.Message);
        }
    }

    void OnConnectionFailed(BluetoothHelper helper)
    {
        WriteMsg("블루투스 연결 실패");
        helper.ScanNearbyDevices();
    }

    public void SendData(int i)
    {
        helper.SendData(i.ToString());
    }

    private void OnDestroy()
    {
        helper.OnScanEnded -= OnScanEnded;
        helper.OnConnected -= OnConnected;
        helper.OnConnectionFailed -= OnConnectionFailed;
        helper.OnDataReceived -= OnMessageReceived;
        helper.Disconnect();
    }
}