using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    [SerializeField] private GameData gameData;

    void Awake()
    {
        GetComponent<Renderer>().material = gameData.playerMaterials[playerID];
    }

    void Update()
    {
        
    }
}
