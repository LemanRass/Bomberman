using System.Collections.Generic;
using UnityEngine;

public class FieldView : MonoBehaviour
{
    public List<BlockView> blocks;
    public List<BrickView> bricks;
    public List<PlayerView> players;

    private void Start()
    {
        GameManager.instance.onInitDone += Init;
    }

    private void Init()
    {
        InitBlocks();
        InitBricks();
        InitPlane();
        InitPlayers();

        GameManager.instance.onInitDone -= Init;
    }

    private void InitPlane()
    {
        var prefab = Resources.Load<GameObject>($"Planes/1/Plane_1");
        var go = Instantiate<GameObject>(prefab, transform);

        float scaleX = GameManager.instance.FIELD_SIZE.x;
        float scaleZ = GameManager.instance.FIELD_SIZE.y;
        go.transform.localScale = new Vector3(scaleX, 0.1f, scaleZ);

        float posX = (scaleX - 1.0f) / 2 * -1.0f;
        float posZ = (scaleZ - 1.0f) / 2;
        go.transform.localPosition = new Vector3(posX, 0, posZ);
    }

    private void InitBlocks()
    {
        blocks = new List<BlockView>();

        foreach(var blockData in GameManager.instance.blocks)
        {
            var prefab = Resources.Load<GameObject>($"Blocks/1/Block_1");
            var go = Instantiate<GameObject>(prefab, transform);
            float x = -(blockData.pos.x * go.transform.localScale.x);
            float z = blockData.pos.y * go.transform.localScale.z;
            go.transform.localPosition = new Vector3(x, 0, z);
            var block = go.GetComponent<BlockView>();
            blocks.Add(block);
        }
    }

    private void InitBricks()
    {
        bricks = new List<BrickView>();

        foreach(var brickData in GameManager.instance.bricks)
        {
            var prefab = Resources.Load<GameObject>("Bricks/1/Brick_1");
            var go = Instantiate<GameObject>(prefab, transform);
            float x = -(brickData.pos.x * go.transform.localScale.x);
            float z = brickData.pos.y * go.transform.localScale.z;
            go.transform.localPosition = new Vector3(x, 0, z);
            var brick = go.GetComponent<BrickView>();
            bricks.Add(brick);
        }
    }

    private void InitPlayers()
    {
        players = new List<PlayerView>();

        foreach(var playerData in GameManager.instance.players)
        {
            int idx = playerData.isLocal ? 1 : 2;
            var prefab = Resources.Load<GameObject>($"Players/{idx}/Player_{idx}");
            var go = Instantiate<GameObject>(prefab, transform);
            go.transform.localPosition = new Vector3(-playerData.pos.x, 0, playerData.pos.y);
            var player = go.GetComponent<PlayerView>();
            player.Init(playerData);
            players.Add(player);
        }
    }
}
