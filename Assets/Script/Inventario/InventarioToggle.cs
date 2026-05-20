using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventarioToggle : MonoBehaviour
{
    public GameObject inventarioPanel;
    private PlayerInputHandler inputHandler;
    private bool wasPressed = false;

    void Start()
    {
        inputHandler = FindObjectOfType<PlayerInputHandler>();
        inventarioPanel.SetActive(false);
    }

    void Update()
    {
        bool isPressed = inputHandler.OpenInventoryTrigger;

        if (isPressed && !wasPressed)
        {
            bool isOpen = inventarioPanel.activeSelf;
            inventarioPanel.SetActive(!isOpen);

            // blocca/sblocca cursore
            if (!isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        wasPressed = isPressed;
    }
}
