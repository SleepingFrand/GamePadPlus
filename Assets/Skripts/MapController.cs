using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] private DataStore dataStore;
    public bool UpdateNow = false;

    private void Start()
    {
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
        dataStore = FindObjectOfType<DataStore>();
    }

    void UpdateValue_For_DataStore()
    {
        SetImage();
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateNow)
        {
            SetImage();
            UpdateNow = false;
        }
    }

    void SetImage()
    {
        this.GetComponent<Image>().sprite = dataStore.CurrentMap.image_Map;
    }
}
