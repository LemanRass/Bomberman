using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database instance { get; private set; }

    public List<DBBlock> blocks;

    public List<DBBrick> bricks;

    public List<DBExplosion> explosions;

    public List<DBPowerUP> powerUps;

    public List<DBBomb> bombs;


    private void Awake()
    {
        instance = this;
    }

    public static DBExplosion GetExplosion(ExplosionType type)
    {
        return instance.explosions.Find(n => n.type.Equals(type));
    }

    public static DBPowerUP GetPowerUP(PowerUPType type)
    {
        return instance.powerUps.Find(n => n.type.Equals(type));
    }
}
