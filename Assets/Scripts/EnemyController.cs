using System.ComponentModel.Design.Serialization;
using UnityEditor.PackageManager;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float cooldownTime = 5f;
    public Transform playerTransform;
    public Animator animator;

    private Vector3 pathTarget;
    public float cooldown = 0f;
    private bool activated = false;
    private bool? mirrored = null;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (cooldown <= 0f)
        {
            if (mirrored.HasValue)
                RunStunAnimation(false);

            Vector3 direction = (this.transform.position - playerTransform.position).normalized * -1;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    if (!activated)
                    {
                        activated = true;
                    }
                    pathTarget = hit.point;
                    transform.LookAt(pathTarget);
                }
            }
            //pathfinding goes here
        }
        else
            cooldown -= Time.deltaTime;

        //hear player on Fire
        if (activated)
        {
            bool playerFireInput = Input.GetButtonDown("Fire1");
            if (playerFireInput)
            {
                pathTarget = playerTransform.position;
                transform.LookAt(pathTarget);
            }
        }
    }

    public bool IsActive()
    {
        return activated && cooldown <= 0f;
    }

    public void TriggerStun(bool wasAttacked)
    {
        if (cooldown <= 0f)
        {
            cooldown = cooldownTime;
            RunStunAnimation(wasAttacked);
        }
    }

    private void RunStunAnimation(bool runAnimation)
    {
        var activateStun = !mirrored.HasValue;
        var monkey = transform.GetChild(0);
        animator.SetBool("Stun", runAnimation);
        if (activateStun)
        {
            mirrored = Random.Range(0, 2) == 0;
            monkey.Rotate(new Vector3(15f, 15f * (mirrored.Value ? 1 : -1), 0f));
        }
        else
        {
            monkey.Rotate(new Vector3(-15f, 15f * (!mirrored.Value ? 1 : -1), 0f));
            mirrored = null;
        }
    }
}
