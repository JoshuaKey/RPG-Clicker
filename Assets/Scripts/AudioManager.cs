using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] AudioClip musicClip;

    private AudioSource music;
    private List<AudioSource> sounds = new List<AudioSource>();

    [HideInInspector] public static AudioManager Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        music = this.gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        music.loop = true;
        if(musicClip != null) {
            PlayMusic(musicClip);
        }

        var sound = this.gameObject.AddComponent<AudioSource>();
        sound.playOnAwake = false;
        sounds.Add(sound);
    }

    public void PlayMusic(AudioClip m) {
        music.clip = m;
        music.loop = true;
        music.Play();
    }

    public void PlaySound(AudioClip s) {
        var sound = GetUnplayedSound();
        sound.clip = s;
        sound.Play();
    }

    public AudioSource GetUnplayedSound() {
        foreach(var sound in sounds) {
            if (!sound.isPlaying) {
                return sound;
            }
        }

        var s = this.gameObject.AddComponent<AudioSource>();
        sounds.Add(s);
        return s;
    }


}
