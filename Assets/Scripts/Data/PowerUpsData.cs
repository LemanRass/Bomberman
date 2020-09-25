using System.Collections.Generic;

public class PowerUpsData
{
    public Dictionary<PowerUPType, int> data { get; private set; }

    public PowerUpsData()
    {
        data = new Dictionary<PowerUPType, int>();
    }

    public void PickUp(PowerUPType type)
    {
        if(data.ContainsKey(type))
        {
            data[type]++;
        }
        else
        {
            data.Add(type, 1);
        }
    }

    public int Count(PowerUPType type)
    {
        if (data.ContainsKey(type))
        {
            return data[type];
        }
        else
        {
            return 0;
        }
    }
}