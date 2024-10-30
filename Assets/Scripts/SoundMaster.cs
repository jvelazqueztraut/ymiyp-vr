using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaster : MonoBehaviour
{
    public GameObject background;
    private List<AudioSource> audioSources;

    public float musicOnVolume = 1.0f;
    public float musicOffVolume = 0.0f;
    public float musicDelayTime = 5.0f;
    public float musicFadeTime = 1.0F;
    private float musicDelayTimer,musicFadetimer;
    private bool musicOn = true;

    public AudioLowPassFilter filter;
    public int filterOnFreq = 240;
    public int filterOffFreq = 5000;

    public float filterFadeTime = 1.0F;
    private float filterTimer;
    private bool filterOn = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = new List<AudioSource>(background.GetComponents<AudioSource>());

        musicDelayTimer = musicDelayTime;
        musicOn = true;
        musicFadetimer = 0;

        filterOn = false;
        filterTimer = 0;
        filter.cutoffFrequency = filterOffFreq;
    }

    // Update is called once per frame
    void Update()
    {
        if (musicDelayTimer > 0)
        {
            musicDelayTimer -= Time.deltaTime;
            if(musicDelayTimer <= 0)
            {
                foreach (AudioSource audio in audioSources)
                {
                    audio.Play();
                }
            }
        }
        else if (musicFadetimer > 0)
        {
            musicFadetimer -= Time.deltaTime;
            float ratio = musicFadetimer / musicFadeTime;
            if (musicOn)
            {
                foreach (AudioSource audio in audioSources)
                {
                    audio.volume = Mathf.Lerp(musicOnVolume, musicOffVolume, ratio);
                }
            }
            else
            {
                foreach (AudioSource audio in audioSources)
                {
                    audio.volume = Mathf.Lerp(musicOffVolume, musicOnVolume, ratio);
                }
            }
        }

        if (filterTimer > 0)
        {
            filterTimer -= Time.deltaTime;
            float ratio = filterTimer / filterFadeTime;
            if (filterOn)
            {
                filter.cutoffFrequency = Mathf.Lerp(filterOnFreq, filterOffFreq, ratio);
            }
            else
            {
                filter.cutoffFrequency = Mathf.Lerp(filterOffFreq, filterOnFreq, ratio);
            }
        }
    }

    public void TurnOnMusic(GameObject obj)
    {
        musicOn = true;
        musicFadetimer = musicFadetimer > 0 ? musicFadetimer : musicFadeTime;
    }

    public void TurnOffMusic(GameObject obj)
    {
        musicOn = false;
        musicFadetimer = musicFadetimer > 0 ? musicFadetimer : musicFadeTime;
    }

    public void TurnOnFilter(GameObject obj)
    {
        filterOn = true;
        filterTimer = filterTimer > 0 ? filterTimer : filterFadeTime;
    }

    public void TurnOffFilter(GameObject obj)
    {
        filterOn = false;
        filterTimer = filterTimer > 0 ? filterTimer : filterFadeTime;
    }
}
