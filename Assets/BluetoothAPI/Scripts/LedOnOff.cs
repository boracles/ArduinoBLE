using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Text;

public class LedOnOff : MonoBehaviour
{

    // 초기화
    BluetoothHelper bluetoothHelper;
    string deviceName;

    string receivedMessage;

    private string tmp;

    void Start()
    {
        deviceName = "BORATOOTH";
        try
        {
            BluetoothHelper.BLE = false;
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; // 데이터 읽어오기
            bluetoothHelper.OnScanEnded += OnScanEnded;

            bluetoothHelper.setTerminatorBasedStream("\n");

            if(!bluetoothHelper.ScanNearbyDevices())
            {
                // 연결 시도하기 - 블루투스 클래식 모드에서 작동
                bluetoothHelper.Connect();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            write(ex.Message);
        }
    }

    private void write(string msg)
    {
        tmp += ">"+ msg + "\n";
    }

    void OnMessageReceived(BluetoothHelper helper)
    {
        receivedMessage = helper.Read();
        Debug.Log(receivedMessage);
        write("들어온 값 : " +receivedMessage);
    }

    void OnConnected(BluetoothHelper helper)
    {
        try
        {
            helper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            write(ex.Message);

        }
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

    void OnConnectionFailed(BluetoothHelper helper)
    {
        write("블루투스 연결 실패");
        Debug.Log("블루투스 연결 실패");
    }


    //Call this function to emulate message receiving from bluetooth while debugging on your PC.
    void OnGUI()
    {
        tmp = GUI.TextField(new Rect(Screen.width / 4, Screen.height / 10, Screen.width / 2, Screen.height / 10 - 10), tmp);

        if (bluetoothHelper != null)
            bluetoothHelper.DrawGUI();
        else
            return;

        if (!bluetoothHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Connect"))
            {
                if (bluetoothHelper.isDevicePaired())
                    bluetoothHelper.Connect(); // tries to connect
                else
                    write("블루투스를 연결할 수 없습니다.");
            }  

        if (bluetoothHelper.isConnected())
        {
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                bluetoothHelper.Disconnect();
                write("블루투스 연결 끊김");
            }

            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Turn On"))
            {
                bluetoothHelper.SendData("O");
                write("Sending O");
            }
        }
            
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
}
