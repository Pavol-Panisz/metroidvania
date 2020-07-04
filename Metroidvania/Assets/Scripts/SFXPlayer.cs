using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [Tooltip("The gameObjects which parents all AudioSources used to play clips once")]
    [SerializeField] private GameObject playOnceHolder;

    [Tooltip("The gameObjects which parents all AudioSources used to play clips on repeat")]
    [SerializeField] private GameObject playLoopHolder;

    [Tooltip("The sounds this SFXPlayer can play")]
    [SerializeField] Sound[] sounds;

    [Tooltip("If we try to play a sound and all relevant audioSources are already playing something, allow/disallow to create a new audioSource")]
    [SerializeField] bool allowNewSources=true;

    List<AudioSourceInfo> onceAudioSourcesI = new List<AudioSourceInfo>();
    List<AudioSourceInfo> loopAudioSourcesI = new List<AudioSourceInfo>();

    private void Awake() {

        //for each AudioSource on the playOnceHolder gameObject, create an
        //AudioSourceInfo object out of it and add it to the onceAudioSourcesI list
        if (playOnceHolder) {
                foreach (AudioSource src in playOnceHolder.GetComponents<AudioSource>()) {
                onceAudioSourcesI.Add(new AudioSourceInfo(src, false));
            }
        }
        
        //same principle as above
        if (playLoopHolder) {
                foreach (AudioSource src in playLoopHolder.GetComponents<AudioSource>()) {
                loopAudioSourcesI.Add(new AudioSourceInfo(src, true));
            }
        }
    }

    private void Update() {

        //for each sound playing in loopingAudioSources, if it finished a loop, change the pitch
        float pitchVal;
        foreach (AudioSourceInfo si in loopAudioSourcesI) {
            #region explanation
            //if AudioSource.timeSamples this frame is smaller than last frame, it can either mean
            //that the clip is being played backwards, or that it started playing from sample 0.
            #endregion explanation
            
            if (si.source.isPlaying && (si.source.timeSamples < si.samplesPosLastF)) {
                pitchVal = si.sound.pitch + Random.Range(-si.sound.pitchModulation, si.sound.pitchModulation);
                si.source.pitch = Mathf.Clamp(pitchVal, -3f, 3f);
            }
            si.samplesPosLastF = si.source.timeSamples;
        }
    }

    public void TryPlayOnce(string name) {
        Sound s = GetSound(name);
        TryPlayInSources(s, onceAudioSourcesI);
    }
    public void TryPlayOnce(Sound s) {
        TryPlayInSources(s, onceAudioSourcesI);
    }

    
    public void TryStartPlayLooped(string name) {
        Sound s = GetSound(name);
        TryPlayInSources(s, loopAudioSourcesI);
    }
    public void TryStopPlayLooped(string name) {
        AudioClip clip = GetSound(name).clip;

        for (int iii=0; iii < loopAudioSourcesI.Count; iii++) {
            if (ReferenceEquals(clip, loopAudioSourcesI[iii].source.clip)) {
                loopAudioSourcesI[iii].source.Stop();
                return;
            }
        }

        //Debug.Log("Tried to stop playing on loop '"+name+"', but it wasn't being played");
    }

    private void TryPlayInSources(Sound s, List<AudioSourceInfo> sourceIs) {
        foreach(AudioSourceInfo sourceInfo in sourceIs) {
            if (sourceInfo.source.isPlaying == false) {
                PlayInSource(s, sourceInfo);
                return;
            }
        }

        //if we made it to this point in code, it means that every audioSource in the
        //provided audioSource list was already playing something

        if (allowNewSources) {

            bool looping=false;
            AudioSourceInfo newSourceI=null;
            if (ReferenceEquals(sourceIs, loopAudioSourcesI)) {
                looping = true;
                newSourceI = new AudioSourceInfo(playLoopHolder.AddComponent<AudioSource>(), looping);
            }
            else if (ReferenceEquals(sourceIs, onceAudioSourcesI)) {
                newSourceI = new AudioSourceInfo(playOnceHolder.AddComponent<AudioSource>(), looping);
            }
            else {
                Debug.Log("during the setup of a new AudioSourceInfo, the sourceInfo-list wasn't recognized");
                return;
            }

            sourceIs.Add(newSourceI);
            PlayInSource(s, newSourceI);
        }
        else {
            Debug.Log("tried to play sound '"+name+"', but all relevant audioSources were already playing something and new ones can't be added");
        }
    }

    private void PlayInSource(Sound s, AudioSourceInfo sourceInfo) {
        AudioSource source = sourceInfo.source;
        source.Stop();
        source.clip = null;

        float p = s.pitch + Random.Range(-s.pitchModulation, s.pitchModulation);
        source.pitch = p;
        source.volume = s.volume;
        source.maxDistance = s.maxDistance; //DEBUG: NOT PLAYING
        source.clip = s.clip;


        source.Play();

        //reset the playback info
        sourceInfo.samplesPosLastF = 0;

        sourceInfo.sound = s;
    }


    public Sound GetSound(string name) {
        foreach (Sound s in sounds) {
            if (name == s.name) {
                return s;
            }
        }

        Debug.Log("Sound '"+name+"' couldn't be found");
        return null;
    }

}

//sound asset in the inspector
[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    [Range(-0f, 1f)] public float volume=0;
    [Range(-3f, 3f)] public float pitch=0;
    public float pitchModulation=0;
    public float maxDistance=0;

    public Sound() {
        if (volume == 0) volume = 1f;
        if (pitch == 0) pitch = 1f;
        if (maxDistance == 0) maxDistance = 6f;
    }
}

//holds an audioSource & playback information used when changing the pitch
[System.Serializable]
public class AudioSourceInfo {
    public AudioSource source;
    [HideInInspector] public int samplesPosLastF=-100000; //negative, so that it triggers a pitch change right away
    [HideInInspector] public Sound sound;

    public AudioSourceInfo(AudioSource s, bool looping) {
        source = s;
        source.loop = looping;
        
        source.playOnAwake = false;
        source.spatialBlend = 1f;
        
    }
}