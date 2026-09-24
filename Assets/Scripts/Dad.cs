using UnityEngine;

public class Dad : MonoBehaviour
{
    [SerializeField] private int toolAmount;
    private int chosenToolID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        System.Random random = new System.Random();
        chosenToolID = random.Next(toolAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player.carriedTool != null)
            {
                Tool tool = player.carriedTool.GetComponent<Tool>();
                if (chosenToolID == tool.toolID)
                {
                    // player wins
                }
                else
                {
                    // stun player
                    // angry message
                    player.Drop();
                }
            }
        }
    }
}
