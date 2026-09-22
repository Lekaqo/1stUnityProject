using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelControl : MonoBehaviour
{
    public GameObject expEffect;
    public Transform tr;
    public Health brHealth;
    private void Start()
    {
        tr = GetComponent<Transform>();
        brHealth = GetComponent<Health>();
        brHealth.DeadTime = 5f;
    }
    private void Update()
    {
        if(brHealth.CurrentHealth == 0)
        {
            ExpBarrel();
        }
    }
    private void ExpBarrel()
    {
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);

        foreach (Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if (rbody != null)
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce
                    (
                    explosionForce: 500f, 
                    explosionPosition: tr.position, 
                    explosionRadius: 10.0f, 
                    upwardsModifier: 300.0f
                    );
            }
        }
    }

}
