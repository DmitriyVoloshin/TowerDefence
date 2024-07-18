using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MortarTower : Tower
{
    public override TowerType TowerType => TowerType.Mortar;

    [SerializeField]
    Transform mortar = default;

    [SerializeField]
    TargetPoint target;

    [SerializeField, Range(0.5f, 2f)]
    float shotsPerSecond = 1f;

    float launchSpeed;

    float launchProgress;

    [SerializeField, Range(0.5f, 3f)]
    float shellBlastRadius = 1f;

    [SerializeField, Range(1f, 100f)]
    float shellDamage = 10f;

    void Awake()
    {
        OnValidate();
    }

    void OnValidate()
    {
        float x = targetingRange + 0.25001f;
        float y = mortar.position.z;
        launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    public override void GameUpdate()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        while (launchProgress >= 1f)
        {
            if (AcquireTarget(out TargetPoint target))
            {
                Launch(target);
                launchProgress -= 1f;
            }
            else
            {
                launchProgress = 0.999f;
            }
        }
    }

    public void Launch(TargetPoint target)
    {
        Vector3 launchPoint = mortar.position;
        Vector3 launchPointGrounded = launchPoint;
        launchPointGrounded.z = 0;

        Vector3 targetPoint = target.Position;
        targetPoint.z = 0;

        //Debug.DrawLine(launchPoint, targetPoint, Color.yellow);
        //Debug.DrawLine(launchPointGrounded, targetPoint, Color.white);

        float x1 = Vector3.Distance(launchPointGrounded, targetPoint);
        float y1 = 0;

        float y0 = Vector3.Distance(launchPointGrounded, launchPoint);

        float g = 9.81f;
        float v = launchSpeed;
        float v2 = v*v;

        float a = -g * x1*x1 / 2 / v2;
        float b = x1;
        float c = a - y1 + y0;

        float D = b*b - 4*a*c;
        if (D < 0)
        {
            Debug.Log("Launch velocity insufficient for range!");
            return;
        }
        float sqrtD = Mathf.Sqrt(D);

        float tanTheta = (-b-sqrtD)/2/a;
        float theta = Mathf.Atan(tanTheta) * 180.0f / Mathf.PI;

        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;


        //Vector3 prev = launchPoint, next = launchPoint;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.y - launchPoint.y;

        float angle = Mathf.Atan2(dir.y, dir.x);
        mortar.localRotation = Quaternion.Euler(0f,0f, angle * Mathf.Rad2Deg - 90) * Quaternion.Euler(-theta, 0f, 0f);

        GameManager.SpawnShell().Initialize(launchPoint, targetPoint,
            new Vector3(launchSpeed * cosTheta * dir.x / dir.magnitude, launchSpeed * cosTheta * dir.y / dir.magnitude, -launchSpeed * sinTheta),
            0.5f, 1.5f);

        /*
        float magnitude = dir.magnitude;

        dir.x = dir.x / magnitude;
        dir.y = dir.y / magnitude;

        for (int i = 1; i <= 12; ++i)
        {
            float t = i / 10f;

            float dx = v * cosTheta * t;
            float dz = v * sinTheta * t - 0.5f * g * t * t;

            next = launchPoint + new Vector3(dir.x * dx, dir.y * dx, -dz);

            Color col;

            if (i%2 == 0)
            {
                col = Color.white;
            }
            else
            {
                col = Color.red;
            }
            Debug.DrawLine(prev, next, col, 2f);
            prev = next;
        }*/
    }
}
