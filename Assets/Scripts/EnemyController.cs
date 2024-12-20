using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool activated = false;
    public float cooldownTime = 5f;
    private float cooldown = 0f;
    public Vector3 pathTarget;
    public Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (cooldown <= 0f)
        {
            Vector3 direction = (this.transform.position - playerTransform.position).normalized * -1;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                pathTarget = hit.point;
                if (hit.collider.gameObject.tag == "Player")
                {
                    if (!activated)
                    {
                        activated = true;
                    }
                }
            }
        }
        else
            cooldown -= Time.deltaTime;
    }

    public void TriggerCooldown()
    {
        cooldown = cooldownTime;
    }
}
