using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database instance { get; private set; }

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

    public static DBBomb GetBomb(int id)
    {
        return instance.bombs.Find(n => n.id.Equals(id));
    }

    public static DBPowerUP GetPowerUP(PowerUPType type)
    {
        return instance.powerUps.Find(n => n.type.Equals(type));
    }
}
