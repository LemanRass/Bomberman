using UnityEngine;

public enum PowerUPType
{
    EXPLOSION_SIZE,
    EXTRA_BOMB
}

public class PowerUp
{
    public PowerUPType type;
    public Vector2 pos;

    public PowerUp(PowerUPType type, Vector2 pos)
    {
        this.type = type;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[PowerUp] Type: {type} Position ({pos.x}, {pos.y}).";
    }
}
