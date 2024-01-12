namespace FlashSexJam.Enemy.Impl
{
    public class TentacleEnemy : EnemyController
    {
        public override (float Min, float Max) SpawnRange => (-3.5f, 3.5f);
    }
}
