using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Waypoint : MonoBehaviour
{
    [SerializeField]
    GameObject point = default;

    Vector2 coordinate;
    bool Usable { get; set; }
}

public class PathFinder : MonoBehaviour
{
    [SerializeField, Range(0.1f, 0.5f)]
    float step = 0.25f;

    [SerializeField]
    GameBoard board = default;


    int maxX;
    int maxY;
    Waypoint[] waypoints;


    public void Initialize(int sizeX, int sizeY)
    {
        maxX = (int)(sizeX / step) + 1;
        maxY = (int)(sizeY / step) + 1;

        waypoints = new Waypoint[maxX * maxY];



    }
}
