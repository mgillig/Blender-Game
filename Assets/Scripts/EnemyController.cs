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
    private PlayerController playerController;
    
    //debug properties
    public bool selected = false;


    //[SerializeField] private bool activated = false;

    void Start()
    {
        var player = GameObject.Find("Player");
        playerTransform = player.transform;
        grid = player.GetComponent<GridController>();
        audioSource = GetComponent<AudioSource>();
        gameController = player.GetComponent<GameController>();
        playerController = player.GetComponent<PlayerController>();
        home = new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }

    void Update()
    {
        if (gameController.gameActive)
        {
            if(!gameController.debugMode)
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
                    //else if(grid.GetGridCellFromPosition(transform.position) == home)
                    else if (new Vector2Int((int)transform.position.x, (int)transform.position.z) == home)
                    {
                        goHome = false;
                        neverPlayedAudio = true;
                    }
                }
                else
                {
                    cooldown -= Time.deltaTime;
                }
            }

            Movement(cooldown <= 0f || !mirrored.HasValue || gameController.debugMode);
        }
    }

    private void OnMouseOver()
    {
        if(gameController.debugMode && Input.GetButtonDown("Fire1"))
        {
            selected = true;
            transform.GetChild(0).Rotate(-90f, 0f, 0f);
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
                //var gridCellPosition = grid.GetGridCellFromPosition(hit.point);
                var gridCellPosition = new Vector2Int((int)hit.point.x, (int)hit.point.z);
                if(!destinationQueue.Any() || destinationQueue.LastOrDefault() != gridCellPosition)
                {
                    var currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
                    print("I am at " + currentPosition + " and am looking at the player at " + gridCellPosition);
                    if (destinationQueue.Any() && Vector2Int.Distance(destinationQueue.LastOrDefault(), currentPosition) > Vector2Int.Distance(gridCellPosition, currentPosition))
                        destinationQueue.Clear();
                    if (!destinationQueue.Any() && Vector2Int.Distance(gridCellPosition, currentPosition) > 1)
                        destinationQueue = grid.GetPath(transform.position, gridCellPosition);
                    else
                        destinationQueue.Add(gridCellPosition);

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

    //for debug
    public void DebugHearPlayer(Vector2Int targetGridCell)
    {
        HearPlayer(targetGridCell);
    }

    private void HearPlayer(Vector2Int? targetGridCell = null)
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < earShot && !targetGridCell.HasValue)
        {
            bool playerFireInput = Input.GetButtonDown("Fire1");
            if (playerFireInput && playerController.enableFire)
            {
                activated = true;
                //var playerGridCellPosition = grid.GetGridCellFromPosition(playerTransform.position);
                //var currentPosition = grid.GetGridCellFromPosition(transform.position);
                var playerGridCellPosition = new Vector2Int((int)playerTransform.position.x, (int)playerTransform.position.z);
                var currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
                print("I am at " + currentPosition + " and hear the player at " + playerGridCellPosition);
                //print(playerGridCellPosition);
                destinationQueue = grid.GetPath(transform.position, playerGridCellPosition);
            }
        }
        else if (targetGridCell.HasValue)
        {
            activated = true;
            var currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            print("I am at " + currentPosition + " and hear the player at " + targetGridCell.Value);
            destinationQueue = grid.GetPath(transform.position, targetGridCell.Value);
            selected = false; 
            transform.GetChild(0).Rotate(90f, 0f, 0f);
        }
    }

    private void Movement(bool forward)
    {
        if (activated && destinationQueue.Any())
        {
            //var target = grid.GetGridCellPosition(destinationQueue.FirstOrDefault());
            var target = new Vector3(destinationQueue.FirstOrDefault().x, 0f, destinationQueue.FirstOrDefault().y);
            //transform.LookAt(target);
            //transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime * (forward ? 1f : -0.5f));
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
