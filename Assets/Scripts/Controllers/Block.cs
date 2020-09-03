using UnityEngine;

//Undestroyable block
public class Block
{
    public Vector2Int pos;
    public Vector3 pos3
    {
        get { return new Vector3(pos.x, 0, pos.y); }
    }

    public Block(int x, int y)
    {
        pos = new Vector2Int(x, y);
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
