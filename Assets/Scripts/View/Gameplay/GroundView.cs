using UnityEngine;

public class GroundView : MonoBehaviour
{
    public Ground ground;

    public void Init(Ground ground)
    {
        this.ground = ground;
        transform.localPosition = new Vector3(ground.pos.x, 0, ground.pos.y);
    }
}
