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
    [SerializeField] private GameObject shotgun;
    //[SerializeField] private Animator shotgunAnimator;
    [SerializeField] private AudioClip fireSound;

    private int health;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;
    private GameController gameController;
    private Animator shotgunAnimator;
    private AnimatorStateInfo animatorState;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponentInChildren<PlayerHealth>();
        health = playerHealth.GetMaxHealth();
        audioSource = GetComponent<AudioSource>();
        gameController = GetComponent<GameController>();
        shotgunAnimator = shotgun.GetComponent<Animator>();
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
        //if(endGame && !audioSource.isPlaying)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameController.gameActive)
        {
            var enemyController = other.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.IsActive() && gameController.gameStarted && !gameController.debugMode)
            {
                health--;
                playerHealth.SetHealth(health);
                if (health == 0)
                    gameController.Die();
                else
                    enemyController.TriggerStun(false);
            }
            else if (gameController.debugMode)
            {
                enemyController.TriggerStun(true);
            }
        }
    }

    //private void Die()
    //{
    //    audioSource.clip = deathSound;
    //    audioSource.loop = true;
    //    audioSource.Play();
    //    gameController.Die();
    //}

    private void Fire()
    {
        if (shotgunAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Ready"))
        {
            shotgunAnimator.SetTrigger("Fire");
            audioSource.clip = fireSound;
            audioSource.loop = false;
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

    public void EquipShotgun(bool equip, bool showEquipAnimation = false)
    {
        if (equip && !shotgun.activeInHierarchy)
        {
            shotgun.SetActive(true);
            var tagHashTest = Animator.StringToHash("Ready");
            var tagHashTest2 = Animator.StringToHash("Busy");
            if (showEquipAnimation && (animatorState.IsTag("Ready") || animatorState.tagHash == 0))
                shotgunAnimator.SetTrigger("Equip");
            else
                shotgunAnimator.Play(animatorState.shortNameHash, 0, animatorState.normalizedTime);
        }
        else if(!equip)
        {
            animatorState = shotgunAnimator.GetCurrentAnimatorStateInfo(0);
            shotgun.SetActive(false);
        }
    }

    public void ResetHealth()
    {
        health = playerHealth.GetMaxHealth();
        playerHealth.SetHealth(playerHealth.GetMaxHealth());
    }
}
