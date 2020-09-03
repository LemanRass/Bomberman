using UnityEngine;

public class Brick
{
    public Vector2Int pos;

    public Brick(Vector2Int pos)
    {
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Wall] Position ({pos.x}, {pos.y});";
    }
}
