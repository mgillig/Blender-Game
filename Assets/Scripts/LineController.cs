using UnityEngine;

public class LineController : MonoBehaviour
{
    private GameObject player;
    private GameObject line;
    private bool isXLine;
    public int drawDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        line = transform.GetChild(0).gameObject;
        isXLine = transform.GetComponentInChildren<LineRenderer>().GetPosition(0).x != 0;
    }

    // Update is called once per frame
    void Update()
    {
        //line.SetActive((isXLine && Mathf.Abs(player.transform.position.z - transform.position.z) < drawDistance) ||
        //              (!isXLine && Mathf.Abs(player.transform.position.x - transform.position.x) < drawDistance));
        //if (isXLine)
        //    line.transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, (transform.position.z - player.transform.position.z) * 0.1f), transform.rotation);
        //else
        //    line.transform.SetLocalPositionAndRotation(new Vector3((transform.position.x - player.transform.position.x) * 0.1f, 0f, 0f), transform.rotation);
    }
}
