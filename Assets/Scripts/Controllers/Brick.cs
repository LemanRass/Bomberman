using UnityEngine;

public class Brick
{
    public DBBrick data;
    public Vector2 pos;

    public Brick(DBBrick data, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Wall] Position ({pos.x}, {pos.y});";
    }
}
