using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Vector2 top_left = new Vector2();
    [SerializeField] private Vector2 bottom_right = new Vector2();
                     
    [SerializeField] private GameObject MapNameText;
                   
    [SerializeField] private GameObject WayNameText;
    [SerializeField] private GameObject WayPointCountText;

    [SerializeField] private GameObject Left_Edit;
    [SerializeField] private GameObject Top_Edit;
    [SerializeField] private GameObject Right_Edit;
    [SerializeField] private GameObject Bottom_Edit;

    DataStore dataStore;

    private void Start()
    {
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
        dataStore = FindObjectOfType<DataStore>();
    }

    private void UpdateValue_For_DataStore()
    {
        SetMapInfo(dataStore.CurrentMap.name_map, dataStore.CurrentMap.RectMap);
        SetWayInfo(dataStore.CurrentWay.name_WAY, dataStore.CurrentWay.positionWayPoints.Count);
    }

    public void SetWayInfo(string name, int count)
    {
        WayNameText.GetComponent<Text>().text = name;
        WayPointCountText.GetComponent<Text>().text = count.ToString();
    }

    public void SetMapInfo(string name, Rect graf_point)
    {
        SetGrafValue(graf_point);
        MapNameText.GetComponent<Text>().text = name;
    }

    public Rect GetGrafValue()
    {
        Rect rect = new Rect();
        rect.x = float.Parse(Left_Edit.GetComponent<InputField>().text);
        rect.y = float.Parse(Top_Edit.GetComponent<InputField>().text);
        rect.width = float.Parse(Right_Edit.GetComponent<InputField>().text);
        rect.height = float.Parse(Bottom_Edit.GetComponent<InputField>().text);
        return rect;
    }

    void SetGrafValue(float left, float top, float right, float bottom)
    {
        Left_Edit.GetComponent<InputField>().text     = left.ToString();
        Top_Edit.GetComponent<InputField>().text      = top.ToString();
        Right_Edit.GetComponent<InputField>().text    = right.ToString();
        Bottom_Edit.GetComponent<InputField>().text   = bottom.ToString();
    }

    void SetGrafValue(Rect rect)
    {
        Left_Edit.GetComponent<InputField>().text = rect.left.ToString();
        Top_Edit.GetComponent<InputField>().text = rect.top.ToString();
        Right_Edit.GetComponent<InputField>().text = rect.right.ToString();
        Bottom_Edit.GetComponent<InputField>().text = rect.bottom.ToString();
    }

    public void ChangeImmage()
    {

    }

    public void SaveMap()
    {

    }

    public void SaveWay()
    {

    }

}
