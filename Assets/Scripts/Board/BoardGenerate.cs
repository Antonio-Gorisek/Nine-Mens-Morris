using UnityEngine;

public class BoardGenerate
{
    private readonly LineRenderer _lineRendererPrefab;
    private readonly GameObject _circleSpot;
    private readonly GameObject _boardParentObj;
    private readonly Camera _mainCamera;

    private readonly float _ringsSpacing;
    private readonly int _numberOfRings;

    /// <summary>
    /// Constructor for the BoardGenerate class.
    /// Initializes the board with a specified rings and spacing size.
    /// </summary>
    public BoardGenerate(int rings = 3, float spacing = 1)
    {
        // if numberOfRings is below 1, sets it to 3 (default).
        _numberOfRings = rings <= 0 ? 3 : rings;
        _ringsSpacing = spacing;

        // LineRenderer and GameObject from the Resources folder for the visual elements.
        _lineRendererPrefab = Resources.Load<LineRenderer>("Game/Line");
        _circleSpot = Resources.Load<GameObject>("Game/Spot");
        _mainCamera = Camera.main;
        _boardParentObj = new GameObject("Board");

        if (_lineRendererPrefab == null) {
            Debug.LogError("LineRendererPrefab is missing. Please ensure 'Game/Line' prefab exists in Resources.");
            return;
        }

        if (_circleSpot == null) {
            Debug.LogError("CircleSpot is missing. Please ensure 'Game/Spot' prefab exists in Resources.");
            return;
        }

        // Setup the board components
        SetupBoard();
    }

    /// <summary>
    /// Configures the board by adjusting the camera, drawing the board, and placing circle buttons.
    /// </summary>
    private void SetupBoard()
    {
        // Adjusts the camera settings based on the number of rings and spacing
        // Ensures the map size always stays within the camera's frame
        CameraController.AdjustCamera(_mainCamera, _numberOfRings, _ringsSpacing);

        BoardDrawer boardDrawer = new BoardDrawer(_lineRendererPrefab, _boardParentObj.transform, _ringsSpacing, _numberOfRings);
        BoardSpotPlacer buttonPlacer = new BoardSpotPlacer(_circleSpot, _boardParentObj.transform, _numberOfRings);

        boardDrawer.DrawBoard();
        buttonPlacer.PlaceCircleButtons();
    }
}