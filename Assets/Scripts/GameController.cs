using System.Collections;
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
    [SerializeField] private GameObject defaultCube;
    [SerializeField] private GameObject resetConfirmationBox;
    [SerializeField] private GameObject quitConfirmationBox;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private GameObject controlsMenu;
    public bool debugMode;
    public bool gameStarted;
    public bool gameActive;
    private bool gameEnded;

    private PlayerHealth healthBar;
    private GridController gridController;
    private MouseController mouseController;
    private PlayerController playerController;
    private GameObject gridLines;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private GameObject deathScreen;
    private GameObject victoryScreen;
    private TimerController timerController;
    private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridController = GetComponent<GridController>();
        mouseController = GetComponent<MouseController>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        gridLines = GameObject.Find("GridLines");
        healthBar = GetComponentInChildren<PlayerHealth>();
        deathScreen = canvas.transform.Find("DeathScreen").gameObject;
        victoryScreen = canvas.transform.Find("VictoryScreen").gameObject;
        timerController = victoryScreen.GetComponent<TimerController>();

        defaultPosition = transform.position;
        defaultRotation = transform.rotation;

        debugMode = false;
        gameStarted = false;
        //gameActive = true;
        playerController.enableMove = false;
        playerController.enableFire = false;
        mouseController.SetMouseMode(gameStarted, false);
        gameEnded = false;


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
        bool toggleMenu = Input.GetButtonDown("Pause");
        if (toggleMenu && gameActive)
            PauseGame();
        if (toggleMenu && gameEnded)
            ResetGame(true);
    }


    public void TriggerGameStart()
    {
        gridController.ActivateGrid();
        if(gridLines != null)
            gridLines.transform.Translate(0f, gridLines.transform.position.y * -1, 0f);
        gameStarted = true;
        resumeButton.SetActive(true); 
        controlsMenu.SetActive(false);
        ResumeGame();
    }

    public void PauseGame()
    {
        menu.SetActive(true);
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;
        mouseController.SetMouseMode(false, false);
        playerController.EquipShotgun(false, false);
        healthBar.DisplayHealthBar(false);
        canvas.GetComponent<Image>().sprite = pauseUI;
        timerController.SetTimerActive(false);
        Time.timeScale = 0f;
    }

    public void ResumeGame(bool firstLoad, bool showControls)
    {
        resetConfirmationBox.SetActive(false);
        quitConfirmationBox.SetActive(false);
        gameActive = true;
        menu.SetActive(false);
        mouseController.SetMouseMode(gameStarted, true);
        playerController.enableMove = gameStarted;
        playerController.enableFire = true;
        playerController.EquipShotgun(true, firstLoad);
        healthBar.DisplayHealthBar(true);
        canvas.GetComponent<Image>().sprite = resumeUI;
        timerController.SetTimerActive(true);
        Time.timeScale = 1f;

        if(showControls)
        {
            controlsMenu.SetActive(true);
            controlsMenu.GetComponent<Animator>().SetFloat("FadeMult", 0.5f);
            controlsMenu.GetComponent<Animator>().SetTrigger("FadeIn");
        }
    }

    public void ResetGame(bool confirmed)
    {
        //print("I AM HERE");
        if (gameStarted && confirmed)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            transform.position = defaultPosition;
            transform.rotation = defaultRotation;
            gridController.instantiateMaze(true);
            defaultCube.SetActive(true);
            gridLines.transform.Translate(0f, -0.12f, 0f);

            gameStarted = false;
            gameActive = true;
            playerController.enableMove = false;
            playerController.enableFire = false;
            mouseController.SetMouseMode(gameStarted, false);
            resumeButton.SetActive(false);
            gameEnded = false;
            Time.timeScale = 1f;
            playerController.ResetHealth();
            SetDeathScreenActive(false);
            SetVictoryScreenActive(false);
            ResumeGame(true, false);
        }
        else if(gameStarted && !confirmed)
        {
            OpenConfirmationBox(true);
        }
        else
            ResumeGame(true, true);
    }

    public void QuitGame(bool confirmed)
    {
        if (confirmed)
            Application.Quit();
        else
            OpenConfirmationBox(false);
    }

    public void AboutUs()
    {
        Application.OpenURL("https://godzilligan.itch.io/");
    }

    public void Confirmation(int confirmation)
    {
        switch (confirmation)
        {
            case 1:
                ResetGame(true);
                break;
            case 2:
                QuitGame(true);
                break;
            default:
                resetConfirmationBox.SetActive(false);
                quitConfirmationBox.SetActive(false);
                break;
        }
    }

    private void OpenConfirmationBox(bool openResetBox)
    {
        var mouseLocation = Input.mousePosition;
        if (openResetBox)
        {
            quitConfirmationBox.SetActive(false);
            resetConfirmationBox.SetActive(true);
            resetConfirmationBox.transform.position = mouseLocation;
        }
        else
        {
            resetConfirmationBox.SetActive(false);
            quitConfirmationBox.SetActive(true);
            quitConfirmationBox.transform.position = mouseLocation;
        }
    }

    public void Die()
    {
        gameEnded = true;
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;
        //Time.timeScale = 0f;
        SetDeathScreenActive(true);
    }

    public void Win()
    {
        gameEnded = true;
        gameEnded = true;
        gameActive = false;
        playerController.enableMove = false;
        playerController.enableFire = false;
        Time.timeScale = 0f;
        SetVictoryScreenActive(true);
    }

    private void SetVictoryScreenActive(bool active)
    {
        victoryScreen.transform.GetChild(0).gameObject.SetActive(active);
        //victoryScreen.transform.GetChild(1).gameObject.SetActive(active);
        if (active)
            timerController.SetFinalTime();
        else
            timerController.ResetTime();
    }

    private void SetDeathScreenActive(bool active)
    {
        deathScreen.transform.GetChild(0).gameObject.SetActive(active);
        deathScreen.transform.GetChild(1).gameObject.SetActive(active);

        if (active)
        {
            audioSource.clip = deathSound;
            audioSource.loop = true;
            audioSource.Play();
            //StartCoroutine(LoadTextbox(deathScreen.transform.GetChild(1).gameObject, 2, true));
            deathScreen.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("FadeIn");
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void ResumeGame()
    {
        ResumeGame(false, false);
    }
}
