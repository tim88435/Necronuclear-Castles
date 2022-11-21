using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using System;

public class ItemPickup : MonoBehaviour
{
    public static Dictionary<ushort, GameObject> weaponList = new Dictionary<ushort, GameObject>();
    private ushort key;
    public Weapon weapon;
    private bool serverSpawned = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (NetworkManager.IsHost)
        {
            weaponList.Add((ushort)gameObject.GetInstanceID(), gameObject);
            key = (ushort)gameObject.GetInstanceID();
            serverSpawned = true;
            //Debug.Log(weaponList.Count);
        }
        else if (!serverSpawned)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Start()
    {
        if (NetworkManager.IsHost)
        {
            SendSpawnedItem();
        }
    }
    private void SendSpawnedItem()
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageIdentification.itemSpawn);
        message.AddVector3(transform.position);
        message.AddVector3(transform.eulerAngles);
        message.AddUShort(GameManager.GetWeapon(weapon));
        message.AddUShort(key);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    [MessageHandler((ushort)MessageIdentification.itemSpawn)]
    public static void GetSpawnedItem(Message message)
    {
        SpawnItem(message.GetVector3(), message.GetVector3(), GameManager.GetWeapon(message.GetUShort()), message.GetUShort());
    }
    private static void SpawnItem(Vector3 itemPosition, Vector3 itemRotation, GameObject itemWeapon, ushort itemKey)
    {
        //if (weaponList.ContainsKey)
        {

        }
        GameObject newWeapon = Instantiate(itemWeapon, itemPosition, Quaternion.Euler(itemRotation));
        ItemPickup newPickup = newWeapon.GetComponent<ItemPickup>();
        newPickup.key = itemKey;
        newPickup.serverSpawned = true;
    }
    //finds and returns weapon with weapon name
    public static GameObject FindItem(ushort ID)
    {
        if (weaponList.ContainsKey(ID))
        {
            return weaponList[ID];
        }
        //if item not found
        Debug.LogWarning($"Weapon with id {ID} not found in the scene");
        return null;
    }
}
