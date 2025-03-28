using System.Collections.Generic;
using System.Linq;
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
    public float earShot;
    public float speed;
    public AudioClip activationSound;
    public AudioClip biteSound;
    public AudioClip stunSound;

    private Vector2Int home;
    private bool? mirrored = null;
    private Quaternion baseRotate;
    private AudioSource audioSource;
    private Transform playerTransform;
    private bool activated = false;
    private bool goHome = false;
    private bool neverPlayedAudio = true;
    private GridController grid;
    private List<Vector2Int> destinationQueue = new List<Vector2Int>();
    private GameController gameController;


    //[SerializeField] private bool activated = false;

    void Start()
    {
        var player = GameObject.Find("Player");
        playerTransform = player.transform;
        grid = player.GetComponent<GridController>();
        audioSource = GetComponent<AudioSource>();
        gameController = player.GetComponent<GameController>();
        home = grid.GetGridCellFromPosition(transform.position);
    }

    void Update()
    {
        if (gameController.gameActive)
        {
            if (cooldown <= 0f)
            {
                if (mirrored.HasValue)
                    RunStunAnimation(false);
                if (!goHome)
                {
                    SeePlayer();
                    HearPlayer();
                }
                else if(grid.GetGridCellFromPosition(transform.position) == home)
                {
                    goHome = false;
                    neverPlayedAudio = true;
                }
            }
            else
            {
                cooldown -= Time.deltaTime;
            }

            Movement(cooldown <= 0f || !mirrored.HasValue);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.CompareTag("Wall") && destinationQueue.Any())
    //    {
    //        var target  = destinationQueue.Last();
    //        destinationQueue = grid.GetPath(transform.position, target);
    //    }

    //}

    private void SeePlayer()
    {
        Vector3 direction = (this.transform.position - playerTransform.position).normalized * -1;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                var gridCellPosition = grid.GetGridCellFromPosition(hit.point);
                if(!destinationQueue.Any() || destinationQueue.LastOrDefault() != gridCellPosition)
                {
                    if (destinationQueue.Any()) {
                       if (Vector2Int.Distance(gridCellPosition, destinationQueue.LastOrDefault()) <= 1)
                            destinationQueue.Clear();
                        destinationQueue.Add(gridCellPosition);
                    }
                    else if(Vector2Int.Distance(gridCellPosition, grid.GetGridCellFromPosition(transform.position)) > 1)
                    {
                        destinationQueue = grid.GetPath(transform.position, gridCellPosition);
                    }
                    if (neverPlayedAudio)
                    {
                        neverPlayedAudio = false;
                        audioSource.clip = activationSound;
                        audioSource.Play();
                    }
                    activated = true;
                }
            }
        }
    }

    private void HearPlayer()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < earShot)
        {
            bool playerFireInput = Input.GetButtonDown("Fire1");
            if (playerFireInput)
            {
                activated = true;
                destinationQueue = grid.GetPath(transform.position, grid.GetGridCellFromPosition(playerTransform.position));
            }
        }
    }

    private void Movement(bool forward)
    {
        if (activated && destinationQueue.Any())
        {
            var target = grid.GetGridCellPosition(destinationQueue.FirstOrDefault());
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime * (forward ? 1f : -0.5f));
            if(transform.position == target)
                destinationQueue.RemoveAt(0);
        } 
    }

    private void SendHome()
    {
        goHome = true;
        destinationQueue = grid.GetPath(transform.position, home);
    }

    public bool IsActive()
    {
        return activated && cooldown <= 0f;
    }

    public void TriggerStun(bool wasAttacked)
    {
        if (cooldown <= 0f)
        {

            if (!wasAttacked)
            {
                audioSource.clip = biteSound;
                audioSource.Play();
                SendHome();
            }
            else
            {
                cooldown = cooldownTime;
                RunStunAnimation(wasAttacked);
                audioSource.clip = stunSound;
                audioSource.Play();
            }
        }
    }

    private void RunStunAnimation(bool stun)
    {
        var activateStun = !mirrored.HasValue;
        var monkey = transform.GetChild(0);
        //animator.SetBool("Stun", runAnimation);
        if (activateStun)
        {
            baseRotate = monkey.rotation;
            mirrored = Random.Range(0, 2) == 0;
            monkey.Rotate(new Vector3(15f * (stun ? -1f : 0f), 15f * (mirrored.Value ? 1f : -1f), 0f));
        }
        else
        {
            monkey.rotation = baseRotate;
            mirrored = null;
        }
    }
}
