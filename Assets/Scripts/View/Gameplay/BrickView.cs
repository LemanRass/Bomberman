using UnityEngine;

public class BrickView : MonoBehaviour
{
    public Brick brick;

    public void Init(Brick brick)
    {
        this.brick = brick;
        float x = -(brick.pos.x * transform.localScale.x);
        float z = brick.pos.y * transform.localScale.z;
        transform.localPosition = new Vector3(x, 0, z);
    }
}
