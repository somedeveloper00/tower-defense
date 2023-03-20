using TowerDefense.Core.Enemies;

namespace TowerDefense.Core.Spawn {
    public class BasicManip : EnemyArgManip {
        public float speedMultip = 1;
        public float lifeMultip = 1;
        
        protected override void Apply(Enemy enemy) {
            enemy.startingLife *= lifeMultip;
            enemy.life = enemy.startingLife;
            enemy.speed *= speedMultip;
        }
    }
}