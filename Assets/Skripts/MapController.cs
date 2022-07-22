using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��������� �������� ��� ������
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// ��������� �������� ���������
    /// </summary>
    [SerializeField] private DataStore dataStore;

    private void Awake()
    {
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
        dataStore = FindObjectOfType<DataStore>();
    }

    void UpdateValue_For_DataStore()
    {
        SetImage();
    }

    /// <summary>
    /// ������������� �������� ����� �� �������� ���������
    /// </summary>
    void SetImage()
    {
        this.GetComponent<Image>().sprite = dataStore.CurrentMap.image_Map;
    }
}
