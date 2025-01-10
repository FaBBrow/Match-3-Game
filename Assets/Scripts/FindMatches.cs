using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public static FindMatches instance ;
    public List<GameObject> CurrentMatches = new List<GameObject>();
    void Start()
    {
        instance = this;
    }

    public void findAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    public void findBombs()
    {
        //StartCoroutine(Bombs());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < Board.instance.width; i++)
        {
            for (int j = 0; j < Board.instance.height; j++)
            {
                GameObject currentDot = Board.instance.allDots[i, j];
                if (currentDot!=null)
                {
                    if (i>0&& i<Board.instance.width-1)
                    {
                        GameObject leftDot = Board.instance.allDots[i - 1, j];
                        GameObject rightDot = Board.instance.allDots[i + 1, j];
                        if (leftDot!=null&& rightDot!=null)
                        {
                            if (leftDot.tag==currentDot.tag&& rightDot.tag==currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isRowBomb|| leftDot.GetComponent<Dot>().isRowBomb|| rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    CurrentMatches.Union(getRowPieces(j));
                                }

                                if (currentDot.GetComponent<Dot>().isCollumnBomb)
                                {
                                    CurrentMatches.Union(getColumnPieces(i));
                                }
                                if (leftDot.GetComponent<Dot>().isCollumnBomb)
                                {
                                    CurrentMatches.Union(getColumnPieces(i-1));
                                }
                                if (rightDot.GetComponent<Dot>().isCollumnBomb)
                                {
                                    CurrentMatches.Union(getColumnPieces(i+1));
                                }
                                
                                if (!CurrentMatches.Contains(leftDot))
                                {
                                    CurrentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!CurrentMatches.Contains(currentDot))
                                {
                                    CurrentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                                if (!CurrentMatches.Contains(rightDot))
                                {
                                    CurrentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                    if (j>0&& j<Board.instance.height-1)
                    {
                        GameObject upperDot = Board.instance.allDots[i , j+1];
                        GameObject downDot = Board.instance.allDots[i , j-1];
                        if (upperDot!=null&& downDot!=null)
                        {
                            
                            if (upperDot.tag==currentDot.tag&& downDot.tag==currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isCollumnBomb|| upperDot.GetComponent<Dot>().isCollumnBomb|| downDot.GetComponent<Dot>().isCollumnBomb)
                                {
                                    CurrentMatches.Union(getColumnPieces(i));
                                }
                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                {
                                    CurrentMatches.Union(getRowPieces(j));
                                }
                                if (upperDot.GetComponent<Dot>().isRowBomb)
                                {
                                    CurrentMatches.Union(getRowPieces(j+1));
                                }
                                if (downDot.GetComponent<Dot>().isRowBomb)
                                {
                                    CurrentMatches.Union(getRowPieces(j-1));
                                }
                                if (!CurrentMatches.Contains(upperDot))
                                {
                                    CurrentMatches.Add(upperDot);
                                }upperDot.GetComponent<Dot>().isMatched = true;
                                if (!CurrentMatches.Contains(currentDot))
                                {
                                    CurrentMatches.Add(currentDot);
                                }currentDot.GetComponent<Dot>().isMatched = true;
                                if (!CurrentMatches.Contains(downDot))
                                {
                                    CurrentMatches.Add(downDot);
                                }downDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    

    public List<GameObject> getColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < Board.instance.height; i++)
        {
            if (Board.instance.allDots[column,i]!=null)
            {
                Debug.Log("çalısıyor");
                dots.Add(Board.instance.allDots[column,i]);
                CurrentMatches.Add(Board.instance.allDots[column,i]);
                Board.instance.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        
        CurrentMatches.Clear();
        return dots;
        
    }
    public List<GameObject> getRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < Board.instance.width; i++)
        {
            if (Board.instance.allDots[i,row]!=null)
            {
                dots.Add(Board.instance.allDots[i,row]);
                CurrentMatches.Add(Board.instance.allDots[i,row]);
                Board.instance.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        CurrentMatches.Clear();
        return dots;
        
    }

    public void checkBombs()
    {
        if (Board.instance.CurrentDot!=null)
        {
            if (Board.instance.CurrentDot.isMatched)
            {
                Dot currentdot = Board.instance.CurrentDot;
                Board.instance.CurrentDot.isMatched = false;
                /*int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb<50)
                {
                    Board.instance.CurrentDot.makeRowBomb();
                }
                else
                {
                    Board.instance.CurrentDot.makeColumnBomb();
                }*/
                if ((currentdot.swipdeAngel>-45&& currentdot.swipdeAngel<=45)|| currentdot.swipdeAngel<-135&& currentdot.swipdeAngel>=135)
                {
                    currentdot.makeRowBomb();
                }
                else
                {
                    currentdot.makeColumnBomb();
                }
            }
            else if (Board.instance.CurrentDot.otherDot!=null)
            {
                Dot otherDot = Board.instance.CurrentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                   /* int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb<50)
                    {
                        otherDot.makeRowBomb();
                    }
                    else
                    {
                        otherDot.makeColumnBomb();
                    }*/
                   Dot currentdot = Board.instance.CurrentDot.GetComponent<Dot>();
                   if ((currentdot.swipdeAngel>-45&& currentdot.swipdeAngel<=45)|| currentdot.swipdeAngel<-135&& currentdot.swipdeAngel>=135)
                   {
                       otherDot.makeRowBomb();
                   }
                   else
                   {
                       otherDot.makeColumnBomb();
                   }
                }
            }
        }
    }
}
