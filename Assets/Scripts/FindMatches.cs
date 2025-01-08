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
}
