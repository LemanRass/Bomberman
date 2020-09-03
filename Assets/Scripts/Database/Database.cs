using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database instance { get; private set; }

    [SerializeField]
    private List<DBPowerUP> m_powerUps;

    public Dictionary<PowerUPType, DBPowerUP> powerUps;

    private void Awake()
    {
        instance = this;
        powerUps = new Dictionary<PowerUPType, DBPowerUP>();
        m_powerUps.ForEach(n => powerUps.Add(n.type, n));
    }

    public static DBPowerUP PowerUP(PowerUPType type)
    {
        return instance.powerUps[type];
    }
}
