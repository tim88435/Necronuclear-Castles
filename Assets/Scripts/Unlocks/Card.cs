using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
public class Card : ScriptableObject
{
    public Sprite cardFace;//for unlocked card
    public Sprite lockedFace;//for locked card

    public int winsNeeded;//how many wins are required to unlock this card
    public GameObject unlock; //this will be the actual unlocked asset
}
