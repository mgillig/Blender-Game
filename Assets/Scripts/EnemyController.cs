using System.ComponentModel.Design.Serialization;
using Unity.Burst.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    public float cooldownTime;
    public float cooldown;
    public float primeDistance;
    public float speed;
    public AudioClip activationSound;
    public AudioClip biteSound;
    public AudioClip stunSound;

    private Vector3 pathTarget;
    private bool? mirrored = null;
    private Quaternion baseRotate;
    private AudioSource audioSource;
    private Transform playerTransform;
    private bool activated = false;
    private bool primed;
    private bool neverPlayedAudio = true;
    private GridController grid;


    //[SerializeField] private bool activated = false;

    void Start()
    {
        var player = GameObject.Find("Player");
        playerTransform = player.transform;
        grid = player.GetComponent<GridController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (cooldown <= 0f)
        {
            primed = Vector3.Distance(transform.position, playerTransform.position) < primeDistance;

            if (mirrored.HasValue)
                RunStunAnimation(false);

            SeePlayer();
            HearPlayer();
            Movement();
        }
        else
            cooldown -= Time.deltaTime;
    }

    private void SeePlayer()
    {
        Vector3 direction = (this.transform.position - playerTransform.position).normalized * -1;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                if (neverPlayedAudio)
                {
                    neverPlayedAudio = false;
                    audioSource.clip = activationSound;
                    audioSource.Play();
                }
                activated = true;
                pathTarget = hit.point;
                transform.LookAt(pathTarget);
            }
        }
    }

    private void HearPlayer()
    {
        if (primed || activated)
        {
            bool playerFireInput = Input.GetButtonDown("Fire1");
            if (playerFireInput)
            {
                activated = true;
                pathTarget = playerTransform.position;
                transform.LookAt(pathTarget);
                grid.GetPath(transform.position, pathTarget);
            }
        }
    }

    private void Movement()
    {
        if (activated)
        {
            //transform.position = Vector3.MoveTowards(transform.position, pathTarget, speed * Time.deltaTime);
        } 
    }

    public bool IsActive()
    {
        return activated && cooldown <= 0f;
    }

    public void TriggerStun(bool wasAttacked)
    {
        if (cooldown <= 0f)
        {
            cooldown = cooldownTime;
            RunStunAnimation(wasAttacked);

            if (!wasAttacked)
            {
                audioSource.clip = biteSound;
                audioSource.Play();
            }
            else
            {
                audioSource.clip = stunSound;
                audioSource.Play();
            }
        }
    }

    private void RunStunAnimation(bool runAnimation)
    {
        var activateStun = !mirrored.HasValue;
        var monkey = transform.GetChild(0);
        //animator.SetBool("Stun", runAnimation);
        if (activateStun)
        {
            baseRotate = monkey.rotation;
            mirrored = Random.Range(0, 2) == 0;
            monkey.Rotate(new Vector3(15f, 15f * (mirrored.Value ? 1f : -1f), 0f));
        }
        else
        {
            monkey.rotation = baseRotate;
            mirrored = null;
        }
    }
}
