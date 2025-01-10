using System;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int Targetx;
    public int Targety;
    public int previousColumn;
    public int previousRow;
    public int boardOffsetX;
    public int boardOffsetY;
    public int column;
    public int row;
    public bool isMatched = false;
    public bool slideing=true;
    
    public GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosiiton;
    public float swipdeAngel=0;
    public float swipeResist = .5f;
    

    public bool isCollumnBomb;
    public bool isRowBomb;
    [SerializeField] private GameObject rowArrow;
    [SerializeField] private GameObject collumnArrow;
    
    private void Start()
    {
        boardOffsetX = (int)Board.instance.boardDotOffset.x;
        boardOffsetY = (int)Board.instance.boardDotOffset.y;
        Targetx=Mathf.RoundToInt(this.transform.position.x);
        Targety= Mathf.RoundToInt(this.transform.position.y);
        column = Targetx;
        row = Targety;
        previousRow = row ;
        previousColumn = column ;

    }

    private void Update()
    {
        setNewPosition();
        
        Targetx = column;
        Targety = row;
       
        
       /*" if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0, 0, 0, .2f);
        }*/
        //FindMatchs();
        
    }

    private void OnMouseDown()
    {
        if (Board.instance.CurrentState==GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(firstTouchPosition);
        }
       
        
    }

    private void OnMouseUp()
    {
        if (Board.instance.CurrentState==GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(finalTouchPosition);
            calculateAngle();

        }
        
    }

    void calculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y-firstTouchPosition.y)>swipeResist|| Mathf.Abs(finalTouchPosition.x-firstTouchPosition.x)>swipeResist)
        {
            swipdeAngel = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x)*(180/Mathf.PI);
            Debug.Log(swipdeAngel);
            movePieces();
            Board.instance.CurrentState = GameState.wait;
            Board.instance.CurrentDot = this;
            if (isRowBomb)
            {
                FindMatches.instance.getRowPieces(row - boardOffsetY);
            }
            else if (isCollumnBomb)
            {
                FindMatches.instance.getColumnPieces(column - boardOffsetX);
            }
        }
        else
        {
            Board.instance.CurrentState = GameState.move;
            
        }
        
        
        
    }

    
    public void movePieces()
    {
        if (swipdeAngel>-45&& swipdeAngel<=45&&column<Board.instance.width-1)
        {
            otherDot = Board.instance.allDots[column-(int)Board.instance.boardDotOffset.x+1, row-(int)Board.instance.boardDotOffset.y];
            previousRow = row ;
            previousColumn = column ;
            otherDot.GetComponent<Dot>().column-= 1;
            column+= 1;

        }
        else if (swipdeAngel>45&& swipdeAngel<=135&& row<Board.instance.height-1)
        {
            otherDot = Board.instance.allDots[column-(int)Board.instance.boardDotOffset.x, row-(int)Board.instance.boardDotOffset.y+1];
            previousRow = row ;
            previousColumn = column ;
            otherDot.GetComponent<Dot>().row-= 1;
            row += 1;

        }
        else if ((swipdeAngel>135 || swipdeAngel<=-135)&& column-Board.instance.boardDotOffset.x>0)
        {
            otherDot = Board.instance.allDots[column-(int)Board.instance.boardDotOffset.x-1, row-(int)Board.instance.boardDotOffset.y];
            previousRow = row ;
            previousColumn = column ;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;

        }
        else if (swipdeAngel<-45&& swipdeAngel>=-135&& row-Board.instance.boardDotOffset.y>0)
        {
            otherDot = Board.instance.allDots[column-(int)Board.instance.boardDotOffset.x, row-(int)Board.instance.boardDotOffset.y-1];
            previousRow = row ;
            previousColumn = column ;
            otherDot.GetComponent<Dot>().row+= 1;
            row -= 1;

        }

        StartCoroutine(CheckMoveCo());
    }

   public void setNewPosition()
    {
        if (Mathf.Abs(Targetx-transform.position.x)>.1f)
        {
            tempPosiiton = new Vector2(Targetx, transform.position.y);
            
          
            transform.DOMove(tempPosiiton, 1);
            
            if (Board.instance.allDots[column-boardOffsetX,row-boardOffsetY]!=this.gameObject)
            {
                Board.instance.allDots[column - boardOffsetX, row - boardOffsetY] = this.gameObject;
            }
            FindMatches.instance.findAllMatches();
        }
       
        else 
        {
            tempPosiiton = new Vector2(Targetx, transform.position.y);
            
            transform.DOMove(tempPosiiton, 1);
           
            //gameObject.name="( " + (collumn- (int)Board.instance.boardDotOffset.x) + "," + (row- (int)Board.instance.boardDotOffset.y) + ")";
        }
        if (Mathf.Abs(Targety-transform.position.y)>.1f)
        {
            tempPosiiton = new Vector2(transform.position.x, Targety);
           
            transform.DOMove(tempPosiiton, 1);
            if (Board.instance.allDots[column-boardOffsetX,row-boardOffsetY]!=this.gameObject)
            {
                Board.instance.allDots[column - boardOffsetX, row - boardOffsetY] = this.gameObject;
            }
            FindMatches.instance.findAllMatches();
        }
        else 
        {
            tempPosiiton = new Vector2(transform.position.x, Targety);
            transform.DOMove(tempPosiiton, 1);
        }
        {
           
            
        }
        if (Targety>5)
        {
            Targety -= 11;
            row -= 11;
        }
        
    }

    public void FindMatchs()
    {
        if (column-boardOffsetX>0&& column-boardOffsetX<Board.instance.width-1)
        {
            GameObject leftDot1 = Board.instance.allDots[column - boardOffsetX - 1, row - boardOffsetY];
            GameObject rightDot1 = Board.instance.allDots[column - boardOffsetX + 1, row - boardOffsetY];
            if (leftDot1!=null&& rightDot1!=null)
            {
                if (leftDot1.tag==this.gameObject.tag&& rightDot1.tag==this.gameObject.tag&& leftDot1.tag==rightDot1.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }
        if (row-boardOffsetY>0&& row-boardOffsetY<Board.instance.height-1)
        {
            GameObject upDot1 = Board.instance.allDots[column - boardOffsetX , row - boardOffsetY+1];
            GameObject downDot1 = Board.instance.allDots[column - boardOffsetX , row - boardOffsetY-1];
            if (upDot1!=null&& downDot1!=null)
            {
                if (upDot1.tag==this.gameObject.tag&& downDot1.tag==this.gameObject.tag&& upDot1.tag==downDot1.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
            
        } 
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(1f);
        if (otherDot!=null)
        {
          
             if (!isMatched&& !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().column = column;
                otherDot.GetComponent<Dot>().row = row;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                Board.instance.CurrentDot = null;
                Board.instance.CurrentState = GameState.move;
            }
            else
            {
                Board.instance.DestroyMatches();
                
            }
            // otherDot = null;
        }
       

    }

    public void makeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void makeColumnBomb()
    {
        isCollumnBomb = true;
        GameObject arrow = Instantiate(collumnArrow, transform.position, quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}


