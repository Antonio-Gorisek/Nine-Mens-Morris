using UnityEngine;

public static class CameraController
{
    /// <summary>
    /// Adjusts the main camera's orthographic size to fit the board.
    /// </summary>
    public static void AdjustCamera(Camera mainCamera, int numberOfRings)
    {
        if (mainCamera == null) return;
        // Set the orthographic size of the camera based on the number of rings and spacing
        mainCamera.orthographicSize = Mathf.Max(5, numberOfRings * 1.7f);
    }
}