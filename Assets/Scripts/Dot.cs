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

    public bool isColorBomb;
    public bool isCollumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    [SerializeField] private GameObject rowArrow;
    [SerializeField] private GameObject collumnArrow;
    [SerializeField] private GameObject colorBomb;
    [SerializeField] private GameObject adjacentBomb;
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

            if (isColorBomb)
            {
                FindMatches.instance.GetColorPieces(otherDot.GetComponent<SpriteRenderer>(),this.gameObject);
            }

            if (isAdjacentBomb)
            {
                FindMatches.instance.getAdjacentPieces(column-boardOffsetX,row-boardOffsetY);
            }
        }
        else
        {
            Board.instance.CurrentState = GameState.move;
            
        }
        
        
        
    }

    public void movePiecesActual(Vector2 direction)
    {
        otherDot = (column - (int)Board.instance.boardDotOffset.x + (int)direction.x >= 0 &&
                    column - (int)Board.instance.boardDotOffset.x + (int)direction.x < Board.instance.allDots.GetLength(0) &&
                    row - (int)Board.instance.boardDotOffset.y + (int)direction.y >= 0 &&
                    row - (int)Board.instance.boardDotOffset.y + (int)direction.y < Board.instance.allDots.GetLength(1))
            ? Board.instance.allDots[column - (int)Board.instance.boardDotOffset.x + (int)direction.x,
                row - (int)Board.instance.boardDotOffset.y + (int)direction.y]
            : null;

        if (otherDot!=null)
        {
            previousRow = row ;
            previousColumn = column ;
            otherDot.GetComponent<Dot>().column+=-1* (int)direction.x;
            otherDot.GetComponent<Dot>().row+=-1* (int)direction.y;
            row += (int)direction.y;
            column += (int)direction.x;
        }
       
       
    }
    
    public void movePieces()
    {
        if (swipdeAngel>-45&& swipdeAngel<=45&&column<Board.instance.width-1)
        {
            movePiecesActual(Vector2.right);

        }
        else if (swipdeAngel>45&& swipdeAngel<=135&& row<Board.instance.height-1)
        {
            movePiecesActual(Vector2.up);

        }
        else if ((swipdeAngel>135 || swipdeAngel<=-135)&& column-Board.instance.boardDotOffset.x>0)
        {
            movePiecesActual(Vector2.left);

        }
        else if (swipdeAngel<-45&& swipdeAngel>=-135&& row-Board.instance.boardDotOffset.y>0)
        {
            movePiecesActual(Vector2.down);

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
        if (Targety>Mathf.Floor(Board.instance.height/2))
        {
            Targety -= Board.instance.height+1;
            row -= Board.instance.height + 1;
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
        else
        {
            Board.instance.CurrentState = GameState.move;
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

    public void makeColorBomb()
    {
        isColorBomb = true;
        GameObject bomb = Instantiate(colorBomb, transform.position, quaternion.identity);
        bomb.transform.parent = this.transform;
    }

    public void makeAdjacentBomb()
    {
        isAdjacentBomb = true;
        GameObject adjacent = Instantiate(adjacentBomb, transform.position, quaternion.identity);
        adjacent.transform.parent = this.transform;
    }
}


