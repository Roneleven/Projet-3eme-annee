using UnityEngine;

public class BlinkAim : MonoBehaviour
{
    public Material material;
    public Color[] colors;
    private int currentColorIndex = 0;
    private int targetColorIndex = 1;
    private float targetPoint;
    public float time;

    // Reference to the Weapon script
    public Weapon weaponScript;

    // Update is called once per frame
    void Update()
    {
        Transition();
    }

    void Transition()
    {
        // Access the overheated variable from the Weapon script
        bool isOverheated = weaponScript.overheated;

        // Do something based on the overheated status
        if (isOverheated)
        {
            // Perform transition only when overheated
            targetPoint += Time.deltaTime / time;
            material.color = Color.Lerp(colors[currentColorIndex], colors[targetColorIndex], targetPoint);
            if (targetPoint >= 1f)
            {
                targetPoint = 0f;
                currentColorIndex = targetColorIndex;
                targetColorIndex++;
                if (targetColorIndex == colors.Length)
                    targetColorIndex = 0;
            }
        }
        else
        {
            // Reset color index to 0 when not overheated
            currentColorIndex = 0;
            material.color = colors[currentColorIndex]; // Set color immediately
        }
    }
}