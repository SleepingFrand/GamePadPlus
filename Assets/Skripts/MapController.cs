using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// –еализует контроль над картой
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// Ёкземпл€р главного хранилища
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
    /// ”станавливает картинку карты из главного хранилища
    /// </summary>
    void SetImage()
    {
        this.GetComponent<Image>().sprite = dataStore.CurrentMap.image_Map;
    }
}
