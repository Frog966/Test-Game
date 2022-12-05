using UnityEngine;

namespace Game {
    namespace Unit {
        public enum Faction {
            ALLY,
            ENEMY,
            NEUTRAL,
        }
    }
    namespace Map {
        public enum MapLocale {
            TEST = 0,
        }

        // The first value must always be BOSS
        public enum NodeType {
            BOSS = 0,
            ENEMY,
            MINI_BOSS,
            SHOP,
        }

        [System.Serializable]
        public struct EnemyDetails {
            public Enemy enemy;
            public Vector2 enemyPos;

            public EnemyDetails(Enemy _enemy, Vector2 _enemyPos) {
                enemy = _enemy;
                enemyPos = _enemyPos;
            }
        }
    }
}