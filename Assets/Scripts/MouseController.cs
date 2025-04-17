using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public GameObject player;

    private bool enableCamera = false;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableCamera)
        {
            float mouseX = Input.GetAxis("Mouse X");
            player.transform.Rotate(new Vector3(0f, sensitivity * mouseX, 0f));
        }
    }

    public void SetMouseMode(bool allowCameraRotate, bool lockMouse)
    {
        enableCamera = allowCameraRotate;
        if(lockMouse)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
    }
}
