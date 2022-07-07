using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToPosition : MonoBehaviour
{
    public float limitHeight;
    public Transform originalPos;
    void Start()
    {
        
    }
    
    void Update()
    {
        // 만약 이 오브젝트의가 일정 높이 이하로 떨어지면
        if (transform.position.y <= limitHeight)
        {
            // 원위치로 돌아간다.
            transform.position = originalPos.position;
        }
    }
}
