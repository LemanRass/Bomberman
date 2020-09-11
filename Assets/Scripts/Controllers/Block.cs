using UnityEngine;

//Undestroyable block
public class Block
{
    public Vector2 pos;

    public Block(int x, int y)
    {
        pos = new Vector2(x, y);
    }

    public Block(Vector2Int pos)
    {
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Block] Position ({pos.x}, {pos.y})";
    }
}
