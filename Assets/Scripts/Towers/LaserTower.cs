using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    [SerializeField]
    Transform turret = default, laserBeam = default;

    [SerializeField]
    TargetPoint target;

    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    Vector3 laserBeamScale;

    public override TowerType TowerType => TowerType.Laser;

    void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTarget(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
            Debug.Log("Searching for target...");
        }
    }

    void Shoot()
    {
        Vector3 direction = turret.position - target.Position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        turret.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + 90);
        laserBeam.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);

        float d = Vector3.Distance(turret.position, target.Position);
        laserBeamScale.x = d;
        laserBeam.localScale = laserBeamScale;
        //laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.right;
        laserBeam.localPosition = turret.localPosition + 0.5f * d * turret.up;

        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }



}
