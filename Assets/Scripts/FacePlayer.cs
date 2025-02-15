using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform playerPosition;

    void Start()
    {
        playerPosition = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerPosition.position);
    }
}
