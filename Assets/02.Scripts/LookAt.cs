using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform camTr;

    private void Start()
    {
        camTr = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(camTr.position);
    }
}
