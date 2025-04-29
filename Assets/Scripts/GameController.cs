using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool debugMode = false;
    public bool gameActive = false;
    //[SerializeField] private GameObject player;
    private CharacterController characterController;
    private GridController gridController;
    private MouseController mouseController;
    private PlayerController playerController;
    private GameObject gridLines;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerAnimationController = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        gridController = GetComponent<GridController>();
        mouseController = GetComponent<MouseController>();
        playerController = GetComponent<PlayerController>();
        gridLines = GameObject.Find("GridLines");
        if (debugMode)
        {
            TriggerStart();
            playerController.enableMove = false;
            playerController.enableFire = false;
            mouseController.SetMouseMode(false, false);
        }
    }


    public void TriggerStart()
    {
        gameActive = true;
        gridController.ActivateGrid();
        mouseController.SetMouseMode(true, true);
        playerController.enableMove = true;
        //characterController.Move(new Vector3(0f, (player.transform.position.y - 1f) * -1f, 0f));
        if(gridLines != null)
            gridLines.transform.Translate(0f, gridLines.transform.position.y * -1, 0f);

    }

    public void PauseGame()
    {
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;
        mouseController.SetMouseMode(false, false);
    }
}
