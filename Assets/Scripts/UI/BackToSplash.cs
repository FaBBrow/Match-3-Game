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

    public void WinOK()
    {
        GameData.gameData.saveData.isActive[Board.instance.level+1] = true;
        GameData.gameData.Save();
        for (int i = 0; i < GoalManager.instance.levelGoals.Length; i++)
        {
            GoalManager.instance.levelGoals[i].numberCollected = 0;
        }
        SceneManager.LoadScene(SceneToLoad);
    }

    public void LoseOk()
    {
        for (int i = 0; i < GoalManager.instance.levelGoals.Length; i++)
        {
            GoalManager.instance.levelGoals[i].numberCollected = 0;
        }
        SceneManager.LoadScene(SceneToLoad);
    }
}