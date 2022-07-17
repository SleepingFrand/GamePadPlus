using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointControl : MonoBehaviour
{
    [SerializeField] private DataStore dataStore;

    [SerializeField] private List<GameObject> Points;
    [SerializeField] private GameObject PointPrefab;
    [SerializeField] private GameObject Map;

    [SerializeField] private GameObject CurrentPoint;

    [SerializeField] private Vector2 clickPosition = new Vector2(361, 361);

    // Start is called before the first frame update
    void Start()
    {
        dataStore = FindObjectOfType<DataStore>();
        if(dataStore.CurrentWay.positionWayPoints.Count > 0)
        {
            PlaseWayPoints(dataStore.CurrentWay.positionWayPoints);
        }

        this.gameObject.AddComponent<LineRenderer>();
    }

    void PlaseWayPoints(List<Vector2> pointsPositions)
    {
        foreach (Vector2 pointPos in pointsPositions)
        {
            Points.Add((Instantiate(PointPrefab, pointPos, Quaternion.identity).transform.parent = Map.transform).gameObject);
        }
        DrawLineWay(pointsPositions);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, 1 << 5))
            {
                if(hit.transform.gameObject.tag == "Map")
                {
                    clickPosition = hit.transform.position;
                    CurrentPoint = null;
                }
                else if (hit.transform.gameObject.tag == "Point")
                {
                    clickPosition = new Vector2(361, 361);
                    CurrentPoint = hit.transform.gameObject;
                }
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
    }

    public void DeletePoint()
    {
        if (CurrentPoint != null)
        {
            dataStore.CurrentWay.positionWayPoints.Remove(CurrentPoint.transform.position);
            Destroy(CurrentPoint);
        }
        DrawLineWay(dataStore.CurrentWay.positionWayPoints);
    }


}
