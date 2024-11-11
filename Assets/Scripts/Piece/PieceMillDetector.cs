using System.Collections.Generic;
using UnityEngine;

public class PieceMillDetector
{
    // Dictionary that maps a position on the board to the name of the player who owns a piece there
    private readonly Dictionary<Vector3, string> _ownerMap;

    // An integer representing the number of rings on the board
    private readonly int _ringsCount;

    private readonly List<List<Vector3>> _horizontalMills;
    private readonly List<List<Vector3>> _verticalMills;
    private readonly List<List<Vector3>> _diagonalMills;


    public PieceMillDetector(List<Vector3> linePoints, int ringsCount)
    {
        this._ownerMap = new Dictionary<Vector3, string>();
        this._ringsCount = ringsCount;
        this._horizontalMills = GenerateHorizontalMills(); // Generates all possible horizontal mills
        this._verticalMills = GenerateVerticalMills(); // Generates all possible vertical mills
        this._diagonalMills = GenerateDiagonalMills(); // Generates all possible diagonal mills
    }

    /// <summary>
    /// Checks if the given position completes a mill for the current player
    /// </summary>
    public bool IsMill(Vector3 position, string currentPlayerName)
    {
        return CheckMill(position, currentPlayerName, _horizontalMills)
            || CheckMill(position, currentPlayerName, _verticalMills)
            || CheckMill(position, currentPlayerName, _diagonalMills)
            || CheckHorizontalMill(position, currentPlayerName)
            || CheckVerticalMill(position, currentPlayerName);
    }

    /// <summary>
    /// Sets the owner of a specific board position (used to track piece ownership)
    /// </summary>
    public void SetOwner(Vector3 position, string playerName)
        => _ownerMap[position] = playerName;

    /// <summary>
    /// Removes the owner from a specific position on the board.
    /// </summary>
    public void RemoveOwner(Vector3 position)
    {
        if (_ownerMap.ContainsKey(position))
        {
            _ownerMap.Remove(position);
        }
    }

    private List<List<Vector3>> GenerateHorizontalMills()
    {
        List<List<Vector3>> mills = new List<List<Vector3>>();
        for (int i = 1; i <= _ringsCount; i++) // Loops through each ring
        {
            mills.Add(new List<Vector3> { new Vector3(i, 0, 0), new Vector3(0, 0, 0), new Vector3(-i, 0, 0) });
            mills.Add(new List<Vector3> { new Vector3(i, i, 0), new Vector3(i, 0, 0), new Vector3(i, -i, 0) });
            mills.Add(new List<Vector3> { new Vector3(-i, i, 0), new Vector3(-i, 0, 0), new Vector3(-i, -i, 0) });
        }
        return mills;
    }

    private List<List<Vector3>> GenerateVerticalMills()
    {
        List<List<Vector3>> mills = new List<List<Vector3>>();
        for (int i = 1; i <= _ringsCount; i++) // Loops through each ring
        {
            mills.Add(new List<Vector3> { new Vector3(0, i, 0), new Vector3(0, 0, 0), new Vector3(0, -i, 0) });
            mills.Add(new List<Vector3> { new Vector3(i, i, 0), new Vector3(0, i, 0), new Vector3(-i, i, 0) });
            mills.Add(new List<Vector3> { new Vector3(i, -i, 0), new Vector3(0, -i, 0), new Vector3(-i, -i, 0) });
        }
        return mills;
    }

    private List<List<Vector3>> GenerateDiagonalMills()
    {
        List<List<Vector3>> mills = new List<List<Vector3>>();
        for (int i = 1; i <= _ringsCount; i++) // Loops through each ring
        {
            mills.Add(new List<Vector3> { new Vector3(i, i, 0), new Vector3(0, 0, 0), new Vector3(-i, -i, 0) });
            mills.Add(new List<Vector3> { new Vector3(i, -i, 0), new Vector3(0, 0, 0), new Vector3(-i, i, 0) });
        }
        return mills;
    }

