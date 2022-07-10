using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOnOff : MonoBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("ON"))
        {
            BluetoothManager.instance.Connect();
        }
        else if (coll.CompareTag("OFF"))
        {
            BluetoothManager.instance.Disconnect();
        }
    }
}
