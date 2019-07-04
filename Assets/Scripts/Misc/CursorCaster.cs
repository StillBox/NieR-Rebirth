using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCaster : MonoBehaviour
{
    [SerializeField] private GameCursor cursorPrefab;
    [SerializeField] private GameObject detectorPrefab;

    private Camera thisCamera;
    private GameCursor cursor;
    private GameObject cursorDetector;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        cursorDetector = Instantiate(detectorPrefab);
    }

    void Update()
    {
        if (GameCursor.isActivated)
        {
            if (PauseMenu.isPaused ||
                STBInput.GetAxis("HorizontalRotate") != 0 || STBInput.GetAxis("VerticalRotate") != 0)
            {
                Destroy(cursor.gameObject);
            }
        }
        else
        {
            if (!PauseMenu.isPaused &&
                (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0))
            {
                if (cursor == null)
                    cursor = Instantiate(cursorPrefab);
            }
        }

        if (GameCursor.isActivated)
        {
            Ray ray = thisCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("CursorDetector")))
            {
                cursor.transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(0))
            {
                cursor.SetFocus(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                cursor.SetFocus(false);
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 offset = GetComponent<TrackedCamera>().Offset;
        cursorDetector.transform.position = transform.position - offset;
    }
}