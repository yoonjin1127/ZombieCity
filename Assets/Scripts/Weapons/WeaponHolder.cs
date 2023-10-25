using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] Gun gun;
     Grenade grenade;
     PlayerShooter shooter;

    // List<Gun> gunList = new List<Gun>();

    public void Fire()
    {
        Debug.Log("Fire");
        gun.Fire();
    }

    public void Reload()
    {
        Debug.Log("Reload");
        gun.Reload();
    }

}
