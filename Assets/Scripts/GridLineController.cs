using UnityEngine;

public class GridLineController : MonoBehaviour
{
    [SerializeField] private GameObject gridLineX;
    [SerializeField] private GameObject gridLineY;
    [SerializeField] private float spacing;
    void Start()
    {
        for (float i = spacing; i <= 100; i += spacing)
        {
            Instantiate(gridLineX, new Vector3(0, transform.position.y, i), new Quaternion(), transform);
            Instantiate(gridLineX, new Vector3(0, transform.position.y, i * -1), new Quaternion(), transform);
            Instantiate(gridLineY, new Vector3(i, transform.position.y, 0), new Quaternion(), transform);
            Instantiate(gridLineY, new Vector3(i * -1, transform.position.y, 0), new Quaternion(), transform);
        }
    }
}
