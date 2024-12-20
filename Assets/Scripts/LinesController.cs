using UnityEngine;

public class SkyboxGrid : MonoBehaviour
{
    public GameObject gridLineX;
    public GameObject gridLineY;
    void Start()
    {
        for(int i = 1; i <= 100; i+=2)
        {
            Instantiate(gridLineX, new Vector3(0, -5, i), new Quaternion());
            Instantiate(gridLineX, new Vector3(0, -5, i * -1), new Quaternion());
            Instantiate(gridLineY, new Vector3(i, -5, 0), new Quaternion());
            Instantiate(gridLineY, new Vector3(i * -1, -5, 0), new Quaternion());
        }
    }
}
