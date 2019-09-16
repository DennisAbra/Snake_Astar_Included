using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    public bool searchingForTail;

    private void Start()
    {
        grid = GetComponent<Grid>();
    }

    public bool FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.grid[(int)startPos.x, (int)startPos.y];
        Node targetNode = grid.grid[(int)targetPos.x, (int)targetPos.y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                searchingForTail = false;
                RetracePath(startNode, targetNode);
                return true;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue; 
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour); 
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        // Debug.Log("Didn't find a path");
        searchingForTail = true;
        return false;
    }

    void RetracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) return 50 * dstY + 10 * (dstX - dstY); // Change the first number to lower, if you want to allow diagonal(ish) movement | ish becuase the snake can't actually move diagonally, but changing that number will make it believe it can
        return 50 * dstX + 10 * (dstY - dstX);
    }
}
