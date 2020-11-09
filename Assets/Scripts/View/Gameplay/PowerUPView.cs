using UnityEngine;

public class PowerUPView : MonoBehaviour
{
    public PowerUP powerUP;

    public void Init(PowerUP powerUP)
    {
        this.powerUP = powerUP;
        transform.localPosition = new Vector3(powerUP.pos.x, 0.05f, powerUP.pos.y);
        gameObject.SetActive(false);
    }
}
