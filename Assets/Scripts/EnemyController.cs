using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool activated = false;
    public bool idle = true;
    public Vector3 pathTarget;

    void Start()
    {

    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            if (hit.collider.gameObject.tag != "Wall")
                print("hehehe I see you");
            //print(hit.collider.gameObject.tag);
        }
    }
}
