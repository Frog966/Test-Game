using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Map;

// Contains all enemy prefabs
// In charge of recording enemies and their correct positions at start of combat
public class World_EnemyLibrary : MonoBehaviour {
    // Enemy prefab lists from Resources folders
    [SerializeField] private Enemy[] enemies; // Contains every single enemy prefab in the Resources/Enemies folder
    [SerializeField] private Enemy[] minibosses; // Contains every single enemy prefab in the Resources/MiniBosses folder
    [SerializeField] private Enemy[] bosses; // Contains every single enemy prefab in the Resources/Bosses folder

    // A nested dictionary containing the enemies, minibosses and bosses of a locale
    [SerializeField] private Dictionary<MapLocale, Dictionary<NodeType, List<List<EncounterEnemyDetails>>>> enemyLibrary;

    // Returns an encounter containing a list of enemies and their positions if the map locale and node type is valid
    public List<EncounterEnemyDetails> ReturnRandomEncounter(MapLocale mapLocale, NodeType nodeType) {
        if (enemyLibrary.ContainsKey(mapLocale) && enemyLibrary[mapLocale].ContainsKey(nodeType)) {
            List<List<EncounterEnemyDetails>> encounterList = enemyLibrary[mapLocale][nodeType];

            return encounterList[UnityEngine.Random.Range(0, encounterList.Count > 0 ? encounterList.Count - 1 : 0)];
        }
        else {
            return Enumerable.Empty<EncounterEnemyDetails>().ToList(); // Return an immutable empty list
        }
    }

    public Enemy ReturnEnemyById(string id) {
        Enemy enemy = Array.Find(enemies, (enemyScript) => enemyScript.GetComponent<Enemy>().id == id);

        if (!enemy) { Debug.LogWarning("ReturnEnemyById ID not found: '" + id + "'"); }

        return enemy;
    }

    void Awake() {
        enemies = Resources.LoadAll("Enemies", typeof(Enemy)).Cast<Enemy>().ToArray();
        minibosses = Resources.LoadAll("MiniBosses", typeof(Enemy)).Cast<Enemy>().ToArray();
        bosses = Resources.LoadAll("Bosses", typeof(Enemy)).Cast<Enemy>().ToArray();
        
        abc //! TODO: Consider using an enum in Enemy script to determine if enemy is enemy/miniboss/boss to avoid using 3 lists + search functions

        // A huge declaration of enemyLibrary
        enemyLibrary = new Dictionary<MapLocale, Dictionary<NodeType, List<List<EncounterEnemyDetails>>>>() {
            // All enemies, minibosses and bosses of MapLocale.TEST
            {
                MapLocale.TEST,
                new Dictionary<NodeType, List<List<EncounterEnemyDetails>>>() {
                    // All enemies of MapLocale.TEST
                    {
                        NodeType.ENEMY,
                        new List<List<EncounterEnemyDetails>>() {
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Test Enemy"), new Vector2Int(3, 0)),
                            },
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Test Enemy"), new Vector2Int(3, 0)),
                                new EncounterEnemyDetails(ReturnEnemyById("Test Enemy"), new Vector2Int(3, 1))
                            },
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Test Enemy"), new Vector2Int(3, 0)),
                                new EncounterEnemyDetails(ReturnEnemyById("Test Enemy"), new Vector2Int(3, 1)),
                                new EncounterEnemyDetails(ReturnEnemyById("Test Enemy"), new Vector2Int(3, -1))
                            },
                        }
                    },
                    // All minibosses of MapLocale.TEST
                    {
                        NodeType.MINI_BOSS,
                        new List<List<EncounterEnemyDetails>>() {
                            new List<EncounterEnemyDetails>() {}
                        }
                    },
                    // All bosses of MapLocale.TEST
                    {
                        NodeType.BOSS,
                        new List<List<EncounterEnemyDetails>>() {
                            new List<EncounterEnemyDetails>() {}
                        }
                    }
                }
            }
        };
    }
}