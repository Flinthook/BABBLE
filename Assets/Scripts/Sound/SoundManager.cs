using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    public List<AudioClip> soundClips = new List<AudioClip>();
    public List<string> soundClipInfo = new List<string>();
    public List<Sprite> cassetteImages = new List<Sprite>();

    private AudioSource audioSource;

    //Singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // <-- This line keeps it alive between scenes
        }
    }




    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /**************************************/
    /***********METODOS DE SONIDO**************/
    /**************************************/

    //método para añadir un sonido a la lista
    public void AddCassetteSound(AudioClip clip, string info, Sprite image)
    {
        if (clip != null)
        {
            soundClips.Add(clip);
            soundClipInfo.Add(info);
            cassetteImages.Add(image); // Add this line
            Debug.Log($"Se ha añadido el sonido: {clip.name} con información: '{info}'");
        }
        else
        {
            Debug.LogWarning("Se intentó añadir un AudioClip nulo al SoundManager.");
        }
    }



    public void PlaySound(AudioClip clipToPlay)
    {
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }


}