using System.Collections.Generic;
using UnityEngine;

public class FieldView : MonoBehaviour
{
    public List<BlockView> blocks;
    public List<BrickView> bricks;
    public List<PlayerView> players;
    public List<BombView> bombs;

    private void Start()
    {
        GameManager.instance.onInitDone += onInit;
        GameManager.instance.onBombSpawned += onSpawnBomb;
        GameManager.instance.onBombRemoved += onRemoveBomb;
        GameManager.instance.onExplosion += onExplosion;
    }

    private void onInit()
    {
        InitBlocks();
        InitBricks();
        InitPlane();
        InitPlayers();

        bombs = new List<BombView>();

        GameManager.instance.onInitDone -= onInit;
    }

    private void InitPlane()
    {
        var prefab = Resources.Load<GameObject>(GameManager.instance.ground.data.prefab);
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

        foreach(var block in GameManager.instance.blocks)
        {
            var prefab = Resources.Load<GameObject>(block.data.prefab);
            var go = Instantiate<GameObject>(prefab, transform);
            float x = -(block.pos.x * go.transform.localScale.x);
            float z = block.pos.y * go.transform.localScale.z;
            go.transform.localPosition = new Vector3(x, 0, z);
            var view = go.GetComponent<BlockView>();
            blocks.Add(view);
        }
    }

    private void InitBricks()
    {
        bricks = new List<BrickView>();

        foreach(var brick in GameManager.instance.bricks)
        {
            var prefab = Resources.Load<GameObject>(brick.data.prefab);
            var go = Instantiate<GameObject>(prefab, transform);
            float x = -(brick.pos.x * go.transform.localScale.x);
            float z = brick.pos.y * go.transform.localScale.z;
            go.transform.localPosition = new Vector3(x, 0, z);
            var view = go.GetComponent<BrickView>();
            bricks.Add(view);
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

    private void onSpawnBomb(Bomb bomb)
    {
        var prefab = Resources.Load<GameObject>(bomb.data.prefab);
        var go = Instantiate<GameObject>(prefab, transform);
        go.transform.localPosition = new Vector3(-bomb.pos.x, 0, bomb.pos.y);
        var view = go.GetComponent<BombView>();
        view.Init(bomb);
        bombs.Add(view);
    }

    private void onRemoveBomb(Bomb bomb)
    {
        //Despawn bomb
        var b = bombs.Find(n => n.bomb.Equals(bomb));
        bombs.Remove(b);
        Destroy(b.gameObject);
    }

    private void onExplosion(ExplosionType type, Vector2 pos)
    {
        //Spawn explosion
        var explosionDB = Database.GetExplosion(type);
        var prefab = Resources.Load<GameObject>(explosionDB.prefab);
        var go = Instantiate<GameObject>(prefab, transform);
        go.transform.localPosition = new Vector3(-pos.x, 0, pos.y);
        var explosionView = go.GetComponent<ExplosionView>();
        explosionView.Explode(1);
    }
}
