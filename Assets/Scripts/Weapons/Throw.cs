using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public float throwForce;
    public float throwUpwardForce;

    public bool readyToThrow { get; set; }

    private void Start()
    {
        readyToThrow = true;
    }

    public void Throwing()
    {
        readyToThrow = false;

        if (totalThrows>0)
        { // instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // calculate direction
        Vector3 forceDirection = attackPoint.forward;

        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        // implement throwCooldown
        Invoke(nameof(ResetThrow), throwCooldown);
        }

    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

}
