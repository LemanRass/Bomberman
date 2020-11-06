using UnityEngine;
using UnityEngine.SceneManagement;

public class HexagonalGameManager : GameManager
{
    protected override void Start()
    {
        Init();
        onInitDone?.Invoke();
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        if (bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    #region Initialization

    protected override void Init()
    {
        InitGround();

        InitBlocks();

        InitPlayers();

        InitBricks();

        InitPowerUps();

        InitBombs();

        InitCamera();
    }

    protected override void InitGround()
    {

    }

    protected override void InitBlocks() { }

    protected override void InitBricks() { }

    protected override void InitPowerUps() { }

    protected override void InitPlayers() { }

    protected override void InitBombs() { }

    protected override void InitCamera() { }

    #endregion



    #region Actions

    public override void MovePlayer(int id, Vector2 dir) { }

    public override void DeathPlayer(Player player) { }

    public override void SpawnBomb(DBBomb dbBomb, Player owner, Vector2 pos) { }

    public override void RemoveBomb(Bomb bomb) { }

    public override void ExplodeBomb(Bomb bomb) { }

    #endregion



    #region Tools

    public override CellType GetCellType(Vector2 pos)
    {
        return CellType.Outside;
    }

    public override bool HandleDestruction(Vector2 pos)
    {
        return false;
    }

    #endregion
}
