using System;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float aspectRatio = 0.625f;
    public float padding = 2;
    private void Start()
    {
        if (Board.instance!=null)
        {
           repositionCamera(Board.instance.width-1,Board.instance.height-1);
        }
    }

    public void repositionCamera(float x,float y)
    {
        Vector3 tempPosition = new Vector3(Mathf.Round(x / 16), Mathf.Round(y / 16),-10);
        //transform.position = tempPosition;
        if (Board.instance.width>=Board.instance.height)
        {
            Camera.main.orthographicSize = (Board.instance.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = Board.instance.height / 2 + padding;
        }
    }
    
}
