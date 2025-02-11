using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button _start;
    [SerializeField] private Button _exit;

    void Start()
    {
        _start.onClick.RemoveAllListeners();
        _start.onClick.AddListener(Game);
        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(Exit);
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void Game()
    {
        SceneManager.LoadScene("Main");
    }
}
