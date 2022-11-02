using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unlock : MonoBehaviour
{
    [SerializeField] private Card card;//what we will be getting data from
    private Image face;//image that is shown to the player
    // Start is called before the first frame update
    void Start()
    {
        face = GetComponent<Image>();
        //gets different image depending on number of wins
        if (card.winsNeeded <= UIManager.Singleton.gamesWon)
        {
            face.sprite = card.cardFace;
        }
        else
        {
            face.sprite = card.lockedFace;
            GetComponent<Button>().interactable = false;//this disables button if number of wins is too low
        }
    }

    public void SelectCostume()
    {
        UIManager.Singleton._playerSkin = card.unlock;//replaces player model with model on the card
    }
}
