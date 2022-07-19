using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PointControl : MonoBehaviour
{
    [SerializeField] private DataStore dataStore;

    [SerializeField] private List<GameObject> Points;
    [SerializeField] private GameObject PointPrefab;
    [SerializeField] private GameObject Map;

    [SerializeField] private GameObject CurrentPoint;

    [SerializeField] private Vector2 clickPosition = new Vector2(361, 361);

    private void Start()
    {
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
        dataStore = FindObjectOfType<DataStore>();
    }

    void UpdateValue_For_DataStore()
    {
        if(dataStore.CurrentWay.positionWayPoints.Count > 0)
        {
            PlaseWayPoints(dataStore.CurrentWay.positionWayPoints);
        }

    }

    void PlaseWayPoints(List<Vector2> pointsPositions)
    {
        foreach (Vector2 pointPos in pointsPositions)
        {
            Points.Add((Instantiate(PointPrefab, new Vector2(pointPos.x, pointPos.y + PointPrefab.gameObject.GetComponent<RectTransform>().rect.height / 2),
                Quaternion.identity).transform.parent = Map.transform).gameObject);
        }
        DrawLineWay(pointsPositions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetTouch()
    {
        Debug.Log("Click on " + Input.mousePosition);


        PointerEventData m_PointerEventData = new PointerEventData(FindObjectOfType<EventSystem>());
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        GraphicRaycaster Graycast = FindObjectOfType<GraphicRaycaster>();

        //Raycast using the Graphics Raycaster and mouse click position
        Graycast.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            if(result.gameObject.tag == "Map")
            {
                clickPosition = result.worldPosition;
                CurrentPoint = null;
                break;
            }
            else if(result.gameObject.tag == "WayPoint")
            {
                CurrentPoint = result.gameObject;
                clickPosition = new Vector2(361, 361);
                break;
            }
        }
    }

    void DrawLineWay(List<Vector2> pointsPositions)
    {
        this.gameObject.GetComponent<LineRenderer>().ResetBounds();

        List<Vector3> PosOnVect3 = new List<Vector3>();

        foreach (Vector2 item in pointsPositions)
        {
            PosOnVect3.Add(new Vector3(item.x, item.y, 0));
        }
        this.gameObject.GetComponent<LineRenderer>().positionCount = PosOnVect3.Count;
        this.gameObject.GetComponent<LineRenderer>().SetPositions(PosOnVect3.ToArray());
    }

    public void CreatePoint()
    {
        if (clickPosition != new Vector2(361, 361)) 
        { 
            Points.Add((Instantiate(PointPrefab, clickPosition, Quaternion.identity).transform.parent = Map.transform).gameObject);
            dataStore.CurrentWay.positionWayPoints.Add(clickPosition);
        }
        DrawLineWay(dataStore.CurrentWay.positionWayPoints);
        clickPosition = new Vector2(361, 361);
    }

    public void DeletePoint()
    {
        if (CurrentPoint != null)
        {
            dataStore.CurrentWay.positionWayPoints.Remove(CurrentPoint.transform.position);
            Destroy(CurrentPoint);
        }
        DrawLineWay(dataStore.CurrentWay.positionWayPoints);
        CurrentPoint = null;
    }


}
