using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public GameObject player;
    public float speed = 6f;
    public Animator shotgunAnimator;
    private int health;
    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponentInChildren<PlayerHealth>();
        health = playerHealth.GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        bool fireInput = Input.GetButtonDown("Fire1");

        var moveDirection = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * new Vector3(horizontalInput, 0, verticalInput);
        moveDirection.Normalize();
        controller.Move(speed * Time.deltaTime * moveDirection);
        if (fireInput)
        {
            fire();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemyController = other.gameObject.GetComponent<EnemyController>();
        if(enemyController != null && enemyController.IsActive())
        {
            health--;
            playerHealth.SetHealth(health);
            enemyController.TriggerStun(false);
        }
    }

    private void fire()
    {
        if (shotgunAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Ready"))
        {
            shotgunAnimator.SetTrigger("Fire");
            RaycastHit hit;
            //Debug.DrawRay(transform.position, Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward, Color.red);
            if (Physics.Raycast(transform.position, Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward, out hit))
            {
                var enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
                if (enemyController != null)
                    enemyController.TriggerStun(true);
            }
        }
    }
}
