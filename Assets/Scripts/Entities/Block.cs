using UnityEngine;

//Undestroyable block
public class Block
{
    public DBBlock data;
    public Vector2 pos;

    public Block(DBBlock data, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Block] Position ({pos.x}, {pos.y})";
    }
}
