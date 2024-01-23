using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeDrag : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;

    // Store the original vertices for reference
    private Vector3[] originalVertices;

    // Store the corners of the box
    private Transform[] corners;

    // Flag to check if a corner is being dragged
    private bool isDragging = false;
    private int draggedCornerIndex = -1;

    void Start()
    {
        // Ensure a SpriteShapeController is assigned
        if (spriteShapeController == null)
        {
            Debug.LogError("SpriteShapeController not assigned!");
            return;
        }

        // Get the original vertices
        //originalVertices = spriteShapeController.splineDetail  Get           GetCornerPositions();

        // Create corners
        //corners = new Transform[originalVertices.Length];
        /*for (int i = 0; i < originalVertices.Length; i++)
        {
            GameObject cornerObj = new GameObject("Corner " + i);
            cornerObj.transform.position = originalVertices[i];
            corners[i] = cornerObj.transform;
        }*/
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            spriteShapeController.spline.SetPosition(0, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            spriteShapeController.BakeCollider();
            spriteShapeController.BakeMesh();
            // Check if any corner is clicked
            /*for (int i = 0; i < corners.Length; i++)
            {
                if (Vector3.Distance(,Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.2f)
                {
                    isDragging = true;
                    draggedCornerIndex = i;
                    break;
                }
            }*/
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            draggedCornerIndex = -1;
        }

        if (false)
        {
            // Drag the corner with the mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            corners[draggedCornerIndex].position = new Vector3(mousePos.x, mousePos.y, 0f);

            // Update the SpriteShapeController vertices
            Vector3[] newVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                newVertices[i] = corners[i].position;
            }

            //spriteShapeController.spline.SetCornerPositions(newVertices);
            
        }
    }
}
