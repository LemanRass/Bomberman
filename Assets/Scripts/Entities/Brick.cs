using UnityEngine;

public class Brick
{
    public DBBrick data;
    public Vector2 pos;
    public Vector2Int coords;

    public Brick(DBBrick data, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
        this.coords = coords;
    }

    public override string ToString()
    {
        return $"[Wall] Position ({pos.x}, {pos.y});";
    }
}
