using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Design.Serialization;
using Unity.Burst.CompilerServices;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float cooldown = 0f;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float earShot;
    [SerializeField] private float speed;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private AudioClip biteSound;
    [SerializeField] private AudioClip stunSound;
    [SerializeField] private Vector2Int home;
    [SerializeField] private bool hungy = false;
    [SerializeField] private bool goHome = false;
    [SerializeField] private Animator animator;

    private AudioSource audioSource;
    private Transform playerTransform;
    private bool neverPlayedAudio = true;
    private GridController grid;
    private List<Vector2Int> destinationQueue = new List<Vector2Int>();
    private GameController gameController;
    private PlayerController playerController;
    private bool gameStart = false;
    private GameObject monkey;
    
    //debug properties
    public bool selected = false;

    void Start()
    {
        var player = GameObject.Find("Player");
        var debugPlayer = GameObject.Find("debug_Player");
        grid = player.GetComponent<GridController>();
        audioSource = GetComponent<AudioSource>();
        gameController = player.GetComponent<GameController>();
        playerController = player.GetComponent<PlayerController>();
        monkey = transform.GetChild(0).gameObject;

        playerTransform = player != null ? player.transform : debugPlayer.transform;
        home = new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }

    void Update()
    {
        if (gameController.gameActive)
        {
            if (gameController.gameStarted && gameStart)
            {
                if (goHome && grid.GetGridCellFromPosition(transform.position) == home)
                {
                    cooldown = 0f;
                    goHome = false;
                    neverPlayedAudio = true;
                }
                if (cooldown <= 0f)
                {
                    if (!goHome)
                    {
                        SeePlayer();
                    }
                    HearPlayer();
                }
                else
                {
                    cooldown -= Time.deltaTime;
                }

                if (destinationQueue.Any())
                    Movement();
            }
            else if (gameController.gameStarted && !gameStart && Input.anyKeyDown)
            {
                gameStart = true;
            }

            if (!destinationQueue.Any() && !gameController.debugMode)
                transform.rotation = Quaternion.LookRotation((this.transform.position - playerTransform.position).normalized * -1);
        }
    }

    public void Respawn()
    {
        transform.position = new Vector3(home.x, transform.position.y, home.y);
        neverPlayedAudio = true;
        cooldown = cooldownTime;
        animator.SetTrigger("Respawn");
    }

    private void OnMouseOver()
    {
        if(gameController.debugMode && Input.GetButtonDown("Fire1"))
        {
            selected = true;
            monkey.transform.Rotate(-90f, 0f, 0f);
            cooldown = cooldownTime;
        }
    }

    private void SeePlayer()
    {
        if ((gameController.debugMode && !selected) || !gameController.debugMode)
        {
            Vector3 direction = (this.transform.position - playerTransform.position).normalized * -1;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    var gridCellPosition = grid.GetGridCellFromPosition(hit.point);
                    if (!destinationQueue.Any() || destinationQueue.LastOrDefault() != gridCellPosition)
                    {
                        var currentPosition = grid.GetGridCellFromPosition(transform.position);
                        print("I am at " + currentPosition + " and am looking at the player at " + gridCellPosition);

                        if(destinationQueue.Any())
                        {
                            if (destinationQueue.Contains(gridCellPosition))
                            {
                                var index = destinationQueue.IndexOf(gridCellPosition) + 1;
                                if (index != destinationQueue.Count)
                                    destinationQueue.RemoveRange(index, destinationQueue.Count - index);
                            }
                            else if(destinationQueue.Any(x => Vector2Int.Distance(x, gridCellPosition) <= 1))
                            {
                                var index = destinationQueue.IndexOf(destinationQueue.FirstOrDefault(x => Vector2Int.Distance(x, gridCellPosition) <= 1)) + 1;
                                if (index != destinationQueue.Count)
                                    destinationQueue.RemoveRange(index, destinationQueue.Count - index);
                                destinationQueue.Add(gridCellPosition);
                            }
                            else
                            {
                                destinationQueue.Clear();
                                destinationQueue = grid.GetPath(transform.position, gridCellPosition);
                            }
                        }
                        else
                        {
                            destinationQueue = grid.GetPath(transform.position, gridCellPosition);
                        }

                        if (neverPlayedAudio)
                        {
                            neverPlayedAudio = false;
                            audioSource.clip = activationSound;
                            audioSource.loop = false;
                            audioSource.Play();
                        }
                        hungy = true;
                    }
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
        if (Vector3.Distance(transform.position, playerTransform.position) < earShot && !gameController.debugMode)
        {
            bool playerFireInput = Input.GetButtonDown("Fire1");
            if (playerFireInput && playerController.enableFire)
            {
                hungy = true;
                var playerGridCellPosition = grid.GetGridCellFromPosition(playerTransform.position);
                var currentPosition = grid.GetGridCellFromPosition(transform.position);
                print("I am at " + currentPosition + " and hear the player at " + playerGridCellPosition);
                destinationQueue = grid.GetPath(transform.position, playerGridCellPosition);
            }
        }
        else if (targetGridCell.HasValue)
        {
            hungy = true;
            var currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            print("I am at " + currentPosition + " and hear the player at " + targetGridCell.Value + " (" + Vector2Int.Distance(currentPosition, targetGridCell.Value));
            destinationQueue = grid.GetPath(transform.position, targetGridCell.Value);
            selected = false; 
            transform.GetChild(0).Rotate(90f, 0f, 0f);
        }
    }

    private void Movement()
    {
        var target = new Vector3(destinationQueue.FirstOrDefault().x, transform.position.y, destinationQueue.FirstOrDefault().y);
        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
        {
            destinationQueue.RemoveAt(0);
            if (gameController.debugMode && !destinationQueue.Any() && grid.GetGridCellFromPosition(transform.position) != home)
                SendHome();
        }
    }

    private void SendHome()
    {
        cooldown = cooldownTime;
        goHome = true;
        hungy = false;
        neverPlayedAudio = true;
        destinationQueue = grid.GetPath(transform.position, home);
    }

    public bool IsActive()
    {
        return hungy && cooldown <= 0f;
    }

    public void TriggerStun(bool wasAttacked)
    {
        if (!wasAttacked && cooldown <= 0f)
        {
            audioSource.clip = biteSound;
            audioSource.loop = false;
            audioSource.Play();
            SendHome();
        }
        else
        {
            audioSource.clip = stunSound;
            audioSource.loop = false;
            audioSource.Play();
            destinationQueue.Clear();
            cooldown = cooldownTime;
            hungy = false;
            animator.SetTrigger("Kill");
        }
    }
}
