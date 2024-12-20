using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts;
using JetBrains.Annotations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject gridCell;
    public int gridSizeX;
    public int gridSizeZ;
    public int enemySpawnRate;
    public int lightSpawnRate;
    private GridCellModel[,] grid;
    private List<Vector2Int> availableCells;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridCellModel[gridSizeX, gridSizeZ];
        GenerateMaze();
        InstantiateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        //bool mouseInput = Input.GetAxisRaw("Fire1") > 0.0;
        //if (mouseInput)
        //    InstantiateGrid();
    }

    void InstantiateGrid()
    {
        int centerX = grid.GetLength(0) / 2;
        int centerZ = grid.GetLength(1) / 2;
        int cellSizeX = (int)gridCell.transform.localScale.x;
        int cellSizeZ = (int)gridCell.transform.localScale.z;
        int victorySide = Random.Range(0, 4);
        int lightCounter = Random.Range(lightSpawnRate / 3, lightSpawnRate);
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for(int z = 0; z < grid.GetLength(1); z++)
            {
                var gridTemp = grid[x,z];
                var cellTempLocation = new Vector3((x - centerX) * cellSizeX + (cellSizeX / 2), 0, (z - centerZ) * cellSizeZ + (cellSizeZ / 2));
                var cellTemp = Instantiate(gridCell, cellTempLocation, new Quaternion());

                //Build Walls
                if(cellTempLocation.x > 4 || cellTempLocation.x < -4 || cellTempLocation.z > 4 || cellTempLocation.z < -4)
                {
                    cellTemp.transform.GetChild(0).gameObject.SetActive(gridTemp.NWallActive);
                    cellTemp.transform.GetChild(1).gameObject.SetActive(gridTemp.SWallActive);
                    cellTemp.transform.GetChild(2).gameObject.SetActive(gridTemp.EWallActive);
                    cellTemp.transform.GetChild(3).gameObject.SetActive(gridTemp.WWallActive);
                }
                else
                {
                    cellTemp.transform.GetChild(0).gameObject.SetActive(false);
                    cellTemp.transform.GetChild(1).gameObject.SetActive(false);
                    cellTemp.transform.GetChild(2).gameObject.SetActive(false);
                    cellTemp.transform.GetChild(3).gameObject.SetActive(false);
                }

                //Set Enemies
                if((gridTemp.NWallActive ? 1 : 0) + (gridTemp.SWallActive ? 1 : 0) + (gridTemp.EWallActive ? 1 : 0) + (gridTemp.WWallActive ? 1 : 0)  == 3 && 
                    (cellTempLocation.x > 5 || cellTempLocation.x < -5 || cellTempLocation.z > 5 || cellTempLocation.z < -5) && 
                    Random.Range(0, enemySpawnRate) == 0)
                {
                    var enemyGameObject = cellTemp.transform.GetChild(4).gameObject;
                    enemyGameObject.SetActive(true);
                    if (!gridTemp.SWallActive)
                        enemyGameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
                    else if (!gridTemp.EWallActive)
                        enemyGameObject.transform.Rotate(new Vector3(0f, 90f, 0f));
                    else if (!gridTemp.WWallActive)
                        enemyGameObject.transform.Rotate(new Vector3(0f, -90f, 0f));
                }

                //Set Victory
                if(victorySide == 0 && x == 0 && z == 0)
                {
                    cellTemp.transform.GetChild(5).gameObject.SetActive(true);
                    cellTemp.transform.GetChild(4).gameObject.SetActive(false);
                }
                else if (victorySide == 1 && x == 0 && z == grid.GetLength(1) - 1)
                {
                    cellTemp.transform.GetChild(5).gameObject.SetActive(true);
                    cellTemp.transform.GetChild(4).gameObject.SetActive(false);
                }
                else if (victorySide == 2 && x == grid.GetLength(0) - 1 && z == 0)
                {
                    cellTemp.transform.GetChild(5).gameObject.SetActive(true);
                    cellTemp.transform.GetChild(4).gameObject.SetActive(false);
                }
                else if (victorySide == 3 && x == grid.GetLength(0) - 1 && z == grid.GetLength(1) - 1)
                {
                    cellTemp.transform.GetChild(5).gameObject.SetActive(true);
                    cellTemp.transform.GetChild(4).gameObject.SetActive(false);
                }

                //Build Lights
                if(lightCounter == 0)
                {
                    cellTemp.transform.GetChild(6).gameObject.SetActive(true);
                    lightCounter = Random.Range(lightSpawnRate / 3, lightSpawnRate);
                }
                else
                    lightCounter--;
            }
        }
    }

    private List<Vector2Int> BuildFrontier(GridCellModel[,] grid, Vector2Int currentCell, List<Vector2Int> neighbors)
    {
        return neighbors;
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
