using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;
    [SerializeField] private float hintDelay;
    private float hintDelaySeconds;
    [SerializeField] private GameObject hintParticle;
    public GameObject currentHint;
    void Start()
    {
        instance = this;
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds<=0&& currentHint==null)
        {
            markHint();
            hintDelaySeconds = hintDelay;
        }
    }

    List<GameObject> findAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < Board.instance.width; i++)
        {
            for (int j = 0; j < Board.instance.height; j++)
            {
                if (Board.instance.allDots[i,j]!=null)
                {
                    if (i<Board.instance.width-1)
                    {
                        if (Board.instance.switchAndCheck(i,j,Vector2.right))
                        {
                            possibleMoves.Add(Board.instance.allDots[i,j]);
                        }

                        
                    }

                    if (j<Board.instance.height-1)
                    {
                        if (Board.instance.switchAndCheck(i,j,Vector2.up))
                        {
                            possibleMoves.Add(Board.instance.allDots[i,j]);
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }
    GameObject pickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = findAllMatches();
        if (possibleMoves.Count>0)
        {
            int pickToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pickToUse];
        }

        return null;
    }

    private void markHint()
    {
        GameObject move = pickOneRandomly();
        if (move!=null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
            
        }
    }

    public void destroyHint()
    {
        if (currentHint != null) 
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
