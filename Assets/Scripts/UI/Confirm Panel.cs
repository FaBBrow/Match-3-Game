using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;

    public Image[] stars;

    public int level;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        ActivateStars();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ActivateStars()
    {
        for (var i = 0; i < stars.Length; i++) stars[i].enabled = false;
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
}