using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackgroundTile : MonoBehaviour
{

    public int hitPoints;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (hitPoints<=0)
        {
            Destroy(this.gameObject);
        }
    }

    public void takeDamage(int damage)
    {
        hitPoints -= damage;
        makeLighter();
    }

    public void makeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }


}