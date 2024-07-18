using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    North, East, South, West
}
public enum DirectionChange
{
    None, TurnRight, TurnLeft, TurnAround
}

public static class DirectionExtensions
{
    static Vector3[] halfVectors = {
        Vector3.up,
        Vector3.right,
        Vector3.down,
        Vector3.left
    };

    static Quaternion[] rotations = {
        //Quaternion.identity,
        Quaternion.Euler(0f, 0f, 180f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(0f, 0f, 0f),
        Quaternion.Euler(0f, 0f, 270f)
    };
    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction] * 0.5f;
    }
    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }
    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }
}


public class GameTile : MonoBehaviour
{
    [SerializeField]
    GameObject arrow = default;
    [SerializeField]
    GameTile north, east, south, west;
    [SerializeField]
    GameTile nextOnPath;
    int distance;

    
    public Direction PathDirection { get; private set; }

    public Vector3 ExitPoint { get; private set; }

    GameTileContent content;

    public GameTile NextTileOnPath => nextOnPath;

    public GameTileContent Content
    {
        get => content;
        set 
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    static Quaternion
    northRotation = Quaternion.Euler(0f, 0f, 0f),
    eastRotation = Quaternion.Euler(0f, 0f, 270f),
    southRotation = Quaternion.Euler(0f, 0f, 180f),
    westRotation = Quaternion.Euler(0f, 0f, 90f);

    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "Redefined neighbors");
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(
            south.north == null && north.south == null, "Redefined neighbors!"
        );
        south.north = north;
        north.south = south;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public void BecomeWall()
    {
        arrow.SetActive(false);
    }

    public bool HasPath => distance != int.MaxValue;
       
    GameTile GrowPathTo(GameTile neighbour, Direction direction)
    {
        if (!HasPath || neighbour == null || neighbour.HasPath)
        {
            return null;
        }
        neighbour.distance = distance + 1;
        neighbour.nextOnPath = this;
        neighbour.PathDirection = direction;
        neighbour.ExitPoint = (neighbour.transform.localPosition + transform.localPosition) * 0.5f;

        if (neighbour.Content.BlocksPath)
        //if (neighbour.Content.Type == GameTileContentType.Wall)
        {
            return null;
        }
        return neighbour;
    }

    public void HidePath()
    {
        arrow.SetActive(false);
    }

    public void ShowPath()
    {
        if (content.Type == GameTileContentType.Empty)
        {
            arrow.SetActive(true);
        }
        else arrow.SetActive(false);

        arrow.transform.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    public bool IsAlternative { get; set; }

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

}
