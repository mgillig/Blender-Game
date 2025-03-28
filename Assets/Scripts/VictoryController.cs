using UnityEngine;

public class VictoryController : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasWon = false;
    //public GameObject victoryScreen;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            var gameController = other.GetComponent<GameController>();
            if (gameController != null)
                gameController.PauseGame();
            var victoryScreen = other.gameObject.transform.GetChild(1).GetChild(1);
            victoryScreen.GetChild(0).gameObject.SetActive(true);
            victoryScreen.GetChild(1).gameObject.SetActive(true);
            victoryScreen.GetComponent<TimerController>().SetFinalTime();
            audioSource.Play();
            hasWon = true;
        }
    }
}
