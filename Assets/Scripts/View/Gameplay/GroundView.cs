using UnityEngine;

public class GroundView : MonoBehaviour
{
    public Ground ground;

    public void Init(Ground ground)
    {
        this.ground = ground;

        float x = -(ground.pos.x * transform.localScale.x);
        float z = ground.pos.y * transform.localScale.z;
        transform.localPosition = new Vector3(x, 0, z);
    }
}
