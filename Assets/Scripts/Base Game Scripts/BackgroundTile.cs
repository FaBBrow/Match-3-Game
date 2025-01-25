using UnityEngine;

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
        if (hitPoints <= 0)
        {
            Destroy(gameObject);
            GoalManager.instance.compareGoal(gameObject.tag);
            GoalManager.instance.updateGoals();
        }
    }

    public void takeDamage(int damage)
    {
        hitPoints -= damage;
        makeLighter();
    }

    public void makeLighter()
    {
        var color = sprite.color;
        var newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}