using UnityEngine;

public class Brick
{
    public Vector2 pos;

    public Brick(Vector2 pos)
    {
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Wall] Position ({pos.x}, {pos.y});";
    }
}
