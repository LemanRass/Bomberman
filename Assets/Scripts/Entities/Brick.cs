using System;
using UnityEngine;

[Serializable]
public class Brick
{
    public DBBrick data;
    public Vector2Int coords;
    public Vector2 pos;

    public Brick(DBBrick data, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.coords = coords;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Wall] Position ({pos.x}, {pos.y});";
    }
}
