using System;
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
    public BlankGoal[] levelGoals;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject goalIntroParent;
    [SerializeField] private GameObject goalGameParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
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
            panel1.thisSprite = levelGoals[i].goalSprite;
            panel1.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }
}