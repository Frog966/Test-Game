using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Map;

// Contains all enemy prefabs
// In charge of recording enemies and their correct positions at start of combat
public class World_EnemyLibrary : MonoBehaviour {
    [SerializeField] private Enemy[] enemies; // Contains every single enemy prefab in the Resources/Enemies folder

    // A nested dictionary containing the enemies, minibosses and bosses of a locale
    [SerializeField] private Dictionary<MapLocale, Dictionary<NodeType, List<List<EnemyDetails>>>> enemyLibrary;

    // Returns an encounter containing a list of enemies and their positions if the map locale and node type is valid
    public List<EnemyDetails> ReturnRandomEncounter(MapLocale mapLocale, NodeType nodeType) {
        if (enemyLibrary.ContainsKey(mapLocale) && enemyLibrary[mapLocale].ContainsKey(nodeType)) {
            List<List<EnemyDetails>> encounterList = enemyLibrary[mapLocale][nodeType];

            return encounterList[UnityEngine.Random.Range(0, encounterList.Count - 1)];
        }
        else {
            return Enumerable.Empty<EnemyDetails>().ToList(); // Return an immutable empty list
        }
    }

    public Enemy ReturnEnemyById(string id) {
        return Array.Find(enemies, (enemyScript) => enemyScript.id == "TestOnly!");
    }

    void Awake() {
        enemies = Resources.FindObjectsOfTypeAll(typeof(Enemy)) as Enemy[];
        
        enemyLibrary = new Dictionary<MapLocale, Dictionary<NodeType, List<List<EnemyDetails>>>>() {
            // All enemies, minibosses and bosses of MapLocale.TEST
            {
                MapLocale.TEST,
                new Dictionary<NodeType, List<List<EnemyDetails>>>() {
                    // All enemies of MapLocale.TEST
                    {
                        NodeType.ENEMY,
                        new List<List<EnemyDetails>>() {
                            new List<EnemyDetails>() {
                                new EnemyDetails(ReturnEnemyById("TestOnly!"), new Vector2(0, 0)),
                            },
                            new List<EnemyDetails>() {
                                new EnemyDetails(ReturnEnemyById("TestOnly!"), new Vector2(0, 0)),
                                new EnemyDetails(ReturnEnemyById("TestOnly!"), new Vector2(0, 1))
                            },
                            new List<EnemyDetails>() {
                                new EnemyDetails(ReturnEnemyById("TestOnly!"), new Vector2(0, 0)),
                                new EnemyDetails(ReturnEnemyById("TestOnly!"), new Vector2(0, 1)),
                                new EnemyDetails(ReturnEnemyById("TestOnly!"), new Vector2(0, -1))
                            },
                        }
                    },
                    // All minibosses of MapLocale.TEST
                    {
                        NodeType.MINI_BOSS,
                        new List<List<EnemyDetails>>() {
                            
                        }
                    },
                    // All bosses of MapLocale.TEST
                    {
                        NodeType.BOSS,
                        new List<List<EnemyDetails>>() {
                            
                        }
                    }
                }
            }
        };
    }
}