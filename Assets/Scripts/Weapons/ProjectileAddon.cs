using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ProjectileAddon : MonoBehaviour
{
    public int damage;
    private Rigidbody rb;
    private PlayerShooter gunHolder;

    private bool targetHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // make sure only to stick to the first target you hit
        if (targetHit)
            return;

        else
            targetHit = true;

        // check if you hit an enemy
        if(collision.gameObject.GetComponent<Enemy>() !=null)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            DamageMessage damageMessage;

            RaycastHit hit;
            hit = GetComponent<RaycastHit>();
            IHitable hitable = hit.transform.GetComponent<IHitable>();
            hitable?.Hit(hit, damage);

            damageMessage.damager = gunHolder.gameObject;
            damageMessage.amount = damage;
            damageMessage.hitPoint = hit.point;
            damageMessage.hitNormal = hit.normal;


            enemy.ApplyDamage(damageMessage);
        }

        // make sure projectile sticks to surface
        rb.isKinematic = true;

        // make sure projectile moves with target
        transform.SetParent(collision.transform);
    }
}
