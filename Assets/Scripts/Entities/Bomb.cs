using System;
using UnityEngine;

[Serializable]
public class Bomb
{
    public Player owner;
    public DBBomb data;
    public Vector2Int coords;
    public Vector2 pos;

    public float spawnTimestamp;
    public float explosionTimestamp;

    public bool isReady = false;
    public int power = 1;

    public Bomb(DBBomb data, Player owner, Vector2Int coords, Vector2 pos)
    {
        this.data = data;
        this.coords = coords;
        this.pos = pos;
        this.owner = owner;
        this.power = owner.powerLimit;

        spawnTimestamp = Time.time;
        explosionTimestamp = spawnTimestamp + data.delay;
    }

    public bool IsReady()
    {
        return Time.time >= explosionTimestamp || isReady;
    }

    public void SetReady()
    {
        isReady = true;
    }
}
