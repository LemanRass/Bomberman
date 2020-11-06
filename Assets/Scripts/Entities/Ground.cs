using UnityEngine;

public class Ground
{
    public DBGround data;
    public Vector2 pos;

    public Ground(DBGround data, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[Block] Position ({pos.x}, {pos.y})";
    }
}
