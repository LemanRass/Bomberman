using System;

public enum ExplosionType
{
    Basic
}

[Serializable]
public class DBExplosion
{
    public ExplosionType type;
    public string prefab;
}
