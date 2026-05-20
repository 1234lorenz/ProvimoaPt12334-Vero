using UnityEngine;

public class MissionToggle : MonoBehaviour
{
    public GameObject missionPanel;
    private PlayerInputHandler inputHandler;
    private bool wasPressed = false;

    void Start()
    {
        inputHandler = FindObjectOfType<PlayerInputHandler>();
        missionPanel.SetActive(false);
    }

    void Update()
    {
        bool isPressed = inputHandler.OpenMissionsTrigger;

        if (isPressed && !wasPressed)
        {
            bool isOpen = missionPanel.activeSelf;
            missionPanel.SetActive(!isOpen);

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