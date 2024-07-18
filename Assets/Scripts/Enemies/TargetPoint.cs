using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetPoint : MonoBehaviour
{
    public Enemy Enemy { get; private set; }

    public Vector3 Position => transform.position;

    void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Target point without Enemy root!", this);
        Debug.Assert(GetComponent<SphereCollider>() != null, "Target point without sphere collider!", this);
        Debug.Assert(gameObject.layer == 9, "Target point on wrong layer!", this);
    }

    public static TargetPoint RandomBuffered => GetBuffered(Random.Range(0, BufferedCount));

    const int enemyLayerMask = 1 << 9;

    static Collider[] buffer = new Collider[2];

    public static int BufferedCount { get; private set; }

    public static bool FillBuffer(Vector3 position, float range)
    {
        int hits = Physics.OverlapSphereNonAlloc(position, range, buffer, enemyLayerMask);
        if (hits > 0)
        {
            return true;
        }
        return false;
    }

    public static TargetPoint GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<TargetPoint>();
        Debug.Assert(target != null, "Targeted non-enemy!", buffer[0]);
        return target;
    }





}
