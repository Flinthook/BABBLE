using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCanvas : MonoBehaviour
{

    GameObject _soundCanvas;
    GameObject _canvasPixel;

    // Start is called before the first frame update
    void Start()
    {
        _soundCanvas = GameObject.Find("SoundCanvasObject");
        _canvasPixel = GameObject.Find("CanvasPixel");
        //lo primero que hago es desactivar el objet
        _soundCanvas.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E");
            //si el objeto esta activo lo desactivo
            if (_soundCanvas.activeSelf)
            {
                _soundCanvas.SetActive(false);
                _canvasPixel.SetActive(true);
            }
            else
            {
                _soundCanvas.SetActive(true);
                _canvasPixel.SetActive(false);
            }
        }


    }
}
