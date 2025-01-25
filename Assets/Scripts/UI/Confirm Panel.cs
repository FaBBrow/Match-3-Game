using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;

    private int starsActive;
    public Image[] stars;
    public TextMeshProUGUI statsScoreText;
    public TextMeshProUGUI statsStarText;
    public int level;
    private int highScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        LoadData();
        ActivateStars();
        setText();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void LoadData()
    {

        starsActive = GameData.gameData.saveData.stars[level];
        highScore = GameData.gameData.saveData.highScores[level];
    }

    public void setText()
    {
        statsScoreText.text = highScore.ToString();
        statsStarText.text = "" + starsActive + "/3";
    }

    public void ActivateStars()
    {
        for (var i = 0; i < starsActive; i++) stars[i].enabled = true;
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level);
        SceneManager.LoadScene(levelToLoad);
    }

    private void OnEnable()
    {
        LoadData();
        ActivateStars();
        setText();
        
    }
}