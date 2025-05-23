using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cassette : MonoBehaviour
{

    public AudioClip audioClip;
    public string audioInfo;



    void OnTriggerEnter(Collider col)
    {

        //si el objeto que colisiona tiene la etiqueta "Player" agregamos el sonido a la lista de sonidos
        if (col.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.AddCassetteSound(audioClip, audioInfo);
            Destroy(gameObject);
        }
 
    }



}
