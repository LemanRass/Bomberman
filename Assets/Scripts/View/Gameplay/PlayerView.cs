using System.Linq;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private Player player;

    public void Init(Player player)
    {
        this.player = player;
        GameManager.instance.onPlayerMoved += OnPlayerMoved;
    }

    private void OnPlayerMoved(Player player)
    {
        if(this.player.id.Equals(player.id))
        {
            transform.localPosition = new Vector3(player.pos.x * -1, transform.localPosition.y, player.pos.y);
        }
    }

    private void Update()
    {
        if (!player.isLocal)
            return;

        if(Input.GetKey(KeyCode.UpArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(0, 1.0f));
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(1.0f, 0));
        }

        if(Input.GetKey(KeyCode.DownArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(0, -1.0f));
        }

        if(Input.GetKey(KeyCode.RightArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(-1.0f, 0));
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            var bombData = Database.instance.bombs.First();
            GameManager.instance.SpawnBomb(bombData, 3, player.pos.ToRound());
        }
    }
}
