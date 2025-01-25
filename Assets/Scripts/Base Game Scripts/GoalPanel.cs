using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public Image thisImage;
    public Sprite thisSprite;
    public TextMeshProUGUI thisText;

    public string thisString;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        setup();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void setup()
    {
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }
}