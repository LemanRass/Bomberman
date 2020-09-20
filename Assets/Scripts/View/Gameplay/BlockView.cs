using UnityEngine;

public class BlockView : MonoBehaviour
{
    [HideInInspector]
    public Block block;

    public void Init(Block block)
    {
        this.block = block;

        float x = -(block.pos.x * transform.localScale.x);
        float z = block.pos.y * transform.localScale.z;
        transform.localPosition = new Vector3(x, 0, z);
    }
}
