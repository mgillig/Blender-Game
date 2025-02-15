using UnityEngine;

public class DefaultCubeController : MonoBehaviour
{
    private GameController gameStart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameStart = GameObject.Find("Player").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerStart()
    {
        gameStart.triggerStart();
        this.gameObject.SetActive(false);
    }
}
