using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button _startPanel;

    void Start()
    {
        _startPanel.onClick.RemoveAllListeners();
        _startPanel.onClick.AddListener(Game);
    }

    private void Game()
    {
        SceneManager.LoadScene("Main");
    }
}
