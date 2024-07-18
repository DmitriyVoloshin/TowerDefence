using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Linq;

public class Path : MonoBehaviour
{
    public PathCreator pathCreator;
    public BezierPath path;

    public List<Vector2> pathPoints;

    void Start()
    {
        pathPoints.Add(new Vector2(-9, -9));
        pathPoints.Add(new Vector2(-9, -8.5f));
        pathPoints.Add(new Vector2(-8.5f, -8.0f));
        pathPoints.Add(new Vector2(-8, -8));
        pathPoints.Add(new Vector2(-7, -8));
        pathPoints.Add(new Vector2(-6, -8));
        pathPoints.Add(new Vector2(-5, -8));
        pathPoints.Add(new Vector2(-5, -7));
        pathPoints.Add(new Vector2(-5, -6));
        pathPoints.Add(new Vector2(-5, -5));
        pathPoints.Add(new Vector2(-5, -4));

        path = new BezierPath(pathPoints.ToArray(), false, PathSpace.xy);

        pathCreator = GetComponent<PathCreator>();
        pathCreator.bezierPath = path;

    }

    void Update()
    {
    }


    VertexPath GeneratePath(Vector2[] points, bool closedPath)
    {
        BezierPath bezierPath = new BezierPath(points, closedPath, PathSpace.xy);

        return new VertexPath(bezierPath, transform, 2.0f, 0.1f);
    }
}
