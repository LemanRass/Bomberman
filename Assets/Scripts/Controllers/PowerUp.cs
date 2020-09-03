using UnityEngine;

public class PowerUp
{
    public PowerUPType type;
    public Vector2Int pos;

    public PowerUp(PowerUPType type, Vector2Int pos)
    {
        this.type = type;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[PowerUp] Type: {type} Position ({pos.x}, {pos.y}).";
    }
}
