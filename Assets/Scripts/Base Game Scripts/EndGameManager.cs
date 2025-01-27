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

    [SerializeField] private GameObject YouWinPanel;
    [SerializeField] private GameObject TryAgainPanel;
    public int currentCounterValue;
    public float timerSeconds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        instance = this;
        SetGameType();
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

    public void SetGameType()
    {
        requirements = Board.instance.world.levels[Board.instance.level].EndGameRequirements;
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
        if (Board.instance.CurrentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;

            if (currentCounterValue <= 0) LoseGame();
        }
    }

    public void WinGame()
    {
        YouWinPanel.SetActive(true);
        Board.instance.CurrentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

       
        FadePanelController.instance.GameOver();
    }

    public void LoseGame()
    {
        TryAgainPanel.SetActive(true);
        Board.instance.CurrentState = GameState.lose;
        Debug.Log("lose");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController.instance.GameOver();
    }
}