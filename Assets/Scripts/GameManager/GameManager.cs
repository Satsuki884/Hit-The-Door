using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _toMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TMP_Text _scoreText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _gameOverPanel.SetActive(false);
        _toMenuButton.onClick.RemoveAllListeners();
        _toMenuButton.onClick.AddListener(ToMenu);
        _restartButton.onClick.RemoveAllListeners();
        _restartButton.onClick.AddListener(Restart);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerBall.Instance.Restart();
        Door.Instance.Restart();
    }

    private void ToMenu()
    {
        SceneManager.LoadScene("Start");
        PlayerBall.Instance.Restart();
        Door.Instance.Restart();
    }

    public void GameOver(string score)
    {
        _scoreText.text = score;
        _gameOverPanel.SetActive(true);
    }
}
