using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation firstConversation;
    [SerializeField] private NPCConversation repeatConversation;

    private bool hasTalked = false;

    private PlayerMovement playerMovement;
    private PlayerCam cameraLook;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        cameraLook = FindObjectOfType<PlayerCam>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasTalked)
                {
                    ConversationManager.Instance.StartConversation(firstConversation);
                    hasTalked = true;
                }
                else
                {
                    ConversationManager.Instance.StartConversation(repeatConversation);
                }

                if (playerMovement != null)
                    playerMovement.enabled = false;
                if (cameraLook != null)
                    cameraLook.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                ConversationManager.OnConversationEnded += OnConversationEnded;
            }
        }
    }

    private void OnConversationEnded()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (cameraLook != null)
            cameraLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ConversationManager.OnConversationEnded -= OnConversationEnded;
    }
}
