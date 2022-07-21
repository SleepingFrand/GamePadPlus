using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStore : MonoBehaviour
{
    public MAP CurrentMap;
    public WAY CurrentWay;

    public List<Shotcat> shotcats;

    private DataMagedger _dataManedger;

    private void Start()
    {
        _dataManedger = this.gameObject.GetComponent<DataMagedger>();

        if (!_dataManedger.TryLoadMap(_dataManedger.pathsMap[0], out CurrentMap))
        {
            Debug.Log("Error on load current map by id 0");
        }

        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, CurrentMap.names_WAY[0], out CurrentWay))
        {
            Debug.Log("Error on load current way by id 0");
        }

        FindObjectOfType<MapController>().UpdateNow = true;

        shotcats = _dataManedger.GetShortName();

       AtionsSystem.UpdateValueForDataStore.Invoke();
    }

    public void ChengeMap(string name)
    {
        if (!_dataManedger.TryLoadMap(name, out CurrentMap))
        {
            Debug.Log("Error on load current map by id 0");
        }

        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, CurrentMap.names_WAY[0], out CurrentWay))
        {
            Debug.Log("Error on load current way by id 0");
        }

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }

    public void ChengeWay(string name)
    {
        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, name, out CurrentWay))
        {
            Debug.Log("Error on load current way by id 0");
        }

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }

    public void SaveMap()
    {
        _dataManedger.SaveMap(CurrentMap);
    }

    public void SaveMap(MAP other)
    {
        _dataManedger.SaveMap(other);
    }
    public void SaveWay()
    {
        _dataManedger.SaveWay(CurrentMap, CurrentWay);
    }


    public void CreateMap()
    {
        MAP newMap = new MAP();

        if(!_dataManedger.CreateMap(out newMap))
        {
            return;
        }

        shotcats = _dataManedger.GetShortName();

        ChengeMap(newMap.name_map);

    }

    public void CreateWay()
    {

        WAY newWay = new WAY();

        newWay.positionWayPoints = new List<Vector2>();
        newWay.name_WAY = "NewWay_" + CurrentMap.names_WAY.Count.ToString();

        _dataManedger.SaveWay(CurrentMap, newWay);

        shotcats = _dataManedger.GetShortName();

        ChengeWay(newWay.name_WAY);
    }

    public void ChangeNameMap( string name)
    {
        if (name == "" || name == CurrentMap.name_map)
            return;

        _dataManedger.ChangeName(CurrentMap, name);

        shotcats = _dataManedger.GetShortName();

        ChengeMap(name);
    }

    public void ChangeNameWay(string name)
    {
        if (name == "" || name == CurrentWay.name_WAY)
            return;

        _dataManedger.ChangeName(CurrentMap, name, CurrentWay);

        shotcats = _dataManedger.GetShortName();

        ChengeWay(name);
    }


    public void DeleteCurrentMap()
    {
        if (_dataManedger.TryRemoveMap(CurrentMap.name_map))
        {
            ChengeMap(_dataManedger.pathsMap[0]);
        }

        shotcats = _dataManedger.GetShortName();

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }

    public void DeleteCurrentWay()
    {
        if (_dataManedger.TryRemoveWay(CurrentMap.name_map, CurrentWay.name_WAY))
        {
            CurrentMap.names_WAY.Remove(CurrentWay.name_WAY);
            SaveMap();
            ChengeWay(CurrentMap.names_WAY[0]);
        }

        shotcats = _dataManedger.GetShortName();

        AtionsSystem.UpdateValueForDataStore.Invoke();
        AtionsSystem.UpdateValueOnSettings();
    }
}