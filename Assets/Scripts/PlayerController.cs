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
    public GameObject messageBox;
    private bool stunned = false;

    private CharacterController controller;
    private Camera playerCamera;
    private PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveSpeedWithItem = 2.5f;
    [SerializeField] private float lookSensitivity = 100f;
    private float verticalVelocity;
    [SerializeField] private float gravity = -9.81f;
    public GameObject otherPlayer;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch;
    private float cameraYaw;

    private bool punchable;
    private bool punchReady = true;
    private Renderer playerRenderer;
    private Color originalColor;

    void Awake()
    {
        // GetComponent<Renderer>().material = gameData.playerMaterials[playerID];
        playerRenderer = GetComponent<Renderer>();
        playerRenderer.material = gameData.playerMaterials[playerID];
        originalColor = playerRenderer.material.color;
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponent<Camera>();
        playerInput = GetComponent<PlayerInput>();

        string messageBoxLayerName = "player" + playerID + "Only";
        int messageBoxLayer = LayerMask.NameToLayer(messageBoxLayerName);
        messageBox.layer = messageBoxLayer;
        GetComponent<Camera>().cullingMask |= LayerMask.GetMask(messageBoxLayerName);
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
        playerInput.actions.devices = new InputDevice[] { Gamepad.all[playerID] };
        punchable = false;
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
        if (carriedTool == null && !stunned)
        {
            PickUp();
        }
        else
        {
            Drop();
        }
    }

    public void OnAttack()
    {
        if(punchable && punchReady)
        {
            otherPlayer.GetComponent<PlayerController>().Stun();
            StartCoroutine(PunchCooldown());
            Debug.Log("Executed attack");
        }
    }

    private IEnumerator PunchCooldown()
    {
        punchReady = false;
        yield return new WaitForSeconds(7f);
        punchReady = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null && other.gameObject != gameObject)
        {
            punchable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {

        punchable = false;

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

        float effectiveMoveSpeed = carriedTool == null ? moveSpeed : moveSpeedWithItem;

        controller.Move(
              movement * effectiveMoveSpeed * Time.deltaTime
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
        if (carriedTool != null)
        {
            carriedTool.transform.parent = null;
            carriedTool.GetComponent<Tool>().isCarried = false;
            carriedTool = null;
        }
    }

    public void Stun()
    {
        stunned = true;
        Debug.Log("Inside stun");
        Drop();
        StartCoroutine(StunCountdown());
    }

    private IEnumerator StunCountdown()
    {
        // yield return new WaitForSeconds(gameData.stunSeconds);
        // stunned = false;
        float elapsed = 0f;
        while (elapsed < gameData.stunSeconds)
        {
            playerRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.3f);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.3f);
            elapsed += 0.6f;
        }
        playerRenderer.material.color = originalColor;
        stunned = false;
    }

    public void ReceiveMessage(string message)
    {
        messageBox.GetComponent<TextMeshPro>().text = message;
        StartCoroutine(MessageCooldown());
    }

    private IEnumerator MessageCooldown()
    {
        yield return new WaitForSeconds(gameData.messageTimerInSeconds);
        messageBox.GetComponent<TextMeshPro>().text = "";
    }
}
