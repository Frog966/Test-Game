using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Player_Effects : MonoBehaviour {
    [SerializeField] private Transform effectsPool;

    [Header("Prefabs")]
    [SerializeField] private GameObject beam;

    [SerializeField] private List<Transform> list_Beams = new List<Transform>();
    
    private static Player_Effects Inst;

    // Beam Methods
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static void PlaceEffect_Beam(int x, int y) { 
        World_GridNode targetNode = World_Grid.GetNode(x, y);
        
        Transform currBeam;
        List<Transform> list_UnusuedBeams = Inst.list_Beams.Where((b) => b.parent == Inst.effectsPool).ToList();

        if (list_UnusuedBeams.Count > 0) { currBeam = list_UnusuedBeams[0]; } // If there is an unused beam effect
        else { currBeam = Inst.AddEffect_Beam(); } // Add a new beam if there isn't

        currBeam.position = targetNode.gameObject.transform.position;

        currBeam.SetParent(targetNode.transform);
        currBeam.GetComponent<Canvas>().sortingOrder = y + 1; // Update entity's canvas sort order
    }

    public static void PlaceEffect_Beam(Vector2Int vec2) {         
        World_GridNode targetNode = World_Grid.GetNode(vec2);
        
        Transform currBeam;
        List<Transform> list_UnusuedBeams = Inst.list_Beams.Where((b) => b.parent == Inst.effectsPool).ToList();

        if (list_UnusuedBeams.Count > 0) { currBeam = list_UnusuedBeams[0]; } // If there is an unused beam effect
        else { currBeam = Inst.AddEffect_Beam(); } // Add a new beam if there isn't

        currBeam.position = targetNode.gameObject.transform.position;
        
        currBeam.SetParent(targetNode.transform);
        currBeam.GetComponent<Canvas>().sortingOrder = vec2.y + 1; // Update entity's canvas sort order
    }

    public static void PlaceEffect_Beam(List<Vector2Int> posList) {
        foreach (Vector2Int pos in posList) { PlaceEffect_Beam(pos); }
    }

    public static void ReturnEffect_Beam() {
        foreach (Transform b in Inst.list_Beams) { b.SetParent(Inst.effectsPool); }
    }
    
    private Transform AddEffect_Beam() {
        Transform newBeam = GameObject.Instantiate(beam, effectsPool).transform; 
        list_Beams.Add(newBeam);

        return newBeam;
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Awake() {
        // Instance declaration
        if (Inst != null && Inst != this) { Destroy(this); }
        else { Inst = this; }

        effectsPool.gameObject.SetActive(false);

        for (int i = 0; i < 5; i ++) { 
            AddEffect_Beam(); 
        }
    }
}