using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour {

    [System.Serializable]
    public struct World {
        public int beginRound;
        public AudioClip music;
        public Sprite background;
    }


    [SerializeField] World[] worlds;
    [SerializeField] int worldLoop = 100;
    private World currWorld;
    private SpriteRenderer background;

	// Use this for initialization
	void Start () {
        background = GetComponent<SpriteRenderer>();

        Game.Instance.OnEnemyDefeat += CheckRound;

        Invoke("CheckRound", .5f); // Initial Check...
	}

    public void CheckRound() {
        World prevWorld = worlds[0];
        int round = Game.Instance.GetRound() % worldLoop;

        for (int i = 1; i < worlds.Length; i++) {
            World currWorld = worlds[i];
            if(currWorld.beginRound > round) { // In Previous World
                break;
            }
            prevWorld = currWorld;
        }

        if(prevWorld.beginRound != currWorld.beginRound) { // Change World
            currWorld = prevWorld;
            AudioManager.Instance.PlayMusic(currWorld.music);
            background.sprite = currWorld.background;
        }
    }
}
