using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public Dictionary<Vector3, string> OwnerMap { get; }
    public List<Vector3> PositionOfSpots { get; }
    public int RingsCount { get; }

    public Board(List<Vector3> positionOfSpots, int ringsCount)
    {
        OwnerMap = new Dictionary<Vector3, string>();
        PositionOfSpots = positionOfSpots;
        RingsCount = ringsCount;
    }
}