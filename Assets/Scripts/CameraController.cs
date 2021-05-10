using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    int travel;
    int scrollSpeed = 3;
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;
    public float dragSpeed = 2;
    private Vector3 dragOrigin;
    void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            Vector3 NewPos = Origin - Diference;
            if (NewPos.y < 7f || NewPos.y > 32f || NewPos.x > 53f || NewPos.x < -50)
            {
                return;
            }
            Camera.main.transform.position = Origin - Diference;
        }
        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = ResetCamera;
        }
        /*
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, 0, -pos.y * dragSpeed);

        transform.Translate(move, Space.World);*/
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f && travel > -8)
        {
            travel = travel - scrollSpeed;
            //Camera.main.transform.Translate(0, 0, 1 * scrollSpeed, Space.Self);
            Camera.main.orthographicSize -= 1.5f;
        }
        else if (d < 0f && travel < 8)
        {
            travel = travel + scrollSpeed;
            //Camera.main.transform.Translate(0, 0, -1 * scrollSpeed, Space.Self);
            Camera.main.orthographicSize += 1.5f;
        }
    }
}
