public static class Calculations
{
    public static float CalcMoveSpeed(Player player)
    {
        float defaultSpeed = 1.5f;
        float koefSpeed = 0.1f;

        return defaultSpeed + player.powerUps.Count(PowerUPType.MOVE_SPEED) * koefSpeed;
    }

    public static int CalcBombsCountLimit(Player player)
    {
        int defaultCount = 1;

        return defaultCount + player.powerUps.Count(PowerUPType.EXTRA_BOMB);
    }

    public static int CalcBombExplosionPower(Player player)
    {
        int defaultPower = 1;

        return defaultPower + player.powerUps.Count(PowerUPType.EXPLOSION_SIZE);
    }
}
