using UnityEngine;

public class AudioHandler : MonoBehaviour {
    [SerializeField] private AudioSource as_BG, as_SE;

    [Header("BGMs")]
    [SerializeField] private AudioClip bgm_Map;
    [SerializeField] private AudioClip bgm_Shop, bgm_Enemy, bgm_Boss;

    private static AudioHandler inst;

    public static void PlayBGM(AudioClip bgMusic) { 
        inst.as_BG.clip = bgMusic;
        inst.as_BG.Play(); 

        Debug.Log("PlayBGM: " + bgMusic);
    }

    public static void PlayClip(AudioClip seClip) { 
        inst.as_SE.clip = seClip;
        inst.as_SE.Play(); 
    }

    public static void PlayBGM_Map() { PlayBGM(inst.bgm_Map); }
    public static void PlayBGM_Shop() { PlayBGM(inst.bgm_Shop); }
    public static void PlayBGM_Enemy() { PlayBGM(inst.bgm_Enemy); }
    public static void PlayBGM_Boss() { PlayBGM(inst.bgm_Boss); }

    void Awake() {
        // Instance declaration
        if (inst != null && inst != this) { Destroy(this); }
        else { inst = this; }

        as_SE.loop = false;

        as_BG.clip = null;
        as_BG.loop = true;
        as_BG.playOnAwake = false;
    }
}
