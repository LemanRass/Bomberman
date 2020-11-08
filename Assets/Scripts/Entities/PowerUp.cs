using UnityEngine;

public class PowerUP
{
    public DBPowerUP data;
    public Vector2 pos;
    public Vector2Int coords;

    public PowerUP(DBPowerUP data, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
        this.coords = coords;
    }

    public override string ToString()
    {
        return $"[PowerUp] Type: {data.type} Position ({pos.x}, {pos.y}).";
    }
}
