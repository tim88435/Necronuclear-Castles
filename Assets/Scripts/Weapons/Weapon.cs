using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public float damage;//weapon damage
    public float cooldown;//weapon cooldown in seconds
    public float length;//weapon length

    public GameObject model;//weapon model
    public GameObject image;//weapon image in the player UI
    public string weaponName;//name of the weapon, to be displayed in the UI
}
