using System;
using UnityEngine;

[Serializable]
public class Player
{
    public int id;
    public DBPlayer data;
    public Vector2 pos;
    public bool isAI;

    public float moveSpeed
    {
        get
        {
            return Calculations.CalcMoveSpeed(this);
        }
    }

    public int bombsLimit
    {
        get
        {
            return Calculations.CalcBombsCountLimit(this);
        }
    }

    public int powerLimit
    {
        get
        {
            return Calculations.CalcBombExplosionPower(this);
        }
    }

    public PowerUpsData powerUps;

    public Player(int id, DBPlayer data, Vector2 pos, bool isAI)
    {
        this.id = id;
        this.data = data;
        this.pos = pos;
        this.isAI = isAI;

        powerUps = new PowerUpsData();
    }

    public void PickUpPowerUp(PowerUPType type)
    {
        powerUps.PickUp(type);
    }
}
