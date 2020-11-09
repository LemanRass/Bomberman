using UnityEngine;

public class BrickView : MonoBehaviour
{
    public Brick brick;

    public void Init(Brick brick)
    {
        this.brick = brick;
        transform.localPosition = new Vector3(brick.pos.x, 0, brick.pos.y);
    }
}
