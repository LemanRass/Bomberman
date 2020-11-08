using UnityEngine;

//Undestroyable block
public class Block
{
    public DBBlock data;
    public Vector2 pos;
    public Vector2Int coords;

    public Block(DBBlock data, Vector2Int coords, Vector2 pos)
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
