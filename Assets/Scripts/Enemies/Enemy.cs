using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Enemy : GameBehavior
{
    EnemyFactory originFactory;

    DirectionChange directionChange;
    [SerializeField]
    Direction direction;
    [SerializeField]
    GameTile currentTile, nextTile;
    Vector3 startPosition, endPosition;
    Quaternion startRotation, endRotation;

    float Health { get; set; }

    [SerializeField]
    float pathOffset;

    float progress, progressFactor;

    [SerializeField]
    Transform model = default;

    Vector3 zOffset = new Vector3(0, 0, -0.2f);

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }
    public void Initialize(float scale, float offset)
    {
        model.localScale = new Vector3(scale, scale, scale);
        pathOffset = offset;
        Health = 100f * scale;
    }
    public void ApplyDamage(float damage)
    {
        Debug.Assert(damage >= 0f, "Negative damage applied.");
        Health -= damage;
    }

    void SetPosition(Vector3 position)
    {
        position.z = -0.2f;
        transform.localPosition = position;
    }

    void SetModelPosition(Vector3 position)
    {
        position.z = -0.2f;
        model.localPosition = position;
    }
    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        
        currentTile = tile;
        nextTile = tile.NextTileOnPath;
        direction = currentTile.PathDirection;

        startPosition = currentTile.transform.localPosition + new Vector3(-pathOffset, 0, 0);
        endPosition = currentTile.ExitPoint + zOffset + new Vector3(-pathOffset,0,0);

        SetPosition(startPosition);
        transform.localRotation = currentTile.PathDirection.GetRotation();
        progress = 0f;
        progressFactor = 2.0f;
    }

    void PrepareOutro()
    {
        startPosition = endPosition + new Vector3(-pathOffset, 0, 0);
        endPosition = currentTile.transform.localPosition + new Vector3(-pathOffset, 0, 0);
        SetPosition(startPosition);
        directionChange = DirectionChange.None;

        SetModelPosition(Vector3.zero);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }

    public override bool GameUpdate()
    {
        if (Health <= 0f)
        {
            OriginFactory.Reclaim(this);
            return false;
        }

        progress += Time.deltaTime * progressFactor;

        if (progress >= 1.0f)
        {
            if (nextTile == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            transform.rotation = currentTile.PathDirection.GetRotation();

            startPosition = currentTile.ExitPoint + zOffset;
            endPosition = nextTile.ExitPoint + zOffset;

            currentTile = nextTile;
            nextTile = nextTile.NextTileOnPath;

            if (nextTile == null)
            {
                progressFactor = 2f;
                directionChange = DirectionChange.None;
                SetModelPosition(Vector3.zero + new Vector3(pathOffset, 0, 0));
                transform.localRotation = direction.GetRotation();
            }
            else
            {
                startRotation = transform.rotation;
                endRotation = currentTile.PathDirection.GetRotation();

                directionChange = direction.GetDirectionChangeTo(currentTile.PathDirection);
                direction = currentTile.PathDirection;
                switch (directionChange)
                {
                    case DirectionChange.None: PrepareForward(); break;
                    case DirectionChange.TurnRight: PrepareTurnRight(); break;
                    case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
                    default: PrepareTurnAround(); break;
                }
            }
            
            progress = (progress - 1f) / progressFactor;
        }
        if (directionChange == DirectionChange.None)
        {
            SetPosition(Vector3.LerpUnclamped(startPosition, endPosition, progress));
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, progress);
        }
        return true;
    }

    /*
    * Make turning by moving through arc
    * 
    * float progress = speed * Time.deltaTime / pi*D/4;
    * float angle = 90 * progress;
    * 
    * right turn in relative normalized coordinates:
    * y = sin(angle);
    * x = 1 - cos(angle);
    * Vector3 position = (x,y,0);
    * 
    * left turn:
    * y = sin(angle);
    * x = 1 - cos(angle)?? not checked
    */

    void PrepareForward()
    {
        SetPosition(startPosition);
        transform.localRotation = currentTile.PathDirection.GetRotation();
        SetModelPosition(new Vector3(pathOffset, 0f,0f));
        progressFactor = 1f;
    }

    void PrepareTurnRight()
    {
        Vector3 right = -transform.right * 0.5f;
        SetPosition(startPosition + right);
        SetModelPosition(new Vector3(pathOffset + 0.5f,0,0));
        progressFactor = 1f / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnLeft()
    {
        Vector3 left = transform.right * 0.5f;
        SetPosition(startPosition + left);
        SetModelPosition(new Vector3(pathOffset - 0.5f,0,0));
        progressFactor = 1f / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    void PrepareTurnAround()
    {
        SetPosition(endPosition);
        SetModelPosition(new Vector3(pathOffset, 0f, 0));
        progressFactor = 1f / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }
}

/*
public class Enemy : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;

    private Healthbar healthbar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthbar = transform.Find("HealthBar").gameObject.GetComponent<Healthbar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ammo"))
        {
            float dmg = collision.gameObject.GetComponent<Ammo>().damage;
            currentHealth -= dmg;
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
            Destroy(collision.gameObject);
        }
    }
}
*/