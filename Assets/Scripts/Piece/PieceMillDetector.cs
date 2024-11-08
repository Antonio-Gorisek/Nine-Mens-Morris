using System.Collections.Generic;
using UnityEngine;

public class PieceMillDetector
{
    private readonly List<Vector3> _linePoints; // All possible positions on the board that could form a line
    private readonly Dictionary<Vector3, string> _ownerMap; // Maps positions to player names to track piece ownership
    private readonly int _numberOfRings;

    public PieceMillDetector(List<Vector3> linePoints, int ringsCount)
    {
        this._linePoints = linePoints;
        this._ownerMap = new Dictionary<Vector3, string>();
        this._numberOfRings = ringsCount;
    }


    /// <summary>
    /// Checks if a given position completes a mill for the current player
    /// </summary>
    public bool IsMill(Vector3 position, string currentPlayerName, HashSet<Vector3> occupiedPositions)
    {
        return CheckForMill(position, currentPlayerName, occupiedPositions, true) ||
               CheckForMill(position, currentPlayerName, occupiedPositions, false) ||
               CheckDiagonalMill(position, currentPlayerName, occupiedPositions);
    }

    /// <summary>
    /// Sets the owner of a position on the board (used for tracking piece ownership)
    /// </summary>
    public void SetOwner(Vector3 position, string playerName) 
        => _ownerMap[position] = playerName;

    /// <summary>
    /// Removes the owner of a position on the board.
    /// </summary>
    public void RemoveOwner(Vector3 position)
    {
        if (_ownerMap.ContainsKey(position))
        {
            _ownerMap.Remove(position); // Removes the entry for the specified position
        }
    }

    /// <summary>
    /// Checks for a horizontal or vertical mill centered around the specified position
    /// </summary>
    private bool CheckForMill(Vector3 position, string currentPlayerName, HashSet<Vector3> occupiedPositions, bool isHorizontal)
    {
        int count = 0; // Counter for consecutive pieces owned by the player
        Vector3 firstPoint = Vector3.zero; // Stores the starting point of a potential mill for validation

        // Sort line points based on either x (horizontal) or y (vertical) position
        _linePoints.Sort((a, b) => isHorizontal ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        foreach (Vector3 point in _linePoints)
        {
            // Check if the point aligns with the position in the selected direction (horizontal or vertical)
            if (IsPointInLine(point, position, isHorizontal))
            {
                // Verify that point is occupied by the current player
                if (occupiedPositions.Contains(point) && _ownerMap.TryGetValue(point, out string owner) && owner == currentPlayerName)
                {
                    count++;
                    if (count == 1) // Set the starting point of a potential mill
                    {
                        firstPoint = point;
                    }
                    // If three consecutive pieces align, validate the mill
                    if (count == 3 && IsMillValid(firstPoint, point))
                    {
                        return true; // A valid mill is detected
                    }
                }
                else
                {
                    count = 0; // Reset count if a break in the mill sequence is found
                }
            }
        }
        return false; // No mill detected
    }

    /// <summary>
    /// Checks for diagonal mill alignment, applicable only on single-ring boards
    /// </summary>
    private bool CheckDiagonalMill(Vector3 position, string currentPlayerName, HashSet<Vector3> occupiedPositions)
    {
        if (_numberOfRings != 1) // Diagonal mills only apply if there is one ring
            return false;

        int count = 0;
        Vector3 firstPoint = Vector3.zero;

        foreach (Vector3 point in _linePoints)
        {
            // Check if the point lies on a diagonal line from the selected position
            if (IsPointOnDiagonal(point, position))
            {
                // Verify that the point is occupied by the current player
                if (occupiedPositions.Contains(point) && _ownerMap.TryGetValue(point, out string owner) && owner == currentPlayerName)
                {
                    count++;
                    if (count == 1)
                    {
                        firstPoint = point; // Set the starting point of the potential mill
                    }
                    // Validate mill after three consecutive pieces align diagonally
                    if (count == 3 && IsMillValid(firstPoint, point))
                    {
                        return true; // Diagonal mill detected
                    }
                }
                else
                {
                    count = 0; // Reset count if alignment is broken
                }
            }
        }
        return false; // No diagonal mill detected
    }

    /// <summary>
    /// Determines if a point aligns horizontally or vertically with another point
    /// </summary>
    private bool IsPointInLine(Vector3 point, Vector3 position, bool isHorizontal)
    {
        return isHorizontal ? Mathf.Approximately(point.y, position.y) : Mathf.Approximately(point.x, position.x);
    }


    /// <summary>
    /// Determines if a point lies on the same diagonal as another point
    /// </summary>
    private bool IsPointOnDiagonal(Vector3 point, Vector3 position)
    {
        return Mathf.Approximately(Mathf.Abs(point.x - position.x), Mathf.Abs(point.y - position.y));
    }

    /// <summary>
    /// Checks the validity of a mill by ensuring pieces are in a straight line without central overlaps
    /// </summary>
    private bool IsMillValid(Vector3 start, Vector3 end)
    {
        // It skips the check for the game with 1 ring because in that game
        // it's possible to form a mill from any direction
        if (_numberOfRings == 1)
            return true;

        // Ensures a valid line (either all x or y coordinates align and no overlap with the center)
        if ((start.x != 0 && end.x != 0 && start.y == end.y && start.z == end.z) ||
            (start.y != 0 && end.y != 0 && start.x == end.x && start.z == end.z))
        {
            // Ensures no central overlap (applies to boards with a distinct center point)
            if (Vector3.Distance(start, Vector3.zero) + Vector3.Distance(Vector3.zero, end) == Vector3.Distance(start, end))
            {
                return false; // Invalid mill due to overlap with the center point
            }
        }
        return true; // Valid mill
    }
}
