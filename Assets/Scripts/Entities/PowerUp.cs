﻿using UnityEngine;

public class PowerUP
{
    public DBPowerUP data;
    public Vector2 pos;

    public PowerUP(DBPowerUP data, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
    }

    public override string ToString()
    {
        return $"[PowerUp] Type: {data.type} Position ({pos.x}, {pos.y}).";
    }
}
