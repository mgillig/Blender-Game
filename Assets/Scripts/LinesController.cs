using UnityEngine;

public class SkyboxGrid : MonoBehaviour
{
    public GameObject gridLineX;
    public GameObject gridLineY;
    public int gridLineHeight = 0;
    void Start()
    {
        for(int i = 2; i <= 100; i+=2)
        {
            Instantiate(gridLineX, new Vector3(0, gridLineHeight, i), new Quaternion());
            Instantiate(gridLineX, new Vector3(0, gridLineHeight, i * -1), new Quaternion());
            Instantiate(gridLineY, new Vector3(i, gridLineHeight, 0), new Quaternion());
            Instantiate(gridLineY, new Vector3(i * -1, gridLineHeight, 0), new Quaternion());
        }
    }
}
