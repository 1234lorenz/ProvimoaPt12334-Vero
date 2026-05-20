using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 Script 1 - PlayerInputHandler

"Cosa sta premendo il giocatore?"


Ascolta tastiera/mouse/controller
Salva i valori in proprietà pubbliche
Non sa nulla di movimento, fisica, ecc.
 
 */

public class PlayerInputHandler : MonoBehaviour//permette di dare il coedice a un gameobject
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string collect = "Collect"; // in Header Action Name Reference
    [SerializeField] private string openBin = "OpenBin"; // in Header
    [SerializeField] private string openInventory = "OpenInventory";
    [SerializeField] private string openMissions = "OpenMissions";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction collectAction;
    private InputAction openBinAction;
    private InputAction openInventoryAction;
    private InputAction openMissionsAction;



    public Vector2 MovementInput { get; private set; }//MovementInput è un Vector2 perché l'input da tastiera/joystick è bidimensionale:

    public Vector2 RotationInput { get; private set; }


    public bool JumpTrigger { get; private set; }

    public bool SprintTrigger { get; private set; }
    public bool CollectTrigger { get; private set; }
    public bool OpenBinTrigger { get; private set; }
    public bool OpenInventoryTrigger { get; private set; }
    public bool OpenMissionsTrigger { get; private set; }



    private void Awake()
    {
        InputActionMap mapRefence = playerControls.FindActionMap(actionMapName);


        movementAction = mapRefence.FindAction(movement);
        rotationAction = mapRefence.FindAction(rotation);
        jumpAction = mapRefence.FindAction(jump);
        sprintAction = mapRefence.FindAction(sprint);
        collectAction = mapRefence.FindAction(collect);
        openBinAction = mapRefence.FindAction(openBin);
        openInventoryAction = mapRefence.FindAction(openInventory);
        openMissionsAction = mapRefence.FindAction(openMissions);
        SubscribeActionValueToInputEvents();
    }

    private void SubscribeActionValueToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTrigger = true;
        jumpAction.canceled += inputInfo => JumpTrigger = false;

        sprintAction.performed += inputInfo => SprintTrigger = true;
        sprintAction.canceled += inputInfo => SprintTrigger = false;

        
        collectAction.performed += inputInfo => CollectTrigger = true;
        collectAction.canceled += inputInfo => CollectTrigger = false;

        openBinAction.performed += inputInfo => OpenBinTrigger = true;
        openBinAction.canceled += inputInfo => OpenBinTrigger = false;

        openInventoryAction.performed += inputInfo => OpenInventoryTrigger = true;
        openInventoryAction.canceled += inputInfo => OpenInventoryTrigger = false;

        openMissionsAction.performed += inputInfo => OpenMissionsTrigger = true;
        openMissionsAction.canceled += inputInfo => OpenMissionsTrigger = false;

    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
