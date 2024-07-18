using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject ammo;

    public float attackSpeed = 2f; // shots per second
    private float cooldown;

    void Start()
    {
        cooldown = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        if (cooldown > 0) cooldown -= Time.deltaTime;
    }


    public void Attack()
    {
        if (cooldown <= 0)
        {
            Instantiate(ammo, transform.position + transform.up * 1.1f, transform.rotation);
            cooldown = 1.0f / attackSpeed;
        }
    }
}
