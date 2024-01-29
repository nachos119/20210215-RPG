using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour
{    
    public float speed;             //회전 속도
    private CinemachineVirtualCamera cameraRotate;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        cameraRotate = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
       // if (speed != 0)
       //     OrbitAround();

        if (Input.GetKeyDown(KeyCode.Q))
        {
 
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            speed++;
        }
        //if (Input.GetKeyUp(KeyCode.Q))
        //{
        //    speed = 0;
        //}
        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    speed = 0;
        //}

    }

    void OrbitAround()
    {
        //CinemachineShot. 
        
    }
}

