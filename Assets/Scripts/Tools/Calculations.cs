public static class Calculations
{
    public static float CalcMoveSpeed(Player player)
    {
        float defaultSpeed = 1.0f;
        float koefSpeed = 1.1f;

        return defaultSpeed + player.powerUps.Count(PowerUPType.MOVE_SPEED) * koefSpeed;
    }

    public static int CalcBombsCountLimit(Player player)
    {
        int defaultCount = 1;

        return defaultCount + player.powerUps.Count(PowerUPType.EXTRA_BOMB);
    }

    public static int CalcBombExplosionPower(Player player)
    {
        int defaultPower = 2;

        return defaultPower + player.powerUps.Count(PowerUPType.EXPLOSION_SIZE);
    }
}
