using System;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private TextMeshProUGUI scoreText;
    public int score;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }

    public void increaseScore( int amount)
    {
        score += amount;
    }
}