    private bool CheckMill(Vector3 position, string player, List<List<Vector3>> mills)
    {
        foreach (var mill in mills) // Loops through each mill (a list of 3 positions forming a line)
        {
            if (mill.Contains(position)) // Checks if the specified position is in the current mill
            {
                bool isMill = true; // Assumes it's a mill unless proven otherwise
                foreach (var pos in mill) // Checks each position in the mill
                {
                    if (!_ownerMap.ContainsKey(pos) || _ownerMap[pos] != player) // If position is not owned by player
                    {
                        isMill = false; // It's not a mill
                        break; // Exits the loop early as soon as one piece doesn't match
                    }
                }
                if (isMill) // If all pieces in the mill belong to the player
                {
                    return true; // Returns true indicating a mill is complete
                }
            }
        }
        return false; // Returns false if no mill is found
    }


    /// <summary>
    /// This method checks for horizontal mills originating from the center of the board (0, 0, 0).
    /// Since in this direction, there can theoretically be an infinite number of points,
    /// detecting a mill using the standard CheckMill logic could create problems.
    /// Therefore, a separate method is used to check for mills in these specific areas.
    /// </summary>
    private bool CheckHorizontalMill(Vector3 position, string player)
    {
        for (int i = -2; i <= 0; i++) // Loops through a range to check for possible mills near the position
        {
            bool isMill = true;
            int count = 0; // Counts pieces in the line
            for (int j = 0; j < 3; j++) // Checks three positions in the line
            {
                Vector3 checkPosition = new Vector3(position.x + i + j, position.y, position.z); // Calculates position in line
                if (_ownerMap.ContainsKey(checkPosition)) // Checks if the position has a piece
                {
                    count++; // Increments count of pieces in the line
                    if (_ownerMap[checkPosition] != player) // Checks if piece is owned by the specified player
                    {
                        isMill = false; // If not, this line isn't a mill
                        break;
                    }
                }
            }
            if (isMill && count == 3) // If all 3 pieces match the player
            {
                if (CountFiguresInLine(position, player, true) > 3) // Ensures line has no extra pieces
                {
                    return false;
                }
                return true; // A mill is confirmed
            }
        }
        return false; // No mill found
    }

    /// <summary>
    /// This method checks for vertical mills originating from the center of the board (0, 0, 0).
    /// Similar to horizontal mills, in this direction there can theoretically be an infinite number of points,
    /// making mill detection problematic using the standard CheckMill logic.
    /// This is why a separate method is used for mill detection in these areas.
    /// </summary>
    private bool CheckVerticalMill(Vector3 position, string player)
    {
        for (int i = -2; i <= 0; i++) // Loops through possible vertical lines around position
        {
            bool isMill = true; // Assumes mill unless proven otherwise
            int count = 0; // Counts the pieces in this line
            for (int j = 0; j < 3; j++) // Checks three positions
            {
                Vector3 checkPosition = new Vector3(position.x, position.y + i + j, position.z); // Vertical line calculation
                if (_ownerMap.ContainsKey(checkPosition)) // Checks if a piece exists
                {
                    count++; // Increments count
                    if (_ownerMap[checkPosition] != player) // Verifies ownership
                    {
                        isMill = false; // No mill
                        break;
                    }
                }
            }
            if (isMill && count == 3) // Confirms mill if all conditions are met
            {
                if (CountFiguresInLine(position, player, false) > 3) // Avoids excess pieces in line
                {
                    return false;
                }
                return true; // Mill confirmed
            }
        }
        return false; // No mill
    }

    /// <summary>
    /// This method counts consecutive pieces in a line (horizontal or vertical) for a given player. 
    /// It is used for mill detection, as it checks if the player has placed more than three consecutive pieces, 
    /// which invalidates the mill. The method goes through the line, counts the player's pieces, and if the count exceeds three, 
    /// the line is considered invalid for a mill.
    /// </summary>
    public int CountFiguresInLine(Vector3 position, string player, bool isHorizontal)
    {
        int count = 0;
        int range = _ringsCount;

        // Loop through the range based on the direction (horizontal or vertical)
        for (int i = -range; i <= range; i++)
        {
            Vector3 checkPosition = isHorizontal ? new Vector3(position.x + i, position.y, position.z) : new Vector3(position.x, position.y + i, position.z);

            if (_ownerMap.ContainsKey(checkPosition) && _ownerMap[checkPosition] == player)
            {
                count++;
            }
            else
            {
                count = 0; // Reset if no piece or if not owned by the player
            }

            if (count > 3) return count; // Return early if more than 3 pieces found
        }

        return count;
    }
}
