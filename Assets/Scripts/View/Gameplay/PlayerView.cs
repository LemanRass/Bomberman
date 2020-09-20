using System.Linq;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [HideInInspector]
    public Player player;

    public void Init(Player player)
    {
        this.player = player;
        transform.localPosition = new Vector3(-player.pos.x, 0, player.pos.y);
    }

    public void OnPlayerMoved()
    {
        transform.localPosition = new Vector3(player.pos.x * -1, transform.localPosition.y, player.pos.y);
    }

    public void OnPlayerDeath()
    {
        //Some death things
        //animation, sound, etc
        Destroy(gameObject);
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
