using System;
using UnityEngine;

[Serializable]
public class Block
{
    public DBBlock data;
    public Vector2Int coords;
    public Vector2 pos;

    public Block(DBBlock data, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.coords = coords;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Block] Position ({pos.x}, {pos.y})";
    }
}
