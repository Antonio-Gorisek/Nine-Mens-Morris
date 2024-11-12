using System.Collections.Generic;
using UnityEngine;

public class PieceNeighbors
{
    private readonly Dictionary<Vector3, string> _ownerMap;
    private readonly List<Vector3> _positionOfSpots;
    private readonly int _ringsCount;

    public PieceNeighbors(Dictionary<Vector3, string> ownerMap, List<Vector3> positionOfSpots, int ringsCount)
    {
        _ownerMap = ownerMap;
        _positionOfSpots = positionOfSpots;
        _ringsCount = ringsCount;
    }

    /// <summary>
    /// Determines the valid moves a piece can make from a given position. 
    /// It checks all neighbors positions in each direction (up, down, left, right).
    /// Returns a list of Vector3 positions representing empty spots where the piece can move.
    /// </summary>
    public List<Vector3> GetAvailableMoves(Vector3 position)
    {
        List<Vector3> availableMoves = new List<Vector3>();

        // Checking neighbors in each direction
        foreach (var direction in new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down })
        {
            for (int i = 1; i <= _ringsCount; i++)
            {
                Vector3 neighbor = position + direction * i;

                // Check if it crosses the central coordinate (0, 0, 0)
                if ((position.x == 0 && neighbor.x == 0 && position.y * neighbor.y < 0) ||
                    (position.y == 0 && neighbor.y == 0 && position.x * neighbor.x < 0))
                {
                    break; // Breaks the loop if crossing the central coordinate
                }

                if (_positionOfSpots.Contains(neighbor) && !_ownerMap.ContainsKey(neighbor))
                {
                    availableMoves.Add(neighbor);
                    break; // Stops the loop after finding the first free spot in this direction
                }
                else if (_ownerMap.ContainsKey(neighbor))
                {
                    break; // Stops the loop if the spot is occupied
                }
            }
        }

        // Add diagonal neighbors exclusively towards the center from corners and from the center to corners if ring count is 1
        if (_ringsCount == 1)
        {
            foreach (var direction in new Vector3[] { new Vector3(1, 1, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0), new Vector3(-1, 1, 0) })
            {
                Vector3 neighbor = position + direction;

                // Check if the diagonal neighbor is exclusively towards the center from corners and from the center to corners
                if ((position.x == 0 || position.y == 0) || (neighbor.x == 0 || neighbor.y == 0))
                {
                    if (_positionOfSpots.Contains(neighbor) && !_ownerMap.ContainsKey(neighbor))
                    {
                        availableMoves.Add(neighbor);
                    }
                }
            }
        }

        return availableMoves;
    }





    /// <summary>
    /// Checks if all of the player's pieces on the board are blocked from moving.
    /// </summary>
    public bool AreAllPiecesBlocked(string player)
    {
        // Loops through each piece on the board to find pieces belonging to the given player.
        foreach (var kvp in _ownerMap)
        {
            // If the piece belongs to the specified player.
            if (kvp.Value == player)
            {
                // Checks if the piece has any available moves. If it can move, returns false.
                if (GetAvailableMoves(kvp.Key).Count > 0)
                {
                    return false; // Not all pieces are blocked if this piece can move.
                }
            }
        }
        return true; // Returns true if no pieces belonging to the player can move.
    }
}
