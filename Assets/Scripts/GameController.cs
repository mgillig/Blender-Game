using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public float playerDropSpeed;
    private Animator playerAnimationController;
    private CharacterController characterController;
    private GridController gridController;
    private MouseController mouseController;
    private PlayerController playerController;
    private bool start = false;
    private float turnSmoothVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerAnimationController = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        gridController = GetComponent<GridController>();
        mouseController = GetComponent<MouseController>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (player.transform.rotation.x > 0)
            {
                var angle = Mathf.SmoothDamp(player.transform.rotation.x, 0f, ref turnSmoothVelocity, playerDropSpeed);
                player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
    }


    public void triggerStart()
    {
        start = true;
        gridController.InstantiateGrid();
        mouseController.SetMouseMode(true, true);
        playerController.enableMove = true;
        playerController.gameStart = true;
        characterController.Move(new Vector3(0f, (player.transform.position.y - 1f) * -1f, 0f));


    }
}
