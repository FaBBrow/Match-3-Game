using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour
{
    public string SceneToLoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OK()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}