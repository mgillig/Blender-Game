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
                gameController.Win();
            audioSource.loop = false;
            audioSource.Play();
            hasWon = true;
        }
    }
}
