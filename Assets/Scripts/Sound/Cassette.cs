using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cassette : MonoBehaviour
{
    public AudioClip audioClip;
    public string audioInfo;
    public Sprite cassetteImage; // Assign in Inspector
    public AudioClip collectSound; // Assign in Inspector

    // Animation parameters
    public float rotationSpeed = 45f; // degrees per second
    public float bobAmplitude = 0.2f; // how high to bob
    public float bobFrequency = 1f;   // how fast to bob

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Spin around Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider col)
    {
        //si el objeto que colisiona tiene la etiqueta "Player" agregamos el sonido a la lista de sonidos
        if (col.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.AddCassetteSound(audioClip, audioInfo, cassetteImage);

            // Play collect sound at cassette position
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            Destroy(gameObject);
        }
    }
}
