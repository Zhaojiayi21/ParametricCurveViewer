using UnityEngine;

public class ControlPointDrag : MonoBehaviour
{
    private bool isDragging;
    private Camera mainCam;
    public GameObject curveManagerObj;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.back, Vector3.zero);
            if (plane.Raycast(ray, out float dist))
            {
                transform.position = ray.GetPoint(dist);
                curveManagerObj.SendMessage("UpdateCurve");
            }
        }
    }
}
