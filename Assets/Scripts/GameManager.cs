using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CanvasGroup gameOverPanel;
    public TMP_Text gameOverText;

    void Awake()
    {
        Instance = this;
        if (gameOverPanel != null)
            gameOverPanel.alpha = 0;
    }

    public void GameOver(bool win)
    {
        Debug.Log("GameOver called: " + (win ? "Win" : "Lose"));
        if (gameOverPanel != null && gameOverText != null)
        {
            gameOverPanel.alpha = 1;
            gameOverPanel.interactable = true;
            gameOverPanel.blocksRaycasts = true;
            gameOverText.text = win ? "Victory" : "Defeat";
        }
    }
}