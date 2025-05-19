using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject menu;
    public bool debugMode;
    public bool gameStarted;
    public bool gameActive;

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


        debugMode = false;
        gameStarted = false;
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;


        if (debugMode)
        {
            TriggerGameStart();
            mouseController.SetMouseMode(false, false);
        }
    }

    void Update()
    {
        if(gameStarted)
        {
            bool toggleMenu = Input.GetButtonDown("Pause");
            if (toggleMenu && gameActive)
                PauseGame();
            else if(toggleMenu && !gameActive)
                ResumeGame();
        }
    }


    public void TriggerGameStart()
    {
        gridController.ActivateGrid();
        if(gridLines != null)
            gridLines.transform.Translate(0f, gridLines.transform.position.y * -1, 0f);
        gameStarted = true;
        ResumeGame();
    }

    public void PauseGame()
    {
        menu.SetActive(true);
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;
        mouseController.SetMouseMode(false, false);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gameActive = true;
        menu.SetActive(false);
        mouseController.SetMouseMode(gameStarted, true);
        playerController.enableMove = gameStarted;
        playerController.enableFire = true;
        Time.timeScale = 1f;
    }

    public void ResetGame()
    {
        print("I AM HERE");
        ResumeGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
