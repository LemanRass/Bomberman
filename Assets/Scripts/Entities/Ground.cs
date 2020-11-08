using System;
using UnityEngine;

[Serializable]
public class Ground
{
    public DBGround data;
    public Vector2 pos;
    public Vector2Int coords;

    public Ground(DBGround data, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
        this.coords = coords;
    }

    public override string ToString()
    {
        return $"[Block] Position ({pos.x}, {pos.y})";
    }
}
