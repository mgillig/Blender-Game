using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridTextController : MonoBehaviour
{
    public TextMeshPro textMesh;
    private GameController gameController;
    private GameObject[] enemies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameController = GameObject.Find("Player").GetComponent<GameController>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void OnMouseOver()
    {
        if (gameController.debugMode && Input.GetButtonDown("Fire1"))
        {
            //print(textMesh.text);
            if(enemies.Length > 0) 
            {
                var location = new Vector2Int((int)transform.position.x, (int)transform.position.z);
                foreach(var enemy in enemies)
                {
                    var controller = enemy.GetComponent<EnemyController>();
                    if(controller != null && controller.selected)
                    {
                        controller.DebugHearPlayer(location);
                    }
                }
            }
        }
    }
}
