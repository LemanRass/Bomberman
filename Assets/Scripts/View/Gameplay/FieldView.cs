using System.Collections.Generic;
using UnityEngine;

public class FieldView : MonoBehaviour
{
    [HideInInspector]
    public List<GroundView> grounds;
    [HideInInspector]
    public List<BlockView> blocks;
    [HideInInspector]
    public List<BrickView> bricks;
    [HideInInspector]
    public List<PowerUPView> powerUPs;
    [HideInInspector]
    public List<PlayerView> players;
    [HideInInspector]
    public List<BombView> bombs;

    private void Start()
    {
        GameManager.instance.onInitDone += onInit;
        GameManager.instance.onBombSpawned += onSpawnBomb;
        GameManager.instance.onBombRemoved += onRemoveBomb;
        GameManager.instance.onExplosion += onExplosion;
        GameManager.instance.onBrickDestroyed += onBrickDestroyed;
        GameManager.instance.onMovePlayer += onMovePlayer;
        GameManager.instance.onDeathPlayer += onDeathPlayer;
        GameManager.instance.onPowerUPPicked += onPowerUPPicked;
        GameManager.instance.onPowerUPDestroyed += onPowerUPDestroyed;
    }

    private void onInit()
    {
        InitBlocks();
        InitBricks();
        InitGround();
        InitPlayers();
        InitPowerUPs();

        bombs = new List<BombView>();

        GameManager.instance.onInitDone -= onInit;
    }

    private void InitGround()
    {
        grounds = new List<GroundView>();

        foreach (var ground in GameManager.instance.grounds)
        {
            var prefab = Resources.Load<GameObject>(ground.data.prefab);
            var go = Instantiate<GameObject>(prefab, transform);
            var view = go.GetComponent<GroundView>();
            view.Init(ground);
            grounds.Add(view);
        }
    }

    private void InitBlocks()
    {
        blocks = new List<BlockView>();

        foreach(var block in GameManager.instance.blocks)
        {
            var prefab = Resources.Load<GameObject>(block.data.prefab);
            var go = Instantiate<GameObject>(prefab, transform);
            var view = go.GetComponent<BlockView>();
            view.Init(block);
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
            var view = go.GetComponent<BrickView>();
            view.Init(brick);
            bricks.Add(view);
        }
    }

    private void InitPowerUPs()
    {
        powerUPs = new List<PowerUPView>();

        foreach(var powerUP in GameManager.instance.powerUPs)
        {
            var prefab = Resources.Load<GameObject>(powerUP.data.prefab);
            var go = Instantiate<GameObject>(prefab, transform);
            var view = go.GetComponent<PowerUPView>();
            view.Init(powerUP);
            powerUPs.Add(view);
        }
    }

    private void InitPlayers()
    {
        players = new List<PlayerView>();

        foreach(var player in GameManager.instance.players)
        {
            var prefab = Resources.Load<GameObject>(player.data.prefab);
            var go = Instantiate<GameObject>(prefab, transform);
            var view = go.GetComponent<PlayerView>();
            view.Init(player);
            players.Add(view);
        }
    }

    private void onSpawnBomb(Bomb bomb)
    {
        var prefab = Resources.Load<GameObject>(bomb.data.prefab);
        var go = Instantiate<GameObject>(prefab, transform);
        var view = go.GetComponent<BombView>();
        view.Init(bomb);
        bombs.Add(view);
    }

    private void onRemoveBomb(Bomb bomb)
    {
        var item = bombs.Find(n => n.bomb.Equals(bomb));
        bombs.Remove(item);
        Destroy(item.gameObject);
    }

    private void onMovePlayer(Player player)
    {
        var item = players.Find(n => n.player.Equals(player));
        item.OnPlayerMoved();
    }

    private void onDeathPlayer(Player player)
    {
        var item = players.Find(n => n.player.Equals(player));
        players.Remove(item);
        item.OnPlayerDeath();
    }

    private void onExplosion(ExplosionType type, Vector2 pos)
    {
        //Spawn explosion shell
        var explosionDB = Database.GetExplosion(type);
        var prefab = Resources.Load<GameObject>(explosionDB.prefab);
        var go = Instantiate<GameObject>(prefab, transform);
        var explosionView = go.GetComponent<ExplosionView>();
        explosionView.Explode(pos);
    }

    private void onBrickDestroyed(Brick brick)
    {
        var item = bricks.Find(n => n.brick.Equals(brick));
        bricks.Remove(item);
        Destroy(item.gameObject);
    }

    private void onPowerUPPicked(PowerUP powerUP)
    {
        var item = powerUPs.Find(n => n.powerUP.Equals(powerUP));
        powerUPs.Remove(item);
        Destroy(item.gameObject);
    }

    private void onPowerUPDestroyed(PowerUP powerUP)
    {
        var item = powerUPs.Find(n => n.powerUP.Equals(powerUP));
        powerUPs.Remove(item);
        Destroy(item.gameObject);
    }
}
