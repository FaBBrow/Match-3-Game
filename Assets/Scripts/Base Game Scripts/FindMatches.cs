using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public static FindMatches instance;
    public List<GameObject> CurrentMatches = new();

    private void Start()
    {
        instance = this;
    }

    public void findAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }


    private IEnumerator FindAllMatchesCo()
    {
       // yield return new WaitForSeconds(0.2f);
       yield return null;
       for (var i = 0; i < Board.instance.width; i++)
        for (var j = 0; j < Board.instance.height; j++)
        {
            var currentDot = Board.instance.allDots[i, j] != null
                ? Board.instance.allDots[i, j].GetComponent<Dot>()
                : null;

            if (currentDot != null)
            {
                if (i > 0 && i < Board.instance.width - 1)
                {
                    var leftDot = Board.instance.allDots[i - 1, j] != null
                        ? Board.instance.allDots[i - 1, j].GetComponent<Dot>()
                        : null;

                    var rightDot = Board.instance.allDots[i + 1, j] != null
                        ? Board.instance.allDots[i + 1, j].GetComponent<Dot>()
                        : null;

                    if (leftDot != null && rightDot != null)
                        if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                        {
                            if (!CurrentMatches.Contains(leftDot.gameObject)) CurrentMatches.Add(leftDot.gameObject);
                            leftDot.isMatched = true;
                            if (!CurrentMatches.Contains(currentDot.gameObject))
                                CurrentMatches.Add(currentDot.gameObject);
                            currentDot.isMatched = true;
                            if (!CurrentMatches.Contains(rightDot.gameObject)) CurrentMatches.Add(rightDot.gameObject);
                            rightDot.isMatched = true;
                        }
                }

                if (j > 0 && j < Board.instance.height - 1)
                {
                    var upperDot = Board.instance.allDots[i, j + 1] != null
                        ? Board.instance.allDots[i, j + 1].GetComponent<Dot>()
                        : null;
                    var downDot = Board.instance.allDots[i, j - 1] != null
                        ? Board.instance.allDots[i, j - 1].GetComponent<Dot>()
                        : null;

                    if (upperDot != null && downDot != null)
                        if (upperDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                        {
                            if (!CurrentMatches.Contains(upperDot.gameObject)) CurrentMatches.Add(upperDot.gameObject);
                            upperDot.isMatched = true;
                            if (!CurrentMatches.Contains(currentDot.gameObject))
                                CurrentMatches.Add(currentDot.gameObject);
                            currentDot.isMatched = true;
                            if (!CurrentMatches.Contains(downDot.gameObject)) CurrentMatches.Add(downDot.gameObject);
                            downDot.isMatched = true;
                        }
                }
            }
        }
    }


    public List<GameObject> getColumnPieces(int column)
    {
        var dots = new List<GameObject>();
        for (var i = 0; i < Board.instance.height; i++)
            if (Board.instance.allDots[column, i] != null)
            {
                var dot = Board.instance.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb) getRowPieces(i);
                Debug.Log("çalısıyor");
                dots.Add(Board.instance.allDots[column, i]);
                CurrentMatches.Add(Board.instance.allDots[column, i]);
                Board.instance.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }

        CurrentMatches.Clear();
        return dots;
    }

    public List<GameObject> getRowPieces(int row)
    {
        var dots = new List<GameObject>();
        for (var i = 0; i < Board.instance.width; i++)
            if (Board.instance.allDots[i, row] != null)
            {
                var dot = Board.instance.allDots[i, row].GetComponent<Dot>();
                if (dot.isCollumnBomb) getColumnPieces(i);

                // dots.Add(Board.instance.allDots[i,row]);
                CurrentMatches.Add(Board.instance.allDots[i, row]);
                Board.instance.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }

        CurrentMatches.Clear();
        return dots;
    }

    public void GetColorPieces(SpriteRenderer sprite1, GameObject sprited)
    {
        for (var i = 0; i < Board.instance.width; i++)
        for (var j = 0; j < Board.instance.height; j++)
            if (Board.instance.allDots[i, j] != null)
                if (Board.instance.allDots[i, j].GetComponent<SpriteRenderer>().sprite == sprite1.sprite)
                {
                    CurrentMatches.Add(Board.instance.allDots[i, j]);
                    Board.instance.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }

        CurrentMatches.Add(sprited);
        sprited.GetComponent<Dot>().isMatched = true;
        CurrentMatches.Clear();
    }

    public void checkBombs(MatchType matchType)
    {
        var currentdot = Board.instance.CurrentDot != null ? Board.instance.CurrentDot.GetComponent<Dot>() : null;
        var otherDot = Board.instance.CurrentDot != null && Board.instance.CurrentDot.otherDot != null
            ? Board.instance.CurrentDot.otherDot.GetComponent<Dot>()
            : null;

        if (currentdot != null)
        {
            if (currentdot.isMatched&& currentdot.tag==matchType.color)
            {
                currentdot.isMatched = false;

                if ((currentdot.swipdeAngel > -45 && currentdot.swipdeAngel <= 45) ||
                    (currentdot.swipdeAngel < -135 && currentdot.swipdeAngel >= 135))
                    currentdot.makeRowBomb();
                else
                    currentdot.makeColumnBomb();
            }
            else if (otherDot != null)
            {
                if (otherDot.isMatched&& otherDot.tag==matchType.color)
                {
                    otherDot.isMatched = false;


                    if ((currentdot.swipdeAngel > -45 && currentdot.swipdeAngel <= 45) ||
                        (currentdot.swipdeAngel < -135 && currentdot.swipdeAngel >= 135))
                        otherDot.makeRowBomb();
                    else
                        otherDot.makeColumnBomb();
                }
            }
        }
    }

    public void checkColorBombs()
    {
        var currentdot = Board.instance.CurrentDot != null ? Board.instance.CurrentDot.GetComponent<Dot>() : null;
        var otherDot = Board.instance.CurrentDot != null && Board.instance.CurrentDot.otherDot != null
            ? Board.instance.CurrentDot.otherDot.GetComponent<Dot>()
            : null;

        if (currentdot != null && currentdot.isMatched && currentdot.tag == Board.instance.matchType.color)
        {

            currentdot.isMatched = false;
            currentdot.makeColorBomb();
        }
        else if (otherDot != null)
        {
            if (otherDot.isMatched&& otherDot.tag==Board.instance.matchType.color) {
                otherDot.isMatched = false;
                otherDot.makeColorBomb(); }
        }
       
    }

    public void checkAdjacentBomb()
    {
        var currentdot = Board.instance.CurrentDot != null ? Board.instance.CurrentDot.GetComponent<Dot>() : null;
        var otherDot = Board.instance.CurrentDot != null && Board.instance.CurrentDot.otherDot != null
            ? Board.instance.CurrentDot.otherDot.GetComponent<Dot>()
            : null;

        if (currentdot != null && currentdot.isMatched && currentdot.tag == Board.instance.matchType.color)
        {

            currentdot.isMatched = false;
            currentdot.makeAdjacentBomb();
        }
        else if (otherDot != null)
        {
            if (otherDot.isMatched&& otherDot.tag==Board.instance.matchType.color)
            {
                otherDot.isMatched = false;
                otherDot.makeAdjacentBomb();
            }
        }
        
    }

    public void getAdjacentPieces(int column, int row)
    {
        for (var i = column - 1; i < column + 2; i++)
        for (var j = row - 1; j < row + 2; j++)
            if (i >= 0 && i < Board.instance.width && j >= 0 && j < Board.instance.height)
                if (!Board.instance.blankSpaces[i, j])
                {
                    CurrentMatches.Add(Board.instance.allDots[i, j]);
                    Board.instance.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }

        CurrentMatches.Clear();
    }
}