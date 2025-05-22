using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Sprite pauseUI;
    [SerializeField] private Sprite resumeUI;
    [SerializeField] private GameObject resumeButton;
    public bool debugMode;
    public bool gameStarted;
    public bool gameActive;

    private CharacterController characterController;
    private GridController gridController;
    private MouseController mouseController;
    private PlayerController playerController;
    private GameObject gridLines;
    private bool firstLoad;


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
        //gameActive = true;
        playerController.enableMove = false;
        playerController.enableFire = true;
        firstLoad = true;
        mouseController.SetMouseMode(gameStarted, true);

        //pauseUI = Resources.Load<Sprite>("Sprites/BlenderUi_Pause");
        //resumeUI = Resources.Load<Sprite>("Sprites/BlenderUi");

        if (pauseUI == null)
            print("cannot find pauseUI");
        if (resumeUI == null)
            print("cannot find resumeUI");

        if (debugMode)
        {
            TriggerGameStart();
            mouseController.SetMouseMode(false, false);
        }
    }

    void Update()
    {
        if (firstLoad)
        {
            playerController.EquipShotgun(true);
            firstLoad = false;
        }

        bool toggleMenu = Input.GetButtonDown("Pause");
        if (toggleMenu && gameActive)
            PauseGame();
        else if (toggleMenu && !gameActive && resumeButton.activeInHierarchy)
            ResumeGame();
    }


    public void TriggerGameStart()
    {
        gridController.ActivateGrid();
        if(gridLines != null)
            gridLines.transform.Translate(0f, gridLines.transform.position.y * -1, 0f);
        gameStarted = true;
        resumeButton.SetActive(true);
        ResumeGame();
    }

    public void PauseGame()
    {
        menu.SetActive(true);
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;
        mouseController.SetMouseMode(false, false);
        //shotgun.SetActive(false);
        playerController.EquipShotgun(false);
        canvas.GetComponent<Image>().sprite = pauseUI;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gameActive = true;
        menu.SetActive(false);
        mouseController.SetMouseMode(gameStarted, true);
        playerController.enableMove = gameStarted;
        playerController.enableFire = true;
        //shotgun.SetActive(true);
        playerController.EquipShotgun(true);
        canvas.GetComponent<Image>().sprite = resumeUI;
        Time.timeScale = 1f;
    }

    public void ResetGame()
    {
        //print("I AM HERE");
        if (gameStarted)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
            ResumeGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Die()
    {

    }
}
