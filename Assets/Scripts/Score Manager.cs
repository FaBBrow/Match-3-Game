using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private TextMeshProUGUI scoreText;
    public int score;
    [SerializeField] private Image scoreBoard;

    private void Start()
    {
        instance = this;
        scoreBoard.fillAmount = 0;
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }

    public void increaseScore(int amount)
    {
        score += amount;
        var length = Board.instance.scoreGoals.Length;
        scoreBoard.fillAmount = score / (float)Board.instance.scoreGoals[length - 1];
    }
}