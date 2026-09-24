using UnityEngine;

public class Tool : MonoBehaviour
{
    public int toolID;
    public bool isCarried = false;
    public string vagueDescription;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.toolsInProximity[toolID] = gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.toolsInProximity.Remove(toolID);
        }
    }
}
