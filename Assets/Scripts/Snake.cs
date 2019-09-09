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

    Vector2 direction = Vector2.right;
    Vector2 oldPos;
    SinglyLinkedList singlyList;
    Pathfinding pathFind;
    Grid grid;
    GameHandler gameHandler;

    bool ate = false;
    GameObject go;
    public bool usePathFinding;
    public bool isPlaying = false;

    public enum eDirection { up, right, down, left };
    public eDirection snakeDirection;

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
            CheckMovementDirectionWithPlayerInput();
            if (tick > timeToMove && !usePathFinding)
            {
                Move();
            }
            if (tick > timeToMove && usePathFinding)
            {
                pathFind.FindPath(transform.position, go.transform.position);
                MoveWithPathFinding();
            }
            tick += Time.fixedDeltaTime;
        }
    }

    Vector2 desiredPos;
    private void MoveWithPathFinding()
    {
         oldPos = transform.position;
         desiredPos = new Vector2(grid.path[0].gridX, grid.path[0].gridY);

        if (ate)
        {
            GameObject go = Instantiate(tailPrefab, oldPos, Quaternion.identity, transform.parent);

            if (singlyList.Count > 1)
            {
                singlyList.InsertAfter(singlyList.head.next, go);
                grid.grid[(int)go.transform.position.x, (int)go.transform.position.y].walkable = false;
            }
            else
            {
                singlyList.InsertLast(go);
            }
            ate = false;
        }
        else if (singlyList.Count > 1)
        {
            var temp = singlyList.GetLastNode();

            if (temp != null)
            {
                grid.grid[(int)temp.gameObjectdata.transform.position.x, (int)temp.gameObjectdata.transform.position.y].walkable = true;
                temp.gameObjectdata.transform.position = oldPos;
                singlyList.RemoveLast();
                singlyList.InsertAfter(singlyList.head.next, temp.gameObjectdata);
                grid.grid[(int)temp.gameObjectdata.transform.position.x, (int)temp.gameObjectdata.transform.position.y].walkable = false;
            }
        }
        CheckPathDirection();
        transform.Translate(direction);
        tick = 0;
    }

    private void CheckPathDirection()
    {
        if(desiredPos.x > oldPos.x)
        {
            direction = Vector2.right;
        } 
        else if(desiredPos.x < oldPos.x)
        {
            direction = Vector2.left;
        }
        else if(desiredPos.y > oldPos.y)
        {
            direction = Vector2.up;
        }
        else if(desiredPos.y < oldPos.y)
        {
            direction = Vector2.down;
        }
    }

    private void CheckMovementDirectionWithPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && snakeDirection != eDirection.right)
        {
            direction = Vector2.left;
            snakeDirection = eDirection.left;

        }
        else if (Input.GetKeyDown(KeyCode.D) && snakeDirection != eDirection.left)
        {
            direction = Vector2.right;
            snakeDirection = eDirection.right;

        }
        else if (Input.GetKeyDown(KeyCode.W) && snakeDirection != eDirection.down)
        {
            direction = Vector2.up;
            snakeDirection = eDirection.up;

        }
        else if (Input.GetKeyDown(KeyCode.S) && snakeDirection != eDirection.up)
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
            GameObject go = Instantiate(tailPrefab, oldPos, Quaternion.identity, transform.parent);

            if (singlyList.Count > 1)
            {
                singlyList.InsertAfter(singlyList.head.next, go);
            }
            else
            {
                singlyList.InsertLast(go);
            }
            ate = false;
        }
        else if (singlyList.Count > 1)
        {
            var temp = singlyList.GetLastNode();
            if (temp != null)
            {
                temp.gameObjectdata.transform.position = oldPos;
                singlyList.RemoveLast();
                singlyList.InsertAfter(singlyList.head.next, temp.gameObjectdata);
            }
        }
    }

    Vector2 foodNewPos;

    public void MoveFood()
    {
        // Values are converted to int as the snake will only move along whole numbers
        float xPos, yPos;
        do
        {
            xPos = (int)Random.Range(leftBorder.position.x - 1, rightBorder.position.x + 1);
            yPos = (int)Random.Range(bottomBorder.position.y + 1, topBorder.position.y - 1);
        }
        while (!grid.grid[(int)xPos, (int)yPos].walkable);
        
        foodNewPos = new Vector2(xPos, yPos);
        go.transform.position = foodNewPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.StartsWith("Food"))
        {
            ate = true;
            //Destroy(collision.gameObject);
            MoveFood();
        }
        else
        {
            gameHandler.OnLost();
        }
    }


}
