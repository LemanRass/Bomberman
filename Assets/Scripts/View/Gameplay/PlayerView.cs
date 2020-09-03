using System.Collections;
using System.Collections.Generic;
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
            transform.localPosition = new Vector3(player.pos.x * -1, player.pos.y, player.pos.z);
        }
    }

    private void Update()
    {
        if (!player.isLocal)
            return;

        if(Input.GetKey(KeyCode.UpArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector3(0, 0, 1.0f));
            //transform.localPosition += new Vector3(0, 0, speed ) * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector3(1.0f, 0, 0));
            //transform.localPosition += new Vector3(-speed, 0, 0) * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.DownArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector3(0, 0, -1.0f));
            //transform.localPosition += new Vector3(0, 0, -speed) * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.RightArrow))
        {
            GameManager.instance.MovePlayer(player.id, new Vector3(-1.0f, 0, 0));
            //transform.localPosition += new Vector3(speed, 0, 0) * Time.deltaTime;
        }
    }
}
