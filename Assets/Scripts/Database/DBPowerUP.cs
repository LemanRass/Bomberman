using System;
using UnityEngine;

public enum PowerUPType
{
    EXPLOSION_SIZE,
    EXTRA_BOMB,
    MOVE_SPEED
}

[Serializable]
public class DBPowerUP
{
    public string name;
    public string prefab;
    public Texture2D icon;
    public PowerUPType type;
}
