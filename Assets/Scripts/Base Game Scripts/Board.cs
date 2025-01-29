using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using WaitForSeconds = UnityEngine.WaitForSeconds;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal,
    Lock,
    Concrete,
    Slime
}
[Serializable]
public class MatchType
{
    public int type;
    public string color;
}
[Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind TileKind;
}

public class Board : MonoBehaviour
{
    public static Board instance;

    [Header("Scriptable Object Stuff")] public World world;
    public int level;

    public GameState CurrentState = GameState.move;
    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private int offset;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] public TileType[] boardLayout;
    [SerializeField] private List<GameObject> Dots;
    [SerializeField] private Vector3 cellGap;
    [SerializeField] private GameObject breakableTileObject;
    [SerializeField] private GameObject lockTilePrefab;
    [SerializeField] private GameObject concreteTilePrefab;
    [SerializeField] private GameObject slimeTilePrefab;
    [SerializeField] public Vector2 boardDotOffset;
    public Dot CurrentDot;
    public int basePieceValue = 20;
    public int[] scoreGoals;
    public MatchType matchType;
    private bool makeSlime = true;

    [Serialize] public GameObject[,] allDots;
    public bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] concreteTiles;
    public BackgroundTile[,] slimeTiles;
    private int streakValue = 1;

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
        if (PlayerPrefs.HasKey("Current Level")) level = PlayerPrefs.GetInt("Current Level");
        instance = this;
        if (world != null)
            if (world.levels[level] != null)
            {
                width = world.levels[level].width;
                height = world.levels[level].height;
                Dots = world.levels[level].Dots;
                scoreGoals = world.levels[level].ScoreGoals;
                boardLayout = world.levels[level].BoardLayout;
            }

        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        concreteTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        setup();
    }

    private void Start()
    {
        boardDotOffset = new Vector2(Mathf.RoundToInt(0 - width / 2),
            Mathf.RoundToInt(0 - height / 2));

        offset = height + 1;
        basePieceValue = 20;
        streakValue = 1;
        CurrentState = GameState.pause;
    }

    public void generateBlankSpaces()
    {
        for (var i = 0; i < boardLayout.GetLength(0); i++)
            if (boardLayout[i].TileKind == TileKind.Blank)
            {
                Debug.Log(boardLayout[i].x);
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
    }

    public void generateSlimeTiles()
    {
        for (var i = 0; i < boardLayout.Length; i++)
            if (boardLayout[i].TileKind == TileKind.Slime)
            {
                var tempPosition = new Vector2(boardLayout[i].x - width / 2, boardLayout[i].y - height / 2);
                Debug.Log("yarattı");
                var tile = Instantiate(slimeTilePrefab, tempPosition, quaternion.identity);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
    }

    public void generateLockTiles()
    {
        for (var i = 0; i < boardLayout.Length; i++)
            if (boardLayout[i].TileKind == TileKind.Lock)
            {
                var tempPosition = new Vector2(boardLayout[i].x - width / 2, boardLayout[i].y - height / 2);
                Debug.Log("yarattı");
                var tile = Instantiate(lockTilePrefab, tempPosition, quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
    }
    public void generateConcreteTiles()
    {
        for (var i = 0; i < boardLayout.Length; i++)
            if (boardLayout[i].TileKind == TileKind.Concrete)
            {
                var tempPosition = new Vector2(boardLayout[i].x - width / 2, boardLayout[i].y - height / 2);
                Debug.Log("yarattı");
                var tile = Instantiate(concreteTilePrefab, tempPosition, quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
    }

    public void generateBreakableTiles()
    {
        for (var i = 0; i < boardLayout.Length; i++)
            if (boardLayout[i].TileKind == TileKind.Breakable)
            {
                var tempPosition = new Vector2(boardLayout[i].x - width / 2, boardLayout[i].y - height / 2);
                Debug.Log("yarattı");
                var tile = Instantiate(breakableTileObject, tempPosition, quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
    }

    private void setup()
    {
        generateBlankSpaces();
        generateBreakableTiles();
        generateLockTiles();
        generateConcreteTiles();
        generateSlimeTiles();
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (!blankSpaces[i, j]&& !concreteTiles[i,j]&& !slimeTiles[i,j])
            {
                var startPosition = new Vector2(i - width / 2, j - height / 2 + offset);
                var tempPosition = new Vector2(i - width / 2, j - height / 2);

                var backgroundTile = Instantiate(tilePrefab, tempPosition, quaternion.identity);
                backgroundTile.transform.SetParent(transform);

                backgroundTile.name = "( " + i + "," + j + ")";
                var dotToUseInt = Random.Range(0, Dots.Count);
                var maxiterations = 0;
                while (MatchesAt(i, j, Dots[dotToUseInt]) && maxiterations < 100)
                {
                    dotToUseInt = Random.Range(0, Dots.Count);
                    maxiterations++;
                }

                maxiterations = 0;
                var dot = Instantiate(Dots[dotToUseInt], tempPosition, quaternion.identity);
                dot.transform.localScale = dot.transform.localScale - cellGap;

                dot.GetComponent<Dot>().row = j - (int)boardDotOffset.y;
                dot.GetComponent<Dot>().column = i - (int)boardDotOffset.x;
                dot.name = "( " + i + "," + j + ")";


                dot.transform.SetParent(transform);

                allDots[i, j] = dot;
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
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    return true;

            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    return true;
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                        return true;

            if (column > 1)
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                        return true;
        }

        return false;
    }

    private MatchType columnOrRow()
    {
        List<GameObject> matchCopy = FindMatches.instance.CurrentMatches as List<GameObject>;
        matchType.type = 0;
        matchType.color = "";
        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisdot = matchCopy[i].GetComponent<Dot>();
            string color = matchCopy[i].tag;
            int column = thisdot.column;
            int row = thisdot.row;
            int columnMatch = 0;
            int rowMatch = 0;
            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextdot = matchCopy[j].GetComponent<Dot>();
                if (nextdot==thisdot)
                {
                    continue;
                }

                if (nextdot.column==thisdot.column&& nextdot.tag==color)
                {
                    columnMatch++;
                }

                if (nextdot.row==thisdot.row && nextdot.tag==color)
                {
                    rowMatch++;
                }
            }

            if (columnMatch==4|| rowMatch==4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }

            else if (columnMatch==2&& rowMatch==2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }

            else  if (columnMatch==3|| rowMatch==3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
           
        }
        matchType.type = 1;
        matchType.color = "";
        return matchType;
        
        //     var numberHorizontal = 0;
        //     var numberVertical = 0;
        //     var firstPiece = FindMatches.instance.CurrentMatches[0].GetComponent<Dot>();
        //     if (firstPiece != null)
        //         foreach (var curentpiece in FindMatches.instance.CurrentMatches)
        //         {
        //             var dot = curentpiece.GetComponent<Dot>();
        //             if (dot.row == firstPiece.row) numberHorizontal++;
        //
        //             if (dot.column == firstPiece.column) numberVertical++;
        //         }
        //
        //     return numberHorizontal == 5 || numberVertical == 5;
    }

    public void DestroyMatchesAt(int column, int row)
    {
       

        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //     if (FindMatches.instance.CurrentMatches.Count == 4) FindMatches.instance.checkBombs();
            //
            //     if (FindMatches.instance.CurrentMatches.Count == 5 || FindMatches.instance.CurrentMatches.Count == 8)
            //     {
            //         if (columnOrRow())
            //             FindMatches.instance.checkColorBombs();
            //         else
            //             FindMatches.instance.checkAdjacentBomb();
            //     }

            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].takeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0) breakableTiles[column, row] = null;
            }
            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].takeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0) lockTiles[column, row] = null;
            }
            damageConcrete(column,row);
            damageSlime(column,row);
            FindMatches.instance.CurrentMatches.Remove(allDots[column, row]);
            var dotPosition = new Vector2(Mathf.Round(allDots[column, row].transform.position.x),
                Mathf.Round(allDots[column, row].transform.position.y));
            GoalManager.instance.compareGoal(allDots[column, row].tag);
            GoalManager.instance.updateGoals();
            SoundManager.instance.playRandomDestroyNoises();
            var particle = Instantiate(destroyEffect, dotPosition, Quaternion.identity);
            Destroy(particle, 0.5f);
            allDots[column,row].GetComponent<Dot>().popAnimation();
            Destroy(allDots[column, row],.5f);
            ScoreManager.instance.increaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;

        }
    }

    public void DestroyMatches()
    {
        if (FindMatches.instance.CurrentMatches.Count > 3)
        {
            MatchType typeOfMatch = columnOrRow();
            if (typeOfMatch.type==1)
            {
                FindMatches.instance.checkColorBombs();
            }
            else if (typeOfMatch.type==2)
            {
                FindMatches.instance.checkAdjacentBomb();
            }
            else if (typeOfMatch.type==3)
            {
                FindMatches.instance.checkBombs(typeOfMatch);   
            }
        }
        FindMatches.instance.CurrentMatches.Clear();
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (allDots[i, j] != null)
                DestroyMatchesAt(i, j);

        StartCoroutine(DecreaseRowCo2());
    }

    public void bombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (concreteTiles[i,j])
                {
                    concreteTiles[i,row].takeDamage(1);
                    if (concreteTiles[i,row].hitPoints<=0)
                    {
                        concreteTiles[i, row] = null;
                    }
                }
            }
        }
    }
    public void bombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (concreteTiles[i,j])
                {
                    concreteTiles[column,j].takeDamage(1);
                    if (concreteTiles[column,j].hitPoints<=0)
                    {
                        concreteTiles[column, j] = null;
                    }
                }
            }
        }
    }

    public void damageConcrete(int column,int row)
    {
        if (column>0)
        {
            if (concreteTiles[column-1,row])
            {
                concreteTiles[column-1,row].takeDamage(1);
                if (concreteTiles[column-1,row].hitPoints<=0)
                {
                    concreteTiles[column - 1, row] = null;
                }

                
            }   
        }
        if (column<width-1)
        {
            if (concreteTiles[column+1,row])
            {
                concreteTiles[column+1,row].takeDamage(1);
                if (concreteTiles[column+1,row].hitPoints<=0)
                {
                    concreteTiles[column + 1, row] = null;
                }

                
            }   
        }
        if (row>0)
        {
            if (concreteTiles[column,row-1])
            {
                concreteTiles[column,row-1].takeDamage(1);
                if (concreteTiles[column,row-1].hitPoints<=0)
                {
                    concreteTiles[column, row-1] = null;
                }

                
            }   
        }
        if (row<height-1)
        {
            if (concreteTiles[column,row+1])
            {
                concreteTiles[column,row+1].takeDamage(1);
                if (concreteTiles[column,row+1].hitPoints<=0)
                {
                    concreteTiles[column, row+1] = null;
                }

                
            }   
        }
    }
    public void damageSlime(int column,int row)
    {
        if (column>0)
        {
            if (slimeTiles[column-1,row])
            {
                slimeTiles[column-1,row].takeDamage(1);
                if (slimeTiles[column-1,row].hitPoints<=0)
                {
                    slimeTiles[column - 1, row] = null;
                }

                makeSlime = false;


            }   
        }
        if (column<width-1)
        {
            if (slimeTiles[column+1,row])
            {
                slimeTiles[column+1,row].takeDamage(1);
                if (slimeTiles[column+1,row].hitPoints<=0)
                {
                    slimeTiles[column + 1, row] = null;
                }makeSlime = false;

                
            }   
        }
        if (row>0)
        {
            if (slimeTiles[column,row-1])
            {
                slimeTiles[column,row-1].takeDamage(1);
                if (slimeTiles[column,row-1].hitPoints<=0)
                {
                    slimeTiles[column, row-1] = null;
                }makeSlime = false;

                
            }   
        }
        if (row<height-1)
        {
            if (slimeTiles[column,row+1])
            {
                slimeTiles[column,row+1].takeDamage(1);
                if (slimeTiles[column,row+1].hitPoints<=0)
                {
                    slimeTiles[column, row+1] = null;
                }makeSlime = false;

                
            }   
        }
    }
    private IEnumerator DecreaseRowCo2()
    {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (!blankSpaces[i, j] && allDots[i, j] == null&& !concreteTiles[i,j]&& !slimeTiles[i,j])
                for (var k = j + 1; k < height; k++)
                    if (allDots[i, k] != null)
                    {
                        allDots[i, k].GetComponent<Dot>().row = j + (int)boardDotOffset.y;
                        allDots[i, k] = null;
                        break;
                    }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }


    private void RefillBoard()
    {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (allDots[i, j] == null && !blankSpaces[i, j]&& !concreteTiles[i,j]&& !slimeTiles[i,j])
            {
                var tempPosition = new Vector2(i - width / 2, j - height / 2 + offset);
                var dotToUse = Random.Range(0, Dots.Count);
                var maxIterations = 0;
                while (MatchesAt(i, j, Dots[dotToUse]) && maxIterations < 10)
                {
                    maxIterations++;
                    dotToUse = Random.Range(0, Dots.Count);
                }

                maxIterations = 0;
                var piece = Instantiate(Dots[dotToUse], tempPosition, quaternion.identity);

                piece.GetComponent<Dot>().row = j - (int)boardDotOffset.y;
                piece.GetComponent<Dot>().column = i - (int)boardDotOffset.x;
                allDots[i, j] = piece;
            }
    }

    private bool MatchesOnBoard()
    {
        FindMatches.instance.findAllMatches();
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (allDots[i, j] != null)
                if (allDots[i, j].GetComponent<Dot>().isMatched)
                    return true;

        return false;
    }

    private IEnumerator FillBoardCo()
    {
        
        yield return new WaitForSeconds(0.5f);
        RefillBoard();
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
           yield break;
            // yield return new WaitForSeconds(0.5f);
        }

       
        CurrentDot = null;
        checkToMakeSlime();
       
        if (isDeadLock()) StartCoroutine(shuffleBoard());
        CurrentState = GameState.move;
        makeSlime = true;
        streakValue = 1;
    }

    private void checkToMakeSlime()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (slimeTiles[i,j]!=null&& makeSlime)
                {
                    
                    makeNewSlime();
                    return;
                }
            }
        }
    }

    private Vector2 CheckForAdjacent(int column,int row)
    {
        if(column < width - 1 && allDots[column+1,row] )
        {
            return Vector2.right;
        }

        if (column > 0 && allDots[column - 1, row] )
        {
            return Vector2.left;
        }
        if (row < height - 1 && allDots[column , row+1] )
        {
            return Vector2.up;
        }
        if (row > 0 && allDots[column, row-1] )
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    private void makeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        while (!slime&& loops<200)
        {
            loops++;
            int newX=Random.Range(0,width);
            int newY=Random.Range(0,height);
            if (slimeTiles[newX,newY])
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                if (adjacent!=Vector2.zero)
                {
                    Destroy(allDots[newX+(int)adjacent.x,newY+(int)adjacent.y]);
                    allDots[newX + (int)adjacent.x, newY + (int)adjacent.y] = null;
                    Vector2 temp = new Vector2(newX + (int)adjacent.x - width / 2, newY + (int)adjacent.y - height / 2);
                    GameObject tile = Instantiate(slimeTilePrefab, temp, quaternion.identity);
                    slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                    slime = true;
                }
            }
        }
    }

    public void switchPieces(int column, int row, Vector2 direction)
    {
        if (allDots[column+(int)direction.x,row+(int)direction.y]!=null)
        { 
            var holder = allDots[column + (int)direction.x, row + (int)direction.y]; 
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row]; 
            allDots[column, row] = holder; 
        }
    }

    private bool checkForMatches()
    {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (allDots[i, j] != null)
            {
                if (i < width - 2)
                    if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                            allDots[i + 2, j].tag == allDots[i, j].tag)
                            return true;

                if (j < height - 2)
                    if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                            allDots[i, j + 2].tag == allDots[i, j].tag)
                            return true;
            }

        return false;
    }

    public bool switchAndCheck(int column, int row, Vector2 direction)
    {
        switchPieces(column, row, direction);
        if (checkForMatches())
        {
            switchPieces(column, row, direction);
            return true;
        }

        switchPieces(column, row, direction);
        return false;
    }

    private bool isDeadLock()
    {
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (allDots[i, j] != null)
            {
                if (i < width - 1)
                    if (switchAndCheck(i, j, Vector2.right))
                        return false;

                if (j < height - 1)
                    if (switchAndCheck(i, j, Vector2.up))
                        return false;
            }

        return true;
    }

    private IEnumerator shuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        var newBoard = new List<GameObject>();
        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (allDots[i, j] != null)
                newBoard.Add(allDots[i, j]);

        for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
            if (!blankSpaces[i, j]&& !concreteTiles[i,j]&& !slimeTiles[i,j])
            {
                var pieceToUse = Random.Range(0, newBoard.Count);

                var maxiterations = 0;
                while (MatchesAt(i, j, newBoard[pieceToUse]) && maxiterations < 100)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxiterations++;
                }

                var piece = newBoard[pieceToUse].GetComponent<Dot>();
                maxiterations = 0;
                piece.column = i + (int)boardDotOffset.x;
                piece.row = j + (int)boardDotOffset.y;
                allDots[i, j] = newBoard[pieceToUse];
                newBoard.Remove(newBoard[pieceToUse]);
            }

        if (isDeadLock()) StartCoroutine(shuffleBoard());
    }
}