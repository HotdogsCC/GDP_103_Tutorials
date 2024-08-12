using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int score = 0;

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score text GameObject not set.");
            return;
        }

        UpdateScore();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}
