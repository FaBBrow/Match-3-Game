using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class BlankGoal
{
    [SerializeField] public int numberNeeded;
    [SerializeField] public int numberCollected;
    [SerializeField] public Sprite goalSprite;
    [SerializeField] public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;
    public List<GoalPanel> currentGoals = new();
    public BlankGoal[] levelGoals;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject goalIntroParent;
    [SerializeField] private GameObject goalGameParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        instance = this;
        setupIntroGoals();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void setupIntroGoals()
    {
        for (var i = 0; i < levelGoals.Length; i++)
        {
            var goal = Instantiate(goalPrefab, goalIntroParent.transform.position, quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            var panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
            var gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            var panel1 = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel1);
            panel1.thisSprite = levelGoals[i].goalSprite;
            panel1.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void updateGoals()
    {
        var goalsCompleted = 0;
        for (var i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }

        if (goalsCompleted >= levelGoals.Length) EndGameManager.instance.WinGame();
    }

    public void compareGoal(string goaltocompare)
    {
        for (var i = 0; i < levelGoals.Length; i++)
            if (goaltocompare == levelGoals[i].matchValue)
                levelGoals[i].numberCollected++;
    }
}