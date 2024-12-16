using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        player.transform.Rotate(new Vector3(0f, sensitivity * mouseX, 0f));
    }
}
