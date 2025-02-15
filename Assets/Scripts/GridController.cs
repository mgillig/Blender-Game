using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts;
using JetBrains.Annotations;
using UnityEngine;

public class GridController : MonoBehaviour
{
    //public GameObject gridCell;
    public GameObject wall;
    public GameObject enemy;
    public GameObject victory;
    public int gridSizeX;
    public int gridSizeZ;
    public int enemySpawnRate;
    public int lightFrequency;
    private GridCellModel[,] grid;
    private List<Vector2Int> availableCells;
    public GameObject victoryScreen;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridCellModel[gridSizeX, gridSizeZ];
        GenerateMaze();
        //InstantiateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Victory"))
        {
            victoryScreen.transform.GetChild(0).gameObject.SetActive(true);
            victoryScreen.transform.GetChild(1).gameObject.SetActive(true);
            victoryScreen.GetComponent<TimerController>().SetFinalTime();
        }
    }

    public void InstantiateGrid()
    {
        int centerX = grid.GetLength(0) / 2;
        int centerZ = grid.GetLength(1) / 2;
        float wallSize = wall.transform.localScale.x;
        int victorySide = Random.Range(0, 4);

        for (int z = 0; z < grid.GetLength(1); z++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                if (x > (gridSizeX / 2) - 1 && x < (gridSizeX / 2) + 1 && z > (gridSizeZ / 2) - 1 && z < (gridSizeZ / 2) + 1)
                    continue;
                var gridCell = grid[x, z];
                var cellLocation = new Vector3((x - centerX) * wallSize, 1, (z - centerZ) * wallSize);
                //build walls
                //S and W walls
                if (gridCell.SWallActive)
                {
                    wall.name = "wall (" + x + ", " + z + ") S";
                    var wallLocation = new Vector3(cellLocation.x, cellLocation.y, cellLocation.z - (wallSize / 2));
                    var wallRotation = new Quaternion(0f, z % 2 == 0 ? 0f : 180f, 0f, 0f);
                    Instantiate(wall, wallLocation, wallRotation);
                }
                if (gridCell.WWallActive)
                {
                    wall.name = "wall (" + x + ", " + z + ") W";
                    var wallLocation = new Vector3(cellLocation.x - (wallSize / 2), cellLocation.y, cellLocation.z);
                    //var wallRotation = new Quaternion(0f, 0f, 0f, 0f);
                    var newWall = Instantiate(wall, wallLocation, new Quaternion());
                    newWall.transform.Rotate(new Vector3(0f, x % 2 == 0 ? 90f : -90f, 0f));
                }

                //N and E walls
                //only used on last cell of row and column since the N and E walls are the S and W walls of the next cell
                if (z == grid.GetLength(1) - 1 && gridCell.NWallActive)
                {
                    wall.name = "wall (" + x + ", " + z + ") N";
                    var wallLocation = new Vector3(cellLocation.x, cellLocation.y, cellLocation.z + (wallSize / 2));
                    var wallRotation = new Quaternion(0f, z + 1 % 2 == 0 ? 0f : 180f, 0f, 0f);
                    Instantiate(wall, wallLocation, wallRotation);
                }
                if (x == grid.GetLength(0) - 1 && gridCell.EWallActive)
                {
                    wall.name = "wall (" + x + ", " + z + ") E";
                    var wallLocation = new Vector3(cellLocation.x + (wallSize / 2), cellLocation.y, cellLocation.z);
                    //var wallRotation = new Quaternion(0f, 0f, 0f, 0f);
                    var newWall = Instantiate(wall, wallLocation, new Quaternion());
                    newWall.transform.Rotate(new Vector3(0f, x + 1 % 2 == 0 ? 90f : -90f, 0f));
                }

                

                //Set Victory
                if ((victorySide == 0 && x == 0 && z == 0) || 
                    (victorySide == 1 && x == 0 && z == grid.GetLength(1) - 1) ||
                    (victorySide == 2 && x == grid.GetLength(0) - 1 && z == 0) || 
                    (victorySide == 3 && x == grid.GetLength(0) - 1 && z == grid.GetLength(1) - 1) )
                {
                    Instantiate(victory, cellLocation, new Quaternion());
                    continue;
                }

                //Set Enemies
                if ((gridCell.NWallActive ? 1 : 0) + (gridCell.SWallActive ? 1 : 0) + (gridCell.EWallActive ? 1 : 0) + (gridCell.WWallActive ? 1 : 0) == 3 &&
                    (x > (gridSizeX / 2) + 1 || x < (gridSizeX / 2) - 1 || z > (gridSizeZ / 2) + 1 || z < (gridSizeZ / 2) - 1) &&
                    Random.Range(0, enemySpawnRate) == 0)
                {
                    Instantiate(enemy, cellLocation, new Quaternion());
                }

                ////Build Lights

                //if (gridTemp.SWallActive)
                //{
                //    if (lightCounter <= 0)
                //    {
                //        cellTemp.transform.GetChild(6).gameObject.SetActive(true);
                //        lightCounter = Random.Range(lightFrequency - 1, lightFrequency + 2);
                //    }
                //    else
                //        lightCounter--;
                //}
            }
        }
    }

    private void GenerateMaze()
    {
        var currentCell = new Vector2Int(grid.GetLength(0) / 2, grid.GetLength(1) / 2);
        grid[currentCell.x, currentCell.y] = new GridCellModel()
        {
            IsFrontier = false
        };

        availableCells = new List<Vector2Int>()
        {
            currentCell
        };

        var neighbors = FindFrontier(currentCell);

        GenerateMaze(currentCell, neighbors);
    }

    private void GenerateMaze(Vector2Int currentCell, List<Vector2Int> eligibleNeighbors)
    {
        if (eligibleNeighbors.Any())
        {
            var frontierToAdd = eligibleNeighbors[Random.Range(0, eligibleNeighbors.Count)];
            availableCells.Add(frontierToAdd);
            eligibleNeighbors.Remove(frontierToAdd);
            if (!eligibleNeighbors.Any())
                availableCells.Remove(currentCell);

            var newCell = new GridCellModel()
            {
                IsFrontier = false
            };

            if(frontierToAdd.x > ((float)grid.GetLength(0) / 2f) - 2f && frontierToAdd.x < ((float)grid.GetLength(0) / 2f) + 2f &&
                frontierToAdd.y > ((float)grid.GetLength(1) / 2f) - 2f && frontierToAdd.y < ((float)grid.GetLength(1) / 2f) + 2f)
            {
                newCell.SWallActive = false;
                newCell.NWallActive = false;
                newCell.EWallActive = false;
                newCell.WWallActive = false;
            }
            else
            {
                //remove currentCell.nWall & newCell.sWall: y+1
                if (currentCell.y + 1 == frontierToAdd.y)
                {
                    grid[currentCell.x, currentCell.y].NWallActive = false;
                    newCell.SWallActive = false;
                }
                //remove currentCell.sWall & newCell.nWall: y-1
                else if (currentCell.y - 1 == frontierToAdd.y)
                {
                    grid[currentCell.x, currentCell.y].SWallActive = false;
                    newCell.NWallActive = false;
                }
                //remove currentCell.eWall & newCell.wWall: x+1
                else if (currentCell.x + 1 == frontierToAdd.x)
                {
                    grid[currentCell.x, currentCell.y].EWallActive = false;
                    newCell.WWallActive = false;
                }
                //remove currentCell.wWall & newCell.eWall: x-1
                else if (currentCell.x - 1 == frontierToAdd.x)
                {
                    grid[currentCell.x, currentCell.y].WWallActive = false;
                    newCell.EWallActive = false;
                }
            }

            grid[frontierToAdd.x, frontierToAdd.y] = newCell;
        }
        if (availableCells.Any())
        {
            currentCell = availableCells[Random.Range(0, availableCells.Count)];
            eligibleNeighbors = FindFrontier(currentCell);

            if (!eligibleNeighbors.Any())
            {
                availableCells.Remove(currentCell);
                foreach (var cell in availableCells.ToList())
                {
                    currentCell = cell;
                    eligibleNeighbors = FindFrontier(currentCell);
                    if (!eligibleNeighbors.Any())
                        availableCells.Remove(currentCell);
                    else
                        break;
                }
            }

            GenerateMaze(currentCell, eligibleNeighbors);
        }
    }
    private List<Vector2Int> FindFrontier(Vector2Int currentCell)
    {
        var neighbors = new List<Vector2Int>();
        var nNeighbor = new Vector2Int(currentCell.x, currentCell.y + 1);
        var sNeighbor = new Vector2Int(currentCell.x, currentCell.y - 1);
        var eNeighbor = new Vector2Int(currentCell.x + 1, currentCell.y);
        var wNeighbor = new Vector2Int(currentCell.x - 1, currentCell.y);

        //nNeighbor
        if (nNeighbor.y < gridSizeZ && (grid[nNeighbor.x, nNeighbor.y] == null || grid[nNeighbor.x, nNeighbor.y].IsFrontier))
        {
            neighbors.Add(nNeighbor);
            if (grid[nNeighbor.x, nNeighbor.y] == null)
                grid[nNeighbor.x, nNeighbor.y] = new GridCellModel();
        }
        //sNeighbor
        if (sNeighbor.y >= 0 && (grid[sNeighbor.x, sNeighbor.y] == null || grid[sNeighbor.x, sNeighbor.y].IsFrontier))
        {
            neighbors.Add(sNeighbor);
            if (grid[sNeighbor.x, sNeighbor.y] == null)
                grid[sNeighbor.x, sNeighbor.y] = new GridCellModel();
        }
        //eNeighbor
        if (eNeighbor.x < gridSizeX && (grid[eNeighbor.x, eNeighbor.y] == null || grid[eNeighbor.x, eNeighbor.y].IsFrontier))
        {
            neighbors.Add(eNeighbor);
            if (grid[eNeighbor.x, eNeighbor.y] == null)
                grid[eNeighbor.x, eNeighbor.y] = new GridCellModel();
        }
        //wNeighbor
        if (wNeighbor.x >= 0 && (grid[wNeighbor.x, wNeighbor.y] == null || grid[wNeighbor.x, wNeighbor.y].IsFrontier))
        {
            neighbors.Add(wNeighbor);
            if (grid[wNeighbor.x, wNeighbor.y] == null)
                grid[wNeighbor.x, wNeighbor.y] = new GridCellModel();
        }

        return neighbors;
    }
}
