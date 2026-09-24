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
            // add itself to players close items dictionary with id as key and its script instance as value
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            // remove itself from players close items dictionary via id
        }
    }
}
