using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : WarEntity
{
    float age, blastRadius, damage;

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        //p.y -= 0.5f * 9.81f * age * age;
        p.z += 0.5f * 9.81f * age * age;
        transform.localPosition = p;

        if (p.z >= 0f)
        {
            GameManager.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);

            OriginFactory.Reclaim(this);
            return false;
        }

        Vector3 d = launchVelocity;
        d.z += 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);

        GameManager.SpawnExplosion().Initialize(p, 0.1f);

        return true;
    }

    Vector3 launchPoint, targetPoint, launchVelocity;
    public void Initialize(Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity,
        float blastRadius, float damage)
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
    }
}
