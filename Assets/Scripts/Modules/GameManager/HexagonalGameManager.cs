using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonalGameManager : GameManager
{
    public const int maxSize = 7;
    public const int minSize = 4;

    public const float verticalOffset = 0.78f;
    public const float horizontalOffset = 1.0f;


    protected override void Update()
    {
        if (bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    #region Initialization

    public override void Init()
    {
        InitGround();

        InitBricks();

        /*InitBlocks();

        InitPlayers();

        

        InitPowerUps();

        InitBombs();

        InitCamera();*/

        onInitDone?.Invoke();
    }

    protected override void InitGround()
    {
        grounds = new List<Ground>();
        var groundData = Database.instance.grounds.Last();

        int rowsCount = (maxSize - minSize) * 2 + 1;

        int count = minSize;
        
        float oy = verticalOffset * (rowsCount / 2);

        for(int i = 0; i < rowsCount; i++)
        {
            float ox = (count - 1) * horizontalOffset / 2;

            for (int j = 0; j < count; j++)
            {
                grounds.Add(new Ground(groundData, new Vector2Int(j, i), new Vector2(-j + ox, oy)));
            }

            if (i >= rowsCount / 2)
            {
                count--;
                ox -= horizontalOffset / 2;

            }
            else
            {
                count++;
                ox += horizontalOffset / 2;

            }

            oy -= verticalOffset;
            //return;
        }
    }

    protected override void InitBlocks()
    {
        blocks = new List<Block>();

        var blockData = Database.instance.blocks.Last();
        
    }

    protected override void InitBricks()
    {
        var brickData = Database.instance.bricks.Last();


        int count = grounds.Count();
        int bricksCount = Mathf.RoundToInt(count * (BRICKS_DENSITY / 100.0f));


        while (bricksCount > 0)
        {
            var ground = grounds.Random();

            var cell = GetCellType(ground.coords);
            if (cell != CellType.Empty)
                continue;

            //if (players.Any(n => Vector2.Distance(n.pos, pos) < Constants.FIELD_CELL_SIZE * 2))
            //    continue;

            bricks.Add(new Brick(brickData, ground.coords, ground.pos));
            bricksCount--;
        }
        //bricks.Add(new Brick(brickData, grounds.Random().pos));
    }

    protected override void InitPowerUps() { }

    protected override void InitPlayers() { }

    protected override void InitBombs() { }

    protected override void InitCamera() { }

    #endregion



    #region Actions

    public override void MovePlayer(int id, Vector2 dir) { }

    public override void DeathPlayer(Player player) { }

    public override void SpawnBomb(DBBomb dbBomb, Player owner, Vector2Int coords) { }

    public override void RemoveBomb(Bomb bomb) { }

    public override void ExplodeBomb(Bomb bomb) { }

    #endregion



    #region Tools

    public override CellType GetCellType(Vector2Int coords)
    {
        if (bricks.Any(n => n.coords.Equals(coords)))
            return CellType.Brick;

        return CellType.Empty;
    }

    public override bool HandleDestruction(Vector2Int coords)
    {
        return false;
    }

    #endregion
}
