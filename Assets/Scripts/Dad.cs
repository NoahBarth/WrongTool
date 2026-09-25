using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dad : MonoBehaviour
{
    [SerializeField] private List<GameObject> tools = new List<GameObject>();
    [SerializeField] private List<GameObject> players = new List<GameObject>();
    [SerializeField] private GameData gameData;
    private int chosenToolID;
    private string message;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < tools.Count; i++)
        {
            tools[i].GetComponent<Tool>().toolID = i;
        }

        System.Random random = new System.Random();
        chosenToolID = random.Next(tools.Count);
        message = tools[chosenToolID].GetComponent<Tool>().vagueDescription;

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
                    player.ReceiveMessage("You Win :)");
                }
                else
                {
                    player.ReceiveMessage("You fool! Wrong Tool!");
                    StartCoroutine(ToolMessage(player));
                    player.Stun();
                }
            }
        }
    }

    private IEnumerator ToolMessage(PlayerController player)
    {
    	yield return new WaitForSeconds(gameData.messageTimerInSeconds + 1);
    	player.ReceiveMessage(message);
    }
}
