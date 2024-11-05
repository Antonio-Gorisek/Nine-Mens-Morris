using System.Linq;
using UnityEngine;

public class BoardDrawer
{
    private readonly LineRenderer lineRendererPrefab;
    private readonly Transform parent;
    private readonly int numberOfRings;
    private readonly float spacing;

    public BoardDrawer(LineRenderer lineRendererPrefab, Transform parent, float spacing, int numberOfRings)
    {
        this.lineRendererPrefab = lineRendererPrefab;
        this.parent = parent;
        this.spacing = spacing;
        this.numberOfRings = numberOfRings;
    }

    /// <summary>
    /// Draws the board by creating rings and diagonals if it is a single-ring game.
    /// </summary>
    public void DrawBoard()
    {
        for (int i = 0; i <= numberOfRings; i++)
        {
            float size = i * spacing;
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
               DrawDiagonals(size);
            }
        }
        DrawMiddleLines(); // Draw the middle lines for the board
    }

    /// <summary>
    /// Draws the middle lines of the board.
    /// </summary>
    private void DrawMiddleLines()
    {
        // Define the positions for the middle lines
        Vector2[][] middleLines = new Vector2[][]
        {
            new Vector2[] { new (-numberOfRings * spacing, 0), new (-spacing, 0) },
            new Vector2[] { new (numberOfRings * spacing, 0), new (spacing, 0) },
            new Vector2[] { new (0, numberOfRings * spacing), new (0, spacing)  },
            new Vector2[] { new (0, -numberOfRings * spacing), new (0, -spacing)  }
        };

        // Draw each middle line
        foreach (var line in middleLines)
        {
            DrawLine(line);
        }
    }

    /// <summary>
    /// Draws diagonal lines on the board if there is only one ring.
    /// </summary>
    private void DrawDiagonals(float size)
    {
        // Define the diagonal line endpoints
        Vector2[] diagonalLines = new Vector2[] {
            new (-size, size), new (size, -size),
            new (size, size), new (-size, -size)
        };

        // Draw each diagonal line
        for (int i = 0; i < diagonalLines.Length; i += 2) {
            DrawLine(new Vector2[] { diagonalLines[i], diagonalLines[i + 1] });
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