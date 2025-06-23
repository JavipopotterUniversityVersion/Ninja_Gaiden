using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularAudios : MonoBehaviour
{
    public AudioSource audio_beach;
    public float volumeOriginal;
    // Start is called before the first frame update
    void Awake()
    {
        volumeOriginal = audio_beach.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale >0)
        {
            audio_beach.volume = volumeOriginal;
        }else if (Time.timeScale==0)
        {
            audio_beach.volume = volumeOriginal * .7f;
        }
    }
}
