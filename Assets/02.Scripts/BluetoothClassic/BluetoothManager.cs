using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using UnityEngine.Android;
using TMPro;

public class BluetoothManager : MonoBehaviour
{
    private BluetoothHelper helper;

    private string deviceName;
    
    // 블루투스로 들어오는 값을 연결할 변수
    public TMP_Text bluetoothMsg;
    public string message;
    private string tmp;

    public GameObject onButton;
    public GameObject offButton;
    
    public static BluetoothManager instance = null;
    
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
    
    void Start()
    {
        deviceName = "BORATOOTH";
        
        try
        {
            BluetoothHelper.BLE = false;
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.OnDataReceived += OnDataReceived;    // 데이터 읽어오기
            helper.OnScanEnded += OnScanEnded;
        
            helper.setTerminatorBasedStream("\n");

            Permission.RequestUserPermission(Permission.CoarseLocation);

            if (!helper.ScanNearbyDevices())
            {
                // 블루투스 연결 시도하기 (블루투스 클래식 모드)
                helper.Connect();
            }
        }
        catch (Exception e)
        {
            WriteMsg(e.Message);
        }
    }

    void OnConnected(BluetoothHelper helper)
    {
        WriteMsg("블루투스 연결 완료");
        
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
    }

    void OnDataReceived(BluetoothHelper helper)
    {
        message = helper.Read();
        WriteMsg($"들어온 값 : {message}");
    }

    private void WriteMsg(string msg)
    {
        tmp += $"> {msg} \n";
        bluetoothMsg.text = tmp;
    }

    public void Connect()
    {
        // 블루투스 연결 시도
        helper.Connect();
        onButton.SetActive(false);
        offButton.SetActive(true);
    }

    public void Disconnect()
    {
        helper.Disconnect();
        onButton.SetActive(true);
        offButton.SetActive(false);
    }

    void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
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
    
    // 아두이노로 데이터 보내기
    public void SendData(int i)
    {
        helper.SendData(i.ToString());
    }
    
    void OnDestroy()
    {
        if (helper != null)
            helper.Disconnect();
    }
}
