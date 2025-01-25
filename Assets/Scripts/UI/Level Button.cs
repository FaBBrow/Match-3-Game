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


    public int level;

    public GameObject confirmPanel;
    private Button myButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ActivateStars()
    {
        for (var i = 0; i < stars.Length; i++) stars[i].enabled = false;
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