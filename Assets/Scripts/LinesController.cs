using UnityEngine;

public class SkyboxGrid : MonoBehaviour
{
    public GameObject gridLineX;
    public GameObject gridLineY;
    void Start()
    {
        for(int i = 1; i <= 50; i++)
        {
            Instantiate(gridLineX, new Vector3(0, -1, i), new Quaternion());
            Instantiate(gridLineX, new Vector3(0, -1, i * -1), new Quaternion());
            Instantiate(gridLineY, new Vector3(i, -1, 0), new Quaternion());
            Instantiate(gridLineY, new Vector3(i * -1, -1, 0), new Quaternion());
        }
    }
}
