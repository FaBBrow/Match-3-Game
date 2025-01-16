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

   

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < Board.instance.width; i++)
        {
            for (int j = 0; j < Board.instance.height; j++)
            {
                Dot currentDot = Board.instance.allDots[i, j] != null ? Board.instance.allDots[i, j].GetComponent<Dot>() : null;

                if (currentDot!=null)
                {
                    if (i>0&& i<Board.instance.width-1)
                    {
                        Dot leftDot = Board.instance.allDots[i - 1, j] != null ? Board.instance.allDots[i - 1, j].GetComponent<Dot>() : null;

                        Dot rightDot = Board.instance.allDots[i + 1, j] != null ? Board.instance.allDots[i + 1, j].GetComponent<Dot>() : null;

                        if (leftDot!=null&& rightDot!=null)
                        {
                            if (leftDot.tag==currentDot.tag&& rightDot.tag==currentDot.tag)
                            {
                            
                                if (!CurrentMatches.Contains(leftDot.gameObject))
                                {
                                    CurrentMatches.Add(leftDot.gameObject);
                                }
                                leftDot.isMatched = true;
                                if (!CurrentMatches.Contains(currentDot.gameObject))
                                {
                                    CurrentMatches.Add(currentDot.gameObject);
                                }
                                currentDot.isMatched = true;
                                if (!CurrentMatches.Contains(rightDot.gameObject))
                                {
                                    CurrentMatches.Add(rightDot.gameObject);
                                }
                                rightDot.isMatched = true;
                            }
                        }
                    }
                    if (j>0&& j<Board.instance.height-1)
                    {
                        Dot upperDot = Board.instance.allDots[i, j + 1] != null ? Board.instance.allDots[i, j + 1].GetComponent<Dot>() : null;
                        Dot downDot = Board.instance.allDots[i, j - 1] != null ? Board.instance.allDots[i, j - 1].GetComponent<Dot>() : null;

                        if (upperDot!=null&& downDot!=null)
                        {
                            
                            if (upperDot.tag==currentDot.tag&& downDot.tag==currentDot.tag)
                            {
                             
                                if (!CurrentMatches.Contains(upperDot.gameObject))
                                {
                                    CurrentMatches.Add(upperDot.gameObject);
                                }upperDot.isMatched = true;
                                if (!CurrentMatches.Contains(currentDot.gameObject))
                                {
                                    CurrentMatches.Add(currentDot.gameObject);
                                }currentDot.isMatched = true;
                                if (!CurrentMatches.Contains(downDot.gameObject))
                                {
                                    CurrentMatches.Add(downDot.gameObject);
                                }downDot.isMatched = true;
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
               // dots.Add(Board.instance.allDots[i,row]);
                CurrentMatches.Add(Board.instance.allDots[i,row]);
                Board.instance.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        CurrentMatches.Clear();
        return dots;
        
    }

    public void GetColorPieces(SpriteRenderer sprite1,GameObject sprited)
    {
        for (int i = 0; i < Board.instance.width; i++)
        {
            for (int j = 0; j < Board.instance.height; j++)
            {
                if (Board.instance.allDots[i,j]!=null)
                {
                    if (Board.instance.allDots[i,j].GetComponent<SpriteRenderer>().color==sprite1.color)
                    {
                        CurrentMatches.Add(Board.instance.allDots[i,j]);
                        Board.instance.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        CurrentMatches.Add(sprited);
        sprited.GetComponent<Dot>().isMatched = true;
        CurrentMatches.Clear();
    }

    public void checkBombs()
    {
        Dot currentdot = Board.instance.CurrentDot != null ? Board.instance.CurrentDot.GetComponent<Dot>() : null;
        Dot otherDot = Board.instance.CurrentDot != null && Board.instance.CurrentDot.otherDot != null 
            ? Board.instance.CurrentDot.otherDot.GetComponent<Dot>() 
            : null;

        if (currentdot!=null)
        {
            if (currentdot.isMatched)
            {
               
                currentdot.isMatched = false;
          
                if ((currentdot.swipdeAngel>-45&& currentdot.swipdeAngel<=45)|| currentdot.swipdeAngel<-135&& currentdot.swipdeAngel>=135)
                {
                    currentdot.makeRowBomb();
                }
                else
                {
                    currentdot.makeColumnBomb();
                }
            }
            else if (otherDot!=null)
            {
                
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                   
                   
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
    public void checkColorBombs()
    {
        Dot currentdot = Board.instance.CurrentDot != null ? Board.instance.CurrentDot.GetComponent<Dot>() : null;
        Dot otherDot = Board.instance.CurrentDot != null && Board.instance.CurrentDot.otherDot != null 
            ? Board.instance.CurrentDot.otherDot.GetComponent<Dot>() 
            : null;

        if (currentdot!=null)
        {
            if (currentdot.isMatched)
            {
               
                currentdot.isMatched = false;
                currentdot.makeColorBomb();
            }
            else if (otherDot!=null)
            {
               
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                  
                   otherDot.makeColorBomb();
                }
            }
        }
    }

    public void checkAdjacentBomb()
    {
        
        Dot currentdot = Board.instance.CurrentDot != null ? Board.instance.CurrentDot.GetComponent<Dot>() : null;
        Dot otherDot = Board.instance.CurrentDot != null && Board.instance.CurrentDot.otherDot != null 
            ? Board.instance.CurrentDot.otherDot.GetComponent<Dot>() 
            : null;

        if (currentdot!=null)
        {
            if (currentdot.isMatched)
            {
               
                currentdot.isMatched = false;
                currentdot.makeAdjacentBomb();
            }
            else if (otherDot!=null)
            {
               
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                  
                    otherDot.makeAdjacentBomb();
                }
            }
        }
    }

    public void getAdjacentPieces(int column,int row)
    {
        for (int i = column - 1; i < column + 2; i++)
        {
            for (int j = row - 1; j < row + 2; j++)
            {
                if (i>=0&&i<Board.instance.width&&j>=0&& j<Board.instance.height)
                {
                    CurrentMatches.Add(Board.instance.allDots[i, j]);
                    Board.instance.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }
            }
        }
        CurrentMatches.Clear();
    }
}
