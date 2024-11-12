using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.q0c1iru95etm")]
public class BoardSpotPlacer
{
    private readonly GameObject _circleSpot;
    private readonly Transform _parent;
    private readonly int _numberOfRings;

    public List<Vector3> ListOfSpots = new List<Vector3>();

    public BoardSpotPlacer(GameObject circleSpot, Transform parent, int numberOfRings)
    {
        this._numberOfRings = numberOfRings;
        this._circleSpot = circleSpot;
        this._parent = parent;
    }

    /// <summary>
    /// Places circle buttons on the board based on the ring configuration.
    /// </summary>
    public void PlaceCircleButtons()
    {
        for (int size = 1; size <= _numberOfRings; size++)
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
        if (_numberOfRings == 1)
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
            GameObject circleButton = Object.Instantiate(_circleSpot, position, Quaternion.identity);
            circleButton.transform.SetParent(_parent);
            ListOfSpots.Add(circleButton.transform.position);
        }
    }
}
