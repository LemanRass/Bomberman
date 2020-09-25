using UnityEngine;

public class PowerUPView : MonoBehaviour
{
    [HideInInspector]
    public PowerUP powerUP;

    public void Init(PowerUP powerUP)
    {
        this.powerUP = powerUP;
        float x = -(powerUP.pos.x * transform.localScale.x);
        float z = powerUP.pos.y * transform.localScale.z;
        transform.localPosition = new Vector3(x, 0.05f, z);
    }
}
