using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private int startingLives = 3 ;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject gameOverScreen;

    private int currentLives;

    private void Start()
    {
        currentLives = startingLives;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void ReduceLives()
    {
        currentLives--;

        UpdateLivesUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }
    }

    private void GameOver()
    {
        gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        gameOverScreen.SetActive(false);

        ResetLives();

        Time.timeScale = 1f;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetLives()
    {
        currentLives = startingLives;
        UpdateLivesUI();
    }
}
