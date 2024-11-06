using System.Linq;
using UnityEngine;

public class BoardDrawer
{
    private readonly LineRenderer lineRendererPrefab;
    private readonly Transform parent;
    private readonly int numberOfRings;

    public BoardDrawer(LineRenderer lineRendererPrefab, Transform parent, int numberOfRings)
    {
        this.lineRendererPrefab = lineRendererPrefab;
        this.parent = parent;
        this.numberOfRings = numberOfRings;
    }

    /// <summary>
    /// Draws the board by creating rings and diagonals if it is a single-ring game.
    /// </summary>
    public void DrawBoard()
    {
        for (int size = 0; size <= numberOfRings; size++)
        {
            Vector2[] squarePoints = new Vector2[]
            {
                new (-size, size), // Top-left corner
                new (size, size),  // Top-right corner
                new (size, -size), // Bottom-right corner
                new (-size, -size), // Bottom-left corner
                new (-size, size)   // Close the square (from Bottom-left to Top-Left)
            };

            // Draw the square (ring) using the calculated points
            DrawLine(squarePoints);

            // If there is only one ring, draw the diagonal lines
            if (numberOfRings == 1)
            {
                DrawMiddleAndDiagonalLines();
            }
        }
        DrawMiddleLines(); // Draw the middle lines for the board
    }

    /// <summary>
    /// Draws all middle and diagonal lines for the board.
    /// </summary>
    private void DrawMiddleAndDiagonalLines()
    {
        float size = numberOfRings;
        // Define all the lines to be drawn from the center
        Vector2[][] lines = new Vector2[][]
        {
            // Middle lines (horizontal and vertical)
            new Vector2[] { new (-size, 0), new (size, 0) }, // Horizontal
            new Vector2[] { new (0, -size), new (0, size) },  // Vertical

            // Diagonal lines
            new Vector2[] { new (0, 0), new (size, size) },  // Top-right diagonal
            new Vector2[] { new (0, 0), new (-size, size) }, // Top-left diagonal
            new Vector2[] { new (0, 0), new (size, -size) }, // Bottom-right diagonal
            new Vector2[] { new (0, 0), new (-size, -size) }  // Bottom-left diagonal
        };

        // Draw each line
        foreach (var line in lines)
        {
            DrawLine(line);
        }
    }

    /// <summary>
    /// Draws the middle lines of the board.
    /// </summary>
    private void DrawMiddleLines()
    {
        float size = numberOfRings;
        // Define the positions for the middle lines
        Vector2[][] middleLines = new Vector2[][]
        {
            new Vector2[] { new (-size, 0), new (-1, 0) },
            new Vector2[] { new (size, 0), new (1, 0) },
            new Vector2[] { new (0, size), new (0, 1)  },
            new Vector2[] { new (0, -size), new (0, -1)  }
        };

        // Draw each middle line
        foreach (var line in middleLines)
        {
            DrawLine(line);
        }
    }

    /// <summary>
    /// Instantiates a line renderer and sets its position based on provided points.
    /// </summary>
    private void DrawLine(Vector2[] positions)
    {
        LineRenderer line = Object.Instantiate(lineRendererPrefab, parent); // Create a new line renderer
        line.positionCount = positions.Length; // Set the number of positions for the line

        // Convert Vector2[] to Vector3[] using LINQ
        Vector3[] positions3D = positions.Select(p => new Vector3(p.x, p.y, 0)).ToArray();

        line.SetPositions(positions3D); // Assign positions to the line renderer
    }
}