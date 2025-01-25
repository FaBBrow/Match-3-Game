using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    public Image buttonImage;
    public Image[] stars;
    public TextMeshProUGUI levelText;
    private int starsActive;


    public int level;

    public GameObject confirmPanel;
    private Button myButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        loadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void loadData()
    {
        if (GameData.gameData.saveData.isActive[level - 1])
        {
            isActive = true;
        }
        else
            isActive = false;

        starsActive = GameData.gameData.saveData.stars[level - 1];
    }
    public void ActivateStars()
    {
        for (var i = 0; i < starsActive; i++) stars[i].enabled = true;
    }

    public void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    public void ShowLevel()
    {
        levelText.text = level.ToString();
    }

    public void ConfirmPanel(int level)
    {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}