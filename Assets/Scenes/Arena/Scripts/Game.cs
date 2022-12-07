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
        public struct EncounterEnemyDetails {
            public Enemy enemy;
            public Vector2Int gridPos;

            public EncounterEnemyDetails(Enemy _enemy, Vector2Int _gridPos) {
                enemy = _enemy;
                gridPos = _gridPos;
            }
        }
    }
}