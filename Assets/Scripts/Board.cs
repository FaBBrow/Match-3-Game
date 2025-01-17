using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using Random = UnityEngine.Random;
using WaitForSeconds = UnityEngine.WaitForSeconds;

public enum GameState
{
    wait,move
}

public enum TileKind
{
    Breakable,Blank,Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind TileKind;
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
    [SerializeField] public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    [SerializeField] private List<GameObject> Dots;
    [SerializeField] private Vector3 cellGap;
    [SerializeField] private GameObject breakableTileObject;
    [Serialize] public GameObject[,] allDots;
    [SerializeField] public Vector2 boardDotOffset;
    public Dot CurrentDot;

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
        breakableTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        setup();
        instance = this;
    }

    private void Start()
    { 
        boardDotOffset = new Vector2(Mathf.RoundToInt(0-width/2),
            Mathf.RoundToInt(0-height/2));
       
        offset = height + 1;
    }

    public void generateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.GetLength(0) ; i++)
        {
            if (boardLayout[i].TileKind==TileKind.Blank)
            {
                Debug.Log(boardLayout[i].x);
                blankSpaces[boardLayout[i].x, boardLayout[i].y]=true;
            }
        }
    }

    public void generateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].TileKind==TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x-width/2, boardLayout[i].y-height/2);
                Debug.Log("yarattı");
                GameObject tile = Instantiate(breakableTileObject,tempPosition , quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void setup()
    {
        generateBlankSpaces();
        generateBreakableTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
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
                    dot.GetComponent<Dot>().column = i - (int)boardDotOffset.x;
                    dot.name = "( " + i + "," + j + ")";



                    dot.transform.SetParent(this.transform);

                    allDots[i, j] = dot;
                }
            }
        }
    }

    public void slidefalser(GameObject dot)
    {
        dot.GetComponent<Dot>().slideing = false;
    }
    public bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {


                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {


                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {


                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {


                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool columnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = FindMatches.instance.CurrentMatches[0].GetComponent<Dot>();
        if (firstPiece!=null)
        {
            foreach (GameObject curentpiece in FindMatches.instance.CurrentMatches)
            {
                Dot dot = curentpiece.GetComponent<Dot>();
                if (dot.row==firstPiece.row)
                {
                    numberHorizontal++;
                }

                if (dot.column==firstPiece.column)
                {
                    numberVertical++;
                }
            }

            
        }
        return (numberHorizontal==5|| numberVertical==5);
    }
    public void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            if (FindMatches.instance.CurrentMatches.Count==4)
            {
                FindMatches.instance.checkBombs();
            }

            if (FindMatches.instance.CurrentMatches.Count == 5|| FindMatches.instance.CurrentMatches.Count==8)
            {
                if (columnOrRow())
                {
                    FindMatches.instance.checkColorBombs();
                }
                else
                {
                    FindMatches.instance.checkAdjacentBomb();
                }
            }

            if (breakableTiles[column,row]!=null)
            {
                breakableTiles[column,row].takeDamage(1);
                if (breakableTiles[column,row].hitPoints<=0)
                {
                    breakableTiles[column, row] = null;
                }
                
            }
            FindMatches.instance.CurrentMatches.Remove(allDots[column,row]);
            Vector2 dotPosition = new Vector2(Mathf.Round(allDots[column, row].transform.position.x),
                Mathf.Round(allDots[column, row].transform.position.y));
            GameObject particle= Instantiate(destroyEffect, dotPosition, Quaternion.identity);
            Destroy(particle,0.5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
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

        StartCoroutine(DecreaseRowCo2());
    }


    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i ++)
        {
            for (int j = 0; j < height; j ++)
            {
                if(!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    for (int k = j + 1; k < height; k ++)
                    {
                        if(allDots[i, k]!= null)
                        {
                            allDots[i, k].GetComponent<Dot>().row = j+(int)boardDotOffset.y;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }






   private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null&& !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i - width / 2, j - height / 2+offset);
                    int dotToUse = Random.Range(0, Dots.Count);
                    GameObject piece = Instantiate(Dots[dotToUse], tempPosition, quaternion.identity);
                    
                    
                    piece.GetComponent<Dot>().row = j - (int)boardDotOffset.y;
                    piece.GetComponent<Dot>().column = i - (int)boardDotOffset.x;
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
        FindMatches.instance.CurrentMatches.Clear();
        CurrentDot = null;
        yield return new WaitForSeconds(0.5f);
        if (isDeadLock())
        {
            shuffleBoard();
        }
        CurrentState = GameState.move;

    }

    public void switchPieces(int column,int row,Vector2 direction )
    {
        GameObject holder = allDots[column+(int)direction.x, row+(int)direction.y];
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = holder;
    }

    private bool checkForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j]!=null)
                {
                    if (i < width - 2)
                    {


                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                                allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2)
                    {


                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                                allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool switchAndCheck(int column, int row, Vector2 direction)
    {
        switchPieces(column,row,direction);
        if (checkForMatches())
        {
            switchPieces(column,row,direction);
            return true;
        }
        switchPieces(column,row,direction);
        return false;
    }

    private bool isDeadLock()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j]!=null)
                {
                    if (i<width-1)
                    {
                        if (switchAndCheck(i,j,Vector2.right))
                        {
                            return false;
                        }

                        
                    }

                    if (j<height-1)
                    {
                        if (switchAndCheck(i,j,Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    private void shuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j]!=null)
                {
                    newBoard.Add(allDots[i,j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i,j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    
                    int maxiterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxiterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxiterations++;
                    }
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxiterations = 0;
                    piece.column = i + (int)boardDotOffset.x;
                    piece.row = j + (int)boardDotOffset.y;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        if (isDeadLock())
        {
            shuffleBoard();
        }
    }
}