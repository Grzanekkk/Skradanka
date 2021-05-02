using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;

    bool gameIsOver;

    void Start()
    {
        gameLoseUI.SetActive(false);
        gameWinUI.SetActive(false);
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().OnPlayerWin += ShowGameWinUI;
    }

    private void Update()
    {
        if (gameIsOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }

    void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player>().OnPlayerWin -= ShowGameWinUI;
    }
}
