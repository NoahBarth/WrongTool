using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    [SerializeField] private GameData gameData;
    public GameObject carriedTool = null;

    void Awake()
    {
        GetComponent<Renderer>().material = gameData.playerMaterials[playerID];
    }

    void Update()
    {
        // TODO uncomment when ready
        //float horizontalInput = 1; // TODO input system
        //float verticalInput = 1; // TODO input system
        //
        //Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        //transform.Translate(gameData.playerSpeed * Time.deltaTime * direction);
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

    void Drop()
    {
        if (carriedTool != null)
        {
            carriedTool.transform.parent = null;
            carriedTool.GetComponent<Tool>().isCarried = false;
            carriedTool = null;
        }
    }
}
