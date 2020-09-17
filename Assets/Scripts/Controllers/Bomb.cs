using UnityEngine;

public class Bomb
{
    public DBBomb data;
    public Vector2 pos;

    public float spawnTimestamp;
    public float explosionTimestamp;

    public int power = 1;

    public Bomb(DBBomb data, int power, Vector2 pos)
    {
        this.data = data;
        this.pos = pos;
        this.power = power;

        spawnTimestamp = Time.time;
        explosionTimestamp = spawnTimestamp + data.delay;
    }

    public bool IsReady()
    {
        return Time.time >= explosionTimestamp;
    }
}
