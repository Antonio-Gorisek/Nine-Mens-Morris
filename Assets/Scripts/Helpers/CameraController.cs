using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.xu1kh2dq8r7w")]
public static class CameraController
{
    /// <summary>
    /// Adjusts the main camera's orthographic size to fit the board.
    /// </summary>
    public static void AdjustCamera(Camera mainCamera, int numberOfRings)
    {
        if (mainCamera == null) return;
        // Set the orthographic size of the camera based on the number of rings and spacing
        mainCamera.orthographicSize = Mathf.Max(5, numberOfRings * 2);
    }
}