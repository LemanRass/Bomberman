using UnityEngine;

public static class Constants
{
    public static int FIELD_SIZE_X = 11;
    public static int FIELD_SIZE_Y = 11;

    public static float FIELD_CELL_SIZE = 1.0f;
    public static float EXPLOSION_AFFECT_DIST = 0.8f;
    public static float MOVE_COLLISION_DIST = 0.98f;
    public static float MOVE_ON_BOMB_THRESHOLD = 0.979f;

    public static Vector2 botRightSpawnPoint = new Vector2(1, 1);
    public static Vector2 topRightSpawnPoint = new Vector2(1, FIELD_SIZE_Y - 2);
    public static Vector2 botLeftSpawnPoint = new Vector2(FIELD_SIZE_X - 2, 1);
    public static Vector2 topLeftSpawnPoint = new Vector2(FIELD_SIZE_X - 2, FIELD_SIZE_Y - 2);

}
