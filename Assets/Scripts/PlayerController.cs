using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    [SerializeField] private GameData gameData;
    public Dictionary<int, GameObject> toolsInProximity = new Dictionary<int, GameObject>();
    public GameObject carriedTool = null;
    public TextMeshPro messageBox;
    private bool stunned = false;

    private CharacterController controller;
    private Camera playerCamera;
    private PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 100f;
    private float verticalVelocity;
[SerializeField] private float gravity = -9.81f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch;
    private float cameraYaw;

    void Awake()
    {
        GetComponent<Renderer>().material = gameData.playerMaterials[playerID];
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponent<Camera>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        // TODO uncomment before Building project
        // if (Display.displays.Length < 2)
        // {
        //     Debug.LogError("Mindestens zwei Displays erforderlich.");
        //     return;
        // }
        // Display.displays[playerID].Activate();
        // GetComponent<Camera>().targetDisplay = playerID+1;
        playerCamera.targetDisplay = playerID;
        playerInput.user.UnpairDevices();

        //  if (Gamepad.all.Count > playerID)
        //  {
            // playerInput.actions.devices = new InputDevice[] { Gamepad.all[playerID] };
        //  }
        //  else
        //  {
        //      Debug.LogError($"No gamepad found for Player {playerID}");
        //  }
            playerInput.actions.devices = new InputDevice[] { Gamepad.all[0] };
    }

    void Update()
    {
        if (!stunned)
        {
            Move();
            Look();
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnPickUp(InputValue value)
    {
        if (carriedTool == null)
	    {
	    	PickUp();
	    }
	    else
	    {
	    	Drop();
	    }
    }

    private void Move()
    {
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Keep movement horizontal
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement =
            right * moveInput.x +
            forward * moveInput.y;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        movement.y = verticalVelocity;

        controller.Move(
            movement * moveSpeed * Time.deltaTime
        );
    }

    private void Look()
    {
        float horizontal =
            lookInput.x * lookSensitivity * Time.deltaTime;

        float vertical =
            lookInput.y * lookSensitivity * Time.deltaTime;

        cameraYaw += horizontal;
        cameraPitch -= vertical;
        cameraPitch = Mathf.Clamp(cameraPitch, -89f, 89f);

        playerCamera.transform.localRotation =
            Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    void PickUp()
    {
        if (toolsInProximity.Count > 0)
	    {
	    	GameObject chosenTool = null;
            foreach (KeyValuePair<int, GameObject> tool in toolsInProximity)
            {
                if (chosenTool == null)
                {
                    chosenTool = tool.Value;
                }
                else
                {
                    if (Vector3.Distance(transform.position, chosenTool.transform.position) >
                        Vector3.Distance(transform.position, tool.Value.transform.position))
                    {
                        chosenTool = tool.Value;
                    }
                }
            }

            carriedTool = chosenTool;
            carriedTool.transform.parent = gameObject.transform;
            carriedTool.GetComponent<Tool>().isCarried = true;
	    }
    }

    public void Drop()
    {
        carriedTool.transform.parent = null;
        carriedTool.GetComponent<Tool>().isCarried = false;
        carriedTool = null;
    }

    public void Stun()
    {
        stunned = true;
        Drop();
        StartCoroutine(StunCountdown());
    }

    private IEnumerator StunCountdown()
    {
        yield return new WaitForSeconds(gameData.stunSeconds);
        stunned = false;
    }

    public void ReceiveMessage(string message)
    {
    	messageBox.text = message;
    	StartCoroutine(MessageCooldown());
    }

    private IEnumerator MessageCooldown()
    {
    	yield return new WaitForSeconds(gameData.messageTimerInSeconds);
    	messageBox.text = "";
    }
}
