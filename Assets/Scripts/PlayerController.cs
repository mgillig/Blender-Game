using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public bool enableMove = false;
    [SerializeField] public bool enableFire = true;

    [SerializeField] private CharacterController controller;
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 6f;
    [SerializeField] private Animator shotgunAnimator;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip deathSound;

    private int health;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;
    private GameController gameController;
    private bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponentInChildren<PlayerHealth>();
        health = playerHealth.GetMaxHealth();
        audioSource = GetComponent<AudioSource>();
        gameController = GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableMove)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            var moveDirection = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * new Vector3(horizontalInput, 0f, verticalInput);
            moveDirection.Normalize();
            controller.Move(speed * Time.deltaTime * moveDirection);
        }
        if (enableFire)
        {
            bool fireInput = Input.GetButtonDown("Fire1");
            if (fireInput)
                Fire();
        }
        if(endGame && !audioSource.isPlaying)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        var enemyController = other.gameObject.GetComponent<EnemyController>();
        if(enemyController != null && enemyController.IsActive() && gameController.gameStarted && !gameController.debugMode)
        {
            health--;
            playerHealth.SetHealth(health);
            if(health == 0)
                Die();
            else
                enemyController.TriggerStun(false);
        }
        else if (gameController.debugMode)
        {
            enemyController.TriggerStun(true);
        }
    }

    private void Die()
    {
        transform.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(true);
        audioSource.clip = deathSound;
        audioSource.Play();
        gameController.PauseGame();
        endGame = true;
    }

    private void Fire()
    {
        if (shotgunAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Ready"))
        {
            shotgunAnimator.SetTrigger("Fire");
            audioSource.clip = fireSound;
            audioSource.Play();
            RaycastHit hit;
            //Debug.DrawRay(transform.position, Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward, Color.red);
            if (Physics.Raycast(transform.position, Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward, out hit))
            {
                var enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
                var cubeController = hit.collider.gameObject.GetComponent<DefaultCubeController>();
                if (enemyController != null)
                    enemyController.TriggerStun(true);
                else if(cubeController != null)
                    cubeController.TriggerStart();
            }
        }
    }
}
