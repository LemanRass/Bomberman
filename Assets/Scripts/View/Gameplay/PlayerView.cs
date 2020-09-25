using System.Linq;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [HideInInspector]
    public Player player;

    public void Init(Player player)
    {
        this.player = player;
        transform.localPosition = new Vector3(-player.pos.x, 0.5f, player.pos.y);
    }

    private void Update()
    {
        if (player.isAI)
        {
            AIController();
        }
        else
        {
            PlayerController();
        }


        //Test
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.id == 0)
            {
                player.PickUpPowerUp(PowerUPType.MOVE_SPEED);
                Debug.Log($"Pickd up speed powerUP ({player.powerUps.data[PowerUPType.MOVE_SPEED]}).");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (player.id == 0)
            {
                player.PickUpPowerUp(PowerUPType.EXTRA_BOMB);
                Debug.Log($"Pickd up extra bomb powerUP ({player.powerUps.data[PowerUPType.EXTRA_BOMB]}).");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (player.id == 0)
            {
                player.PickUpPowerUp(PowerUPType.EXPLOSION_SIZE);
                Debug.Log($"Pickd up explosion size powerUP ({player.powerUps.data[PowerUPType.EXPLOSION_SIZE]}).");
            }
        }
    }

    private void PlayerController()
    {
        if (Input.GetKey(Settings.instance.playersKeyMap[player.id].MOVE_UP))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(0, player.moveSpeed));
        }

        if (Input.GetKey(Settings.instance.playersKeyMap[player.id].MOVE_LEFT))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(player.moveSpeed, 0));
        }

        if (Input.GetKey(Settings.instance.playersKeyMap[player.id].MOVE_DOWN))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(0, -player.moveSpeed));
        }

        if (Input.GetKey(Settings.instance.playersKeyMap[player.id].MOVE_RIGHT))
        {
            GameManager.instance.MovePlayer(player.id, new Vector2(-player.moveSpeed, 0));
        }

        if (Input.GetKeyDown(Settings.instance.playersKeyMap[player.id].PLANT_BOMB))
        {
            var bombData = Database.instance.bombs.First();
            GameManager.instance.SpawnBomb(bombData, player, player.pos.ToRound());
        }
    }

    private void AIController()
    {

    }


    #region Callbacks

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

    #endregion
}
