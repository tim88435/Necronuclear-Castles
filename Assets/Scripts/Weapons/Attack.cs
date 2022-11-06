using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;//god i hate tmp

public class Attack : MonoBehaviour
{
    private SphereCollider _weaponHitbox;
    [SerializeField] private Weapon _weapon;//default is fist
    [SerializeField] private bool _isBlocking = false;

    // Start is called before the first frame update
    void Start()
    {
        //grab and disable weapon hitbox
        _weaponHitbox = GetComponentInChildren<SphereCollider>();
        _weaponHitbox.enabled = false;
        //set default weapon as fist
        SetWeapon(_weapon);
    }

    public void SetWeapon(Weapon weapon)//changes UI and hitbox to match weapon
    {
        GameObject.Find("Weapon Image").GetComponent<Image>().sprite = weapon.image;
        GameObject.Find("Weapon Name").GetComponent<TextMeshProUGUI>().text = weapon.weaponName;
        _weaponHitbox.radius = weapon.length;
        GameObject model = GameObject.Find("Weapon Model");
        model.GetComponent<MeshFilter>().mesh = weapon.model;
        model.GetComponent<MeshRenderer>().material = weapon.skin;
    }

    private void Update()
    {
        if (UIManager.Singleton.blockButton.buttonHeld)//if block button held
        {
            _isBlocking = true;
        }
        else
        {
            _isBlocking = false;
        }    
    }

    public void Swing()
    {
        
    }
}
