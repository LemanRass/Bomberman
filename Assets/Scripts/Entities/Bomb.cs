using UnityEngine;

public class Bomb
{
    public Player owner;
    public DBBomb data;
    public Vector2 pos;

    public float spawnTimestamp;
    public float explosionTimestamp;

    public bool isReady = false;
    public int power = 1;

    public Bomb(DBBomb data, Player owner, Vector2 pos)
    {
        this.data = data;
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
