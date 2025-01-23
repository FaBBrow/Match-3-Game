using System;
using TMPro;
using UnityEngine;

public enum GameType
{
    Moves,
    Time
}

[Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager instance;
    public EndGameRequirements requirements;

    [SerializeField] private GameObject moveLabel;

    [SerializeField] private GameObject timeLabel;

    [SerializeField] private TextMeshProUGUI counter;

    public int currentCounterValue;
    public float timerSeconds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        instance = this;
        setupGame();
    }

    // Update is called once per frame
    private void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                decreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    public void setupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            moveLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            moveLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = "" + currentCounterValue;
    }

    public void decreaseCounterValue()
    {
        currentCounterValue--;
        counter.text = "" + currentCounterValue;

        if (currentCounterValue <= 0)
        {
            Debug.Log("lose");
            currentCounterValue = 0;
            counter.text = "" + currentCounterValue;
        }
    }
}