using UnityEngine;

public class BlockView : MonoBehaviour
{
    public Block block;

    public void Init(Block block)
    {
        this.block = block;
        transform.localPosition = new Vector3(block.pos.x, 0, block.pos.y);
    }
}
