using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using Random = UnityEngine.Random;
using WaitForSeconds = UnityEngine.WaitForSeconds;

public enum GameState
{
    wait,move
}
public class Board : MonoBehaviour
{
    public GameState CurrentState=GameState.move;
    public static Board instance;
    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private int offset;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject destroyEffect;
    
    private BackgroundTile[,] allTiles;
    [SerializeField] private List<GameObject> Dots;
    [SerializeField] private Vector3 cellGap;
    [SerializeField]public GameObject[,] allDots;
    [SerializeField] public Vector2 boardDotOffset;

    public Board(int height, int width)
    {
        this.height = height;
        this.width = width;
    }

    public int width
    {
        get => _width;
        set => _width = value;
    }

    public int height
    {
        get => _height;
        set => _height = value;
    }

    private void Awake()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        setup();
        instance = this;
    }

    private void Start()
    {
        boardDotOffset = new Vector2(Mathf.RoundToInt(allDots[0, 0].transform.position.x),
            Mathf.RoundToInt(allDots[0, 0].transform.position.y));
    }

    private void setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 startPosition = new Vector2(i - width / 2, j - height / 2 + offset);
                Vector2 tempPosition = new Vector2(i - width / 2, j - height / 2);
                
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, quaternion.identity);
                backgroundTile.transform.SetParent(this.transform);
                
                backgroundTile.name = "( " + i + "," + j + ")";
                int dotToUseInt = Random.Range(0, Dots.Count);
                int maxiterations = 0;
                while (MatchesAt(i, j, Dots[dotToUseInt]) && maxiterations < 100)
                {
                    dotToUseInt = Random.Range(0, Dots.Count);
                    maxiterations++;
                }

                maxiterations = 0;
                GameObject dot = Instantiate(Dots[dotToUseInt], tempPosition, quaternion.identity);
                dot.transform.localScale = dot.transform.localScale - cellGap;
                
                dot.GetComponent<Dot>().row = j - (int)boardDotOffset.y;
                dot.GetComponent<Dot>().collumn = i - (int)boardDotOffset.x;
                dot.name = "( " + i + "," + j + ")";
                
                
                
                dot.transform.SetParent(this.transform);
                
                allDots[i, j] = dot;
            }
        }
    }

    public void slidefalser(GameObject dot)
    {
        dot.GetComponent<Dot>().slideing = false;
    }
    public bool MatchesAt(int collumn, int row, GameObject piece)
    {
        if (collumn > 1 && row > 1)
        {
            if (allDots[collumn - 1, row].tag == piece.tag && allDots[collumn - 2, row].tag == piece.tag)
            {
                return true;
            }

            if (allDots[collumn, row - 1].tag == piece.tag && allDots[collumn, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (collumn <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[collumn, row - 1].tag == piece.tag && allDots[collumn, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }

            if (collumn > 1)
            {
                if (allDots[collumn - 1, row].tag == piece.tag && allDots[collumn - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void DestroyMatchesAt(int collumn, int row)
    {
        if (allDots[collumn, row].GetComponent<Dot>().isMatched)
        {
            FindMatches.instance.CurrentMatches.Remove(allDots[collumn,row]);
            Vector2 dotPosition = new Vector2(Mathf.Round(allDots[collumn, row].transform.position.x),
                Mathf.Round(allDots[collumn, row].transform.position.y));
            GameObject particle= Instantiate(destroyEffect, dotPosition, Quaternion.identity);
            Destroy(particle,0.5f);
            Destroy(allDots[collumn, row]);
            allDots[collumn, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    public IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }

            nullCount = 0;
        }

        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i - width / 2, j - height / 2+offset);
                    int dotToUse = Random.Range(0, Dots.Count);
                    GameObject piece = Instantiate(Dots[dotToUse], tempPosition, quaternion.identity);
                    
                    
                    piece.GetComponent<Dot>().row = j - (int)boardDotOffset.y;
                    piece.GetComponent<Dot>().collumn = i - (int)boardDotOffset.x;
                    allDots[i, j] = piece;
                    
                    

                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }

        yield return new WaitForSeconds(0.5f);
        CurrentState = GameState.move;

    }
}