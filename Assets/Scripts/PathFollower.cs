using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PathFollower : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 1.0f;
    float distanceTravelled;

    void Start()
    {
        GameObject path = GameObject.FindGameObjectWithTag("Path");

        pathCreator = path.GetComponent<PathCreator>();
        if (pathCreator != null)
        {
            pathCreator.pathUpdated += OnPathChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pathCreator != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction) * Quaternion.Euler(0, 90, 90);
        }
    }
    
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}