using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas loseMenu;
    [SerializeField] Text scoreText;

    Snake snake;
    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
        mainMenu.enabled = true;
        loseMenu.enabled = false;
    }
    public void PlayWithPathFinding()
    {
        mainMenu.enabled = false;
        snake.usePathFinding = true;
        snake.isPlaying = true;
    }

    public void PlayNormal()
    {
        mainMenu.enabled = false;
        snake.usePathFinding = false;
        snake.isPlaying = true;
    }

    public void OnLost()
    {
        snake.isPlaying = false;
        loseMenu.enabled = true;
        if(snake.usePathFinding)
        {
            loseMenu.GetComponentInChildren<Text>().text = "Cheating is not the way to victory, would you like to try again?";
        }
        else
        {
            loseMenu.GetComponentInChildren<Text>().text = "Nice try, want to try again?";
        }
    }

    public void Retry()
    {
        loseMenu.enabled = false;
        SceneManager.LoadScene(0);
    }

    public void UpdateScoreText()
    {
        int score = snake.singlyList.Count - 1;
        scoreText.text = score.ToString();
    }
}
