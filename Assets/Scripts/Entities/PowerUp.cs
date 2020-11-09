using System;
using UnityEngine;

[Serializable]
public class PowerUP
{
    public DBPowerUP data;
    public Vector2Int coords;
    public Vector2 pos;

    public PowerUP(DBPowerUP data, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.coords = coords;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[PowerUp] Type: {data.type} Position ({pos.x}, {pos.y}).";
    }
}
