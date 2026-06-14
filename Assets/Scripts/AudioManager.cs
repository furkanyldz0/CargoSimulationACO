using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource soundMusicObject; //kameradaki AudioSource'a eriþeceðiz

    [SerializeField] private AudioClip buttonClickSoundClip;
    [SerializeField] private AudioClip buttonBackClickSoundClip;
    [SerializeField] private AudioClip initiateSimulationSoundClip;
    [SerializeField] private AudioClip ambienceSoundClip;
    [SerializeField] private AudioClip citySelectSoundClip;

    private float lastSoundTime;
    private float soundCooldown = 0.1f;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla AudioManager var!");
        }
        Instance = this;
    }

    private void Start() {
        var cameraPosition = Camera.main.transform.position;
        soundFXObject.transform.position = cameraPosition;
        soundMusicObject.transform.position = cameraPosition;

        PlayAmbientMusic();
    }


    private void PlayAmbientMusic() {
        soundMusicObject.loop = true;

        soundMusicObject.clip = ambienceSoundClip;
        soundMusicObject.Play();
    }

    private void PlaySoundClip(AudioClip audioClip, float volume) {
        //AudioSource audioSource = Instantiate(soundFXObject, Camera.main.transform.position, Quaternion.identity);

        //audioSource.clip = audioClip;
        //audioSource.volume = volume;
        //audioSource.Play();

        //float clipLength = audioSource.clip.length;
        //Destroy(audioSource.gameObject, clipLength + 0.1f);
        //yukarýdaki þekli ile kamera konumunda spawn oluyor

        soundFXObject.PlayOneShot(audioClip, volume);
    }

    public void PlayButtonClickSound() {
        if (Time.time - lastSoundTime < soundCooldown) return;
        lastSoundTime = Time.time;
        PlaySoundClip(buttonClickSoundClip, 1f);
    }
    public void PlayButtonBackClickSound() {
        if (Time.time - lastSoundTime < soundCooldown) return;
        lastSoundTime = Time.time;
        PlaySoundClip(buttonBackClickSoundClip, 1f);
    }
    public void PlayInitiateSimulationSound() {
        PlaySoundClip(initiateSimulationSoundClip, 0.7f);
    }
    public void PlayCitySelectSound() {
        PlaySoundClip(citySelectSoundClip, 0.8f);
    }


}
