using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    [SerializeField] private GameData gameData;
    public GameObject carriedTool = null;
    private bool stunned = false;

    private CharacterController controller;
    private Camera playerCamera;
    private PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 100f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch;

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

        // if (Gamepad.all.Count > playerID)
        // {
            playerInput.actions.devices = new InputDevice[] { Gamepad.all[0] };
        // }
        // else
        // {
        //     Debug.LogError($"No gamepad found for Player {playerID}");
        // }
    }

    void Update()
    {
        // TODO uncomment when ready
        //float horizontalInput = 1; // TODO input system
        //float verticalInput = 1; // TODO input system
        //
        //Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        //transform.Translate(gameData.playerSpeed * Time.deltaTime * direction);
        Move();
        Look();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void Move()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Remove any vertical component
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = right * moveInput.x + forward * moveInput.y;

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void Look()
    {
        float horizontal = lookInput.x * lookSensitivity * Time.deltaTime;

        float vertical = lookInput.y * lookSensitivity * Time.deltaTime;

        // Rotate player horizontally
        transform.Rotate(0f, horizontal, 0f);

        // Rotate camera vertically
        cameraPitch -= vertical;

        cameraPitch = Mathf.Clamp(cameraPitch, -89f, 89f);

        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Tool>() != null && carriedTool == null)
        {
            if (!collision.gameObject.GetComponent<Tool>().isCarried)
            {
                carriedTool = collision.gameObject;
                carriedTool.transform.parent = gameObject.transform;
                carriedTool.GetComponent<Tool>().isCarried = true;
            }
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
        StartCoroutine(StunCountdown());
    }

    private IEnumerator StunCountdown()
    {
        yield return new WaitForSeconds(gameData.stunSeconds);
        stunned = false;
    }
}
