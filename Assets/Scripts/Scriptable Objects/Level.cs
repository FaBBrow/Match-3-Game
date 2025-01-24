using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")] public int width;
    public int height;


    [Header("Starting Tiles")] public TileType[] BoardLayout;

    [Header("Available Dots")] public List<GameObject> Dots;

    [Header("Score Goals")] public int[] ScoreGoals;

    [Header("End Game Requirements")] public EndGameRequirements EndGameRequirements;
    public BlankGoal[] levelGoals;
}