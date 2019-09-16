using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject tailPrefab;
    [SerializeField] GameObject foodPrefab;
    [Header("Boundaries")]
    [SerializeField] Transform topBorder;
    [SerializeField] Transform bottomBorder;
    [SerializeField] Transform leftBorder;
    [SerializeField] Transform rightBorder;

    [Header("Movement")]
    [Range(0.02f, 1f)]
    [SerializeField] float timeToMove = .1f;
    float tick = 0;

    [SerializeField] Vector2 direction = Vector2.right;
    Vector2 oldPos;
    public SinglyLinkedList singlyList;
    Pathfinding pathFind;
    Grid grid;
    GameHandler gameHandler;

    bool ate = false;
    GameObject go;
    [HideInInspector]public bool usePathFinding;
    [HideInInspector] public bool isPlaying = false;
    bool foundPath;
    [Header("Only set above 1 for pathfinding")]
    [Range(1, 10)]
    [SerializeField] float timeScaleMultiplier;

     enum eDirection { up, right, down, left };
     eDirection snakeDirection;

    void Start()
    {
        pathFind = FindObjectOfType<Pathfinding>();
        singlyList = new SinglyLinkedList();
        singlyList.InsertFrontOfLine(gameObject);
        grid = FindObjectOfType<Grid>();
        gameHandler = FindObjectOfType<GameHandler>();
        go = Instantiate(foodPrefab, new Vector2(5, 5), Quaternion.identity);
        MoveFood();
    }

    private void FixedUpdate()
    {
        if(isPlaying)
        {
            NormalPlay();
            PathfindingPlay();
            tick += Time.fixedDeltaTime;
        }
    }

    private void PathfindingPlay()
    {
        Time.timeScale = timeScaleMultiplier;
        if (tick > timeToMove && usePathFinding)
        {
            foundPath = pathFind.FindPath(transform.position, go.transform.position);
            MoveWithPathFinding();
        }
    }

    private void NormalPlay()
    {
        CheckMovementDirectionWithPlayerInput(); // Self explanatory
        if (tick > timeToMove && !usePathFinding)
        {
            Move();
        }
    }

    
    Vector2 desiredPos;
    private void MoveWithPathFinding()
    {
         oldPos = transform.position;
         if(foundPath)
         {
            desiredPos = new Vector2(grid.path[0].gridX, grid.path[0].gridY);
            CheckPathDirection();
        }

       else if(pathFind.searchingForTail && !foundPath)
        {
            MoveWithPanicPath();
        }

        HandleSnakeBodyMovement();

        transform.Translate(direction);
        tick = 0;
    }

    private void MoveWithPanicPath()
    {
        Debug.Log("Path not found");
        Tile temp = singlyList.last;
        if (temp != null)
        {
            grid.grid[(int)temp.gameObjectdata.transform.position.x, (int)temp.gameObjectdata.transform.position.y].walkable = true;
            bool foundTail = pathFind.FindPath(transform.position, temp.gameObjectdata.transform.position);
            if (foundTail)
            {
                desiredPos = new Vector2(grid.path[0].gridX, grid.path[0].gridY);
                CheckPathDirection();
            }
            else TryToSurvive();
        }
    }

    private void TryToSurvive()
    {
        Node currentNode = grid.grid[(int)transform.position.x, (int)transform.position.y];
        foreach (Node node in grid.GetNeighbours(currentNode))
        {
            if(node.walkable && node.gridX > transform.position.x)
            {
                direction = Vector2.right;
            }
            else if(node.walkable && node.gridX < transform.position.x)
            {

                direction = Vector2.left;
            }
            else if(node.walkable && node.gridY > transform.position.y)
            {
                direction = Vector2.up;

            }
            else if(node.walkable && node.gridY < transform.position.y)
            {
                direction = Vector2.down;
            }
        }
    }

    private void SetGridWalkableTrue(Transform temp)
    {
        grid.grid[(int)temp.transform.position.x, (int)temp.transform.position.y].walkable = true;
    }

    private void SetGridWalkableFalse(Transform temp)
    {
        grid.grid[(int)temp.transform.position.x, (int)temp.transform.position.y].walkable = false;
    }

    private void CheckPathDirection()
    {
        if(desiredPos.x > oldPos.x)
        {
            direction = Vector2.right;
        }
        else if (desiredPos.y < oldPos.y)
        {
            direction = Vector2.down;
        }
        else if(desiredPos.x < oldPos.x)
        {
            direction = Vector2.left;
        }
        else if(desiredPos.y > oldPos.y)
        {
            direction = Vector2.up;
        }

    }

    private void CheckMovementDirectionWithPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && snakeDirection != eDirection.right || Input.GetKeyDown(KeyCode.LeftArrow) && snakeDirection != eDirection.right)
        {
            direction = Vector2.left;
            snakeDirection = eDirection.left;

        }
        else if (Input.GetKeyDown(KeyCode.D) && snakeDirection != eDirection.left || Input.GetKeyDown(KeyCode.RightArrow) && snakeDirection != eDirection.left)
        {
            direction = Vector2.right;
            snakeDirection = eDirection.right;

        }
        else if (Input.GetKeyDown(KeyCode.W) && snakeDirection != eDirection.down || Input.GetKeyDown(KeyCode.UpArrow) && snakeDirection != eDirection.down)
        {
            direction = Vector2.up;
            snakeDirection = eDirection.up;

        }
        else if (Input.GetKeyDown(KeyCode.S) && snakeDirection != eDirection.up || Input.GetKeyDown(KeyCode.DownArrow) && snakeDirection != eDirection.up)
        {
            direction = Vector2.down;
            snakeDirection = eDirection.down;

        }
    }

    public void Move()
    {
        oldPos = transform.position;

        HandleSnakeBodyMovement();
        transform.Translate(direction);
        tick = 0;
    }

    private void HandleSnakeBodyMovement()
    {
        if (ate)
        {
            GameObject tail = Instantiate(tailPrefab, oldPos, Quaternion.identity, transform.parent);

            if (singlyList.Count > 1)
            {
                singlyList.InsertAfter(singlyList.head.next, tail);
                SetGridWalkableFalse(tail.transform);
            }
            else 
            {
                singlyList.InsertLast(tail);
            }
            gameHandler.UpdateScoreText();
            ate = false;
        }
        else if (singlyList.Count > 1)
        {
            var temp = singlyList.GetLastNode();

            if (temp != null)
            {
                SetGridWalkableTrue(temp.gameObjectdata.transform);
                temp.gameObjectdata.transform.position = oldPos;
                singlyList.RemoveLast();
                singlyList.InsertAfter(singlyList.head.next, temp.gameObjectdata);
                SetGridWalkableFalse(temp.gameObjectdata.transform);
            }
        }
    }


    public void MoveFood()
    {
        // I started out with spawning a new piece of food everytime the food ate.
        // Went with only spawning it once, and then moving it everytime it gets eaten instead as the pathfinder wont calculate a path until it spawned.
        // Made everything so much more smooth
        // Values are converted to int as the snake will only move along whole numbers
        Vector2 foodNewPos;
        float xPos, yPos;
        do
        {
            xPos = (int)Random.Range(leftBorder.position.x + 1, rightBorder.position.x - 1); // The magic number is to prevent food to spawn inside my borders
            yPos = (int)Random.Range(bottomBorder.position.y + 1, topBorder.position.y - 1); // The magic number is to prevent food to spawn inside my borders
            // xPos = (int)Random.Range(0,70); // The magic number is to prevent food to spawn inside my borders
            // yPos = (int)Random.Range(0,50);
        }
        while (!grid.grid[(int)xPos, (int)yPos].walkable || xPos == transform.position.x && yPos == transform.position.y);
            foodNewPos = new Vector2(xPos, yPos);
            go.transform.position = foodNewPos;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.StartsWith("Food"))
        {
            ate = true;
            MoveFood();
        }
        else
        {
            gameHandler.OnLost();
        }
    }


}
