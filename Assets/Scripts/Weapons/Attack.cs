using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    private SphereCollider _weaponHitbox;

    // Start is called before the first frame update
    void Start()
    {
        //grab and disable weapon hitbox
        _weaponHitbox = GetComponentInChildren<SphereCollider>();
        _weaponHitbox.enabled = false;
        
    }

    public void SetWeapon(Weapon weapon)
    {
        GameObject.Find("Weapon Image").GetComponent<Image>().sprite = weapon.image;
        GameObject.Find("Weapon Name").GetComponent<Text>().text = weapon.weaponName;
        _weaponHitbox.radius = weapon.length;
    }
}
