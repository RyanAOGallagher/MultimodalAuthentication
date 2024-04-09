using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject cursorPrefab; // Reference to your cursor prefab
    private GameObject cursorInstance; // Instance of the cursor
    public NetworkManager nm;
    float cursor_offset_x;
    float cursor_offset_y;

    void Start()
    {
        // Instantiate the cursor and store the reference to the instance
        cursorInstance = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity);
    }

    void Update()
    {
        // Update the cursor position each frame
        UpdateCursorPosition();
    }

void UpdateCursorPosition()
{
    string[] cursorPos = nm.get_points();

    if (cursorPos != null && (cursorPos[0] == "p" || cursorPos[0] == "np"))
    {
        // Normalize cursor position to 0 to 1
        float normalizedX = float.Parse(cursorPos[1]) / 450.0f;
        float normalizedY = float.Parse(cursorPos[2]) / 450.0f;

        // Scale and offset to fit into the world space range (-11, 11)
        // Since the range is -11 to 11, the total range is 22 units.
        // We subtract by 1 to shift the range from (0, 22) to (-11, 11).
        float worldX = (normalizedX * 22f) - 11f;
        float worldY = ((1 - normalizedY) * 22f) - 11f; // Y is inverted

        cursorInstance.transform.localPosition = new Vector3(worldX, worldY, 85);
    }
}


}
