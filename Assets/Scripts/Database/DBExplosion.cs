using System;

public enum ExplosionType
{
    Basic,
    Brick
}

[Serializable]
public class DBExplosion
{
    public ExplosionType type;
    public string prefab;
}
