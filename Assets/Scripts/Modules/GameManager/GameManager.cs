using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake() => instance = this;

    [Range(0, 100)]
    public int BRICKS_DENSITY = 90;
    [Range(0, 100)]
    public int POWERUP_DENSITY = 50;

    [HideInInspector]
    public Ground[][] grounds;
    [HideInInspector]
    public List<Block> blocks = new List<Block>();
    [HideInInspector]
    public List<Brick> bricks = new List<Brick>();
    [HideInInspector]
    public List<PowerUP> powerUPs = new List<PowerUP>();
    [HideInInspector]
    public List<Player> players = new List<Player>();
    [HideInInspector]
    public List<Bomb> bombs = new List<Bomb>();

    public Action onInitDone = null;
    public Action<Player> onMovePlayer = null;
    public Action<Player> onDeathPlayer = null;
    public Action<Bomb> onBombSpawned = null;
    public Action<Bomb> onBombRemoved = null;
    public Action<Brick> onBrickDestroyed = null;
    public Action<PowerUP> onPowerUPPicked = null;
    public Action<PowerUP> onPowerUPDestroyed = null;
    public Action<ExplosionType, Vector2> onExplosion = null;

    protected abstract void Update();

    #region Initialization

    public abstract void Init();

    protected abstract void InitGround();

    protected abstract void InitBlocks();

    protected abstract void InitBricks();

    protected abstract void InitPowerUps();

    protected abstract void InitPlayers();

    protected abstract void InitBombs();

    protected abstract void InitCamera();

    #endregion



    #region Actions

    public abstract void MovePlayer(int id, Vector2 dir);

    public abstract void DeathPlayer(Player player);

    public abstract void SpawnBomb(DBBomb dbBomb, Player owner, Vector2 pos);

    public abstract void RemoveBomb(Bomb bomb);

    public abstract void ExplodeBomb(Bomb bomb);

    #endregion



    #region Tools

    public abstract CellType GetCellType(Vector2Int coords);

    public abstract bool HandleDestruction(Ground ground);

    #endregion
}
