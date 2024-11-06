using System.Collections.Generic;
using UnityEngine;

public class BoardSpotPlacer
{
    private readonly GameObject circleSpot;
    private readonly Transform parent;
    private readonly int numberOfRings;

    public List<Vector3> ListOfSpots = new List<Vector3>();

    public BoardSpotPlacer(GameObject circleSpot, Transform parent, int numberOfRings)
    {
        this.numberOfRings = numberOfRings;
        this.circleSpot = circleSpot;
        this.parent = parent;
    }

    /// <summary>
    /// Places circle buttons on the board based on the ring configuration.
    /// </summary>
    public void PlaceCircleButtons()
    {
        for (int size = 1; size <= numberOfRings; size++)
        {
            // Place buttons at the corners of the current ring
            PlaceButtons(new Vector3[]
            {
                new Vector3(-size, size, 0),
                new Vector3(size, size, 0),
                new Vector3(size, -size, 0),
                new Vector3(-size, -size, 0)
            });

            // Place buttons at the midpoints of the current ring
            PlaceButtons(new Vector3[]
            {
                new Vector3(-size, 0, 0),
                new Vector3(size, 0, 0),
                new Vector3(0, size, 0),
                new Vector3(0, -size, 0)
            });
        }

        // If there is only one ring, add the center point (0, 0, 0)
        if (numberOfRings == 1)
        {
            PlaceButtons(new Vector3[] { Vector3.zero });
        }
    }


    /// <summary>
    /// Instantiates circle buttons at specified positions.
    /// </summary>
    private void PlaceButtons(Vector3[] positions)
    {
        foreach (var position in positions)
        {
            GameObject circleButton = Object.Instantiate(circleSpot, position, Quaternion.identity);
            circleButton.transform.SetParent(parent);
            ListOfSpots.Add(circleButton.transform.position);
        }
    }
}
