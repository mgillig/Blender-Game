using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public GameObject player;
    public float speed = 6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        var moveDirection = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * new Vector3(horizontalInput, 0, verticalInput);
        controller.Move(speed * Time.deltaTime * moveDirection);
    }
}
