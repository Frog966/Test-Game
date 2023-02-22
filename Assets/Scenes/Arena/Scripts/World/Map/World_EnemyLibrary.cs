using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Game.Map;

// Contains all enemy prefabs
// In charge of recording enemies and their correct positions at start of combat
public class World_EnemyLibrary : MonoBehaviour {
    // Enemy prefab lists from Resources folders
    [SerializeField] private IEnemy[] enemies; // Contains every single enemy prefab in the Resources/Enemies folder
    [SerializeField] private IEnemy[] minibosses; // Contains every single enemy prefab in the Resources/MiniBosses folder
    [SerializeField] private IEnemy[] bosses; // Contains every single enemy prefab in the Resources/Bosses folder

    // A nested dictionary containing the enemies, minibosses and bosses of a locale
    [SerializeField] private Dictionary<MapLocale, Dictionary<NodeType, List<List<EncounterEnemyDetails>>>> encounterDict;

    // Returns an encounter containing a list of enemies and their positions if the map locale and node type is valid
    public List<EncounterEnemyDetails> ReturnRandomEncounter(MapLocale mapLocale, NodeType nodeType) {
        if (encounterDict.ContainsKey(mapLocale) && encounterDict[mapLocale].ContainsKey(nodeType)) {
            List<List<EncounterEnemyDetails>> encounterList = encounterDict[mapLocale][nodeType];

            return encounterList[UnityEngine.Random.Range(0, encounterList.Count)];
        }
        else {
            return Enumerable.Empty<EncounterEnemyDetails>().ToList(); // Return an immutable empty list
        }
    }

    public IEnemy ReturnEnemyById(string id) {
        IEnemy enemy = Array.Find(enemies, (enemyScript) => enemyScript.ID == id);

        if (enemy == null) { Debug.LogWarning("ReturnEnemyById ID not found: '" + id + "'"); }

        return enemy;
    }

    public IEnemy ReturnMiniBossById(string id) {
        IEnemy enemy = Array.Find(minibosses, (enemyScript) => enemyScript.ID == id);

        if (enemy == null) { Debug.LogWarning("ReturnMiniBossById ID not found: '" + id + "'"); }

        return enemy;
    }

    public IEnemy ReturnBossById(string id) {
        IEnemy enemy = Array.Find(bosses, (enemyScript) => enemyScript.ID == id);

        if (enemy == null) { Debug.LogWarning("ReturnBossById ID not found: '" + id + "'"); }

        return enemy;
    }

    void Awake() {
        enemies = Resources.LoadAll("Enemies", typeof(IEnemy)).Cast<IEnemy>().ToArray();
        // minibosses = Resources.LoadAll("MiniBosses", typeof(IEnemy)).Cast<IEnemy>().ToArray();
        bosses = Resources.LoadAll("Bosses", typeof(IEnemy)).Cast<IEnemy>().ToArray();

        // A huge declaration of encounterDict
        encounterDict = new Dictionary<MapLocale, Dictionary<NodeType, List<List<EncounterEnemyDetails>>>>() {
            // All enemies, minibosses and bosses of MapLocale.TEST
            {
                MapLocale.TEST,
                new Dictionary<NodeType, List<List<EncounterEnemyDetails>>>() {
                    // All enemies of MapLocale.TEST
                    {
                        NodeType.ENEMY,
                        new List<List<EncounterEnemyDetails>>() {
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Powie 1"), new Vector2Int(4, 1)),
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(5, 1)),
                            },
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(5, 0)),
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(5, 1)),
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(5, 2)),
                            },
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Powie 1"), new Vector2Int(4, 1)),
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(5, 0)),
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(5, 2)),
                            },
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnEnemyById("Canguard 1"), new Vector2Int(4, 1)),
                                new EncounterEnemyDetails(ReturnEnemyById("Powie 1"), new Vector2Int(5, 1)),
                            }
                        }
                    },
                    // All minibosses of MapLocale.TEST
                    // {
                    //     NodeType.MINI_BOSS,
                    //     new List<List<EncounterEnemyDetails>>() {
                    //         new List<EncounterEnemyDetails>() {
                    //             new EncounterEnemyDetails(ReturnMiniBossById("Test Miniboss"), new Vector2Int(4, 1)),
                    //         }
                    //     }
                    // },
                    // All bosses of MapLocale.TEST
                    {
                        NodeType.BOSS,
                        new List<List<EncounterEnemyDetails>>() {
                            new List<EncounterEnemyDetails>() {
                                new EncounterEnemyDetails(ReturnBossById("Test Boss"), new Vector2Int(4, 1)),
                            }
                        }
                    }
                }
            }
        };
    }
}