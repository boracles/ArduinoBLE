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
    private static string serviceUUID = "00001800-0000-1000-8000-00805f9b34fb";
    private static string characteristicUUID = "00002a00-0000-1000-8000-00805f9b34fb";
    private BluetoothHelperCharacteristic bluetoothHelperCharacteristic;
    
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
            helper = BluetoothHelper.GetInstance("BORATOOTH");
            helper.OnScanEnded += OnScanEnded;
            helper.OnConnected += OnConnected;
            helper.OnDataReceived += OnMessageReceived;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.OnCharacteristicChanged += OnCharacteristicChanged;
            helper.OnCharacteristicNotFound += OnCharacteristicNotFound;
            helper.OnServiceNotFound += OnServiceNotFound;
        
            helper.ScanNearbyDevices();
        
            helper.setTerminatorBasedStream("\n");
        
            Permission.RequestUserPermission(Permission.CoarseLocation);
            bluetoothHelperCharacteristic = new BluetoothHelperCharacteristic(characteristicUUID, serviceUUID);
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

    void OnCharacteristicChanged(BluetoothHelper helper, byte [] data, BluetoothHelperCharacteristic characteristic)
    {
        Debug.Log($"Update valud for characteristic [{characteristic.getName()}] of service [{characteristic.getService()}]");
        Debug.Log($"New value : [{System.Text.Encoding.ASCII.GetString(data)}]");
    }

    void OnServiceNotFound(BluetoothHelper helper, string service)
    {
        Debug.Log($"Service [{service}] not found");
    }

    void OnCharacteristicNotFound(BluetoothHelper helper, string service, string characteristic)
    {
        Debug.Log($"Characteristic [{service}] of service [{service}] not found");
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
        helper.OnCharacteristicChanged -= OnCharacteristicChanged;
        helper.OnCharacteristicNotFound -= OnCharacteristicNotFound;
        helper.OnServiceNotFound -= OnServiceNotFound;
        helper.OnDataReceived -= OnMessageReceived;
        helper.Disconnect();
    }
}
