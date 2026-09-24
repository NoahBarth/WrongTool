using System.Collections.Generic;
using UnityEngine;

public class Dad : MonoBehaviour
{
    [SerializeField] private List<GameObject> tools = new List<GameObject>();
    [SerializeField] private List<GameObject> players = new List<GameObject>();
    private int chosenToolID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < tools.Count; i++)
        {
            tools[i].GetComponent<Tool>().toolID = i;
        }

        System.Random random = new System.Random();
        chosenToolID = random.Next(tools.Count);
        string message = tools[chosenToolID].GetComponent<Tool>().vagueDescription;

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().ReceiveMessage(message);
        }
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
                    player.GetComponent<PlayerController>().ReceiveMessage("You Win :)");
                }
                else
                {
                    player.GetComponent<PlayerController>().ReceiveMessage("You fool! Wrong Tool!");
                    player.Stun();
                }
            }
        }
    }
}
