using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
//using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Tower : GameTileContent
{
    const int enemyLayerMask = 1 << 9;
    static Collider[] targetsBuffer = new Collider[2];

    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }
    public abstract TowerType TowerType { get; }

    protected bool TrackTarget(ref TargetPoint target)
    {
        if (target == null)
        {
            return false;
        }
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        if (Vector3.Distance(a, b) > targetingRange + 0.125f)
        {
            target = null;
            return false;
        }
        return true;
    }

    protected bool AcquireTarget(out TargetPoint target)
    {
        if (TargetPoint.FillBuffer(transform.localPosition, targetingRange))
        {
            target = TargetPoint.RandomBuffered;
            return true;
        }
        target = null;
        return false;
    }

}







/*public List<GameObject> enemiesInRange;
   public GameObject myWeapon;

   private float turnSpeed = 50.0f; // per second

   void Start()
   {
       enemiesInRange = new List<GameObject>();

       myWeapon = gameObject.transform.Find("Weapon").gameObject;
   }

   void Update()
   {

       if (EmeniesPresent())
       {
           Vector3 vectorToTarget = enemiesInRange[0].transform.position - transform.position;

           float angle = Vector3.SignedAngle(vectorToTarget, transform.up, Vector3.forward);
           float angleToRotate = turnSpeed * Time.deltaTime;

           if (Mathf.Abs(angle) <= Mathf.Abs(angleToRotate))
           {
               transform.Rotate(Vector3.back, angle);
           }
           else
           {
               transform.Rotate(Vector3.back, Mathf.Sign(angle) * angleToRotate);
           }

           myWeapon.GetComponent<Cannon>().Attack();
       }
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
       if (collision.gameObject.CompareTag("Enemy"))
       {
           enemiesInRange.Add(collision.gameObject);
       }
   }
   private void OnTriggerExit2D(Collider2D collision)
   {
       if (collision.gameObject.CompareTag("Enemy"))
       {
           enemiesInRange.Remove(collision.gameObject);
       }
   }

   bool EmeniesPresent()
   {
       return Enumerable.Count(enemiesInRange) > 0;
   }*/