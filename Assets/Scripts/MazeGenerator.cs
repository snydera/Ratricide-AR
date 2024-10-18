using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    private MazeCell[,] _mazeGrid;

    public void StartMazeGeneration(int seed)
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        // Set the seed for random generation
        Random.InitState(seed);

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                //_mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, -1, z), Quaternion.identity, transform);

                MazeCell cell = Instantiate(_mazeCellPrefab, new Vector3(x, -1, z), Quaternion.identity, transform);
                _mazeGrid[x, z] = cell;

                // Check if the cell is at the boundary (exterior wall)
                bool isInteriorCell = IsInteriorCell(x, z);

                // If the cell is interior, make its walls destructible
                if (isInteriorCell)
                {
                    AddDamageableComponent(cell);
                }
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);

        transform.localScale = new Vector3(7, 1, 7);
    }

    private bool IsInteriorCell(int x, int z)
    {
        // A cell is interior if it's not on the outer edges of the maze
        return (x > 0 && x < _mazeWidth - 1) && (z > 0 && z < _mazeDepth - 1);
    }

    private void AddDamageableComponent(MazeCell cell)
    {
        // Add the DamageableWall component to each wall of the interior cells
        if (cell.leftWall != null) cell.leftWall.AddComponent<DamageableWall>();
        if (cell.rightWall != null) cell.rightWall.AddComponent<DamageableWall>();
        if (cell.frontWall != null) cell.frontWall.AddComponent<DamageableWall>();
        if (cell.backWall != null) cell.backWall.AddComponent<DamageableWall>();
    }


    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
}
