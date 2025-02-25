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
            var playerController = other.GetComponent<PlayerController>();
            var mouseController = other.GetComponent<MouseController>();
            if (playerController != null)
            {
                playerController.enableMove = true;
                playerController.enableFire = true;
                mouseController.SetMouseMode(false, false);
            }
            var victoryScreen = other.gameObject.transform.GetChild(1).GetChild(1);
            victoryScreen.GetChild(0).gameObject.SetActive(true);
            victoryScreen.GetChild(1).gameObject.SetActive(true);
            victoryScreen.GetComponent<TimerController>().SetFinalTime();
            audioSource.Play();
            hasWon = true;
        }
    }
}
