using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    [Header("PlayerData")]
    public float playerSpeed;
    public List<Material> playerMaterials = new List<Material>();
    public float stunSeconds;
}
