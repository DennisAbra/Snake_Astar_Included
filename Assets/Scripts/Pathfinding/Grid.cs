using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node[,] grid;
    public Vector2 gridWorldSize;
    public bool walkable;

    private void Awake()
    {
        // WorldPosition for GameObjects are the same as their position in the grid
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[Mathf.RoundToInt(gridWorldSize.x), Mathf.RoundToInt(gridWorldSize.y)];
        for (int x = 0; x < gridWorldSize.x; x++)
        {
            for (int y = 0; y < gridWorldSize.y; y++)
            {
                bool walkable = true;
                grid[x, y] = new Node(walkable, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0 || x == 1 && y == 1 || x == -1 && y == 1 || x == 1 && y == -1 || x == -1 && y == -1) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if(checkX >= 0 && checkX < gridWorldSize.x && checkY >= 0 && checkY < gridWorldSize.y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public List<Node> path;


    private void OnDrawGizmos()
    {
        if(grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(node)) Gizmos.color = Color.green;
                Gizmos.DrawCube(new Vector3(node.gridX, node.gridY, 0), Vector3.one * (1-.1f ));
            }
           
        }
    }
}
