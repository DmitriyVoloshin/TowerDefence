using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public float health;
    public float maxHealth;
    private Vector3 localScale;

    GameObject healthFill;
    
    void Start()
    {
        healthFill = transform.Find("HealthFill").gameObject;
        maxHealth = 1.0f;
        health = maxHealth;
    }

    
    void Update()
    {
        localScale = healthFill.transform.localScale;
        transform.localScale = localScale * health / maxHealth;
    }
}
