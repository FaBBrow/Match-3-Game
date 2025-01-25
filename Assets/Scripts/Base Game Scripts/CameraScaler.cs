using System;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float aspectRatio = 0.625f;
    public float padding = 2;
    public float yOffset = 1;
    private void Start()
    {
        if (Board.instance!=null)
        {
           repositionCamera(Board.instance.width-1,Board.instance.height-1);
        }
    }

    public void repositionCamera(float x,float y)
    {
        if (Board.instance.width%2==0)
        {
            Vector3 temp = new Vector3(transform.position.x - 0.5f, transform.position.y,-10);
           transform.position = temp;
        }
        if (Board.instance.width>=Board.instance.height)
        {
            Camera.main.orthographicSize = (Board.instance.width / 2 + padding+yOffset) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = Board.instance.height / 2 + padding+yOffset;
        }
    }
    
}
