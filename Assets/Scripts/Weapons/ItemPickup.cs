using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public static List<GameObject> weaponList = new List<GameObject>();
    public int itemID;
    public Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        itemID = weaponList.Count;//this helps autogenerate item id at the start of the game
        //very unflexible, does not help if items are spawned mid-game
        weaponList.Add(gameObject);
        //Debug.Log(weaponList.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
