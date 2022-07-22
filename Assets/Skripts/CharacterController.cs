using UnityEngine;

/// <summary>
/// Реализует управление персонажем
/// </summary>
public class CharacterController : MonoBehaviour
{
    #region Поля
    /// <summary>
    /// Карта (Игровой объект)
    /// </summary>
    [SerializeField] private GameObject Map;
    /// <summary>
    /// Нарвавление пути
    /// </summary>
    [SerializeField] private float Direction;
    /// <summary>
    /// Текущаа позиция в географических координатах
    /// </summary>
    [SerializeField] private Vector2 GeoPos;
    #endregion

    #region Приватные поля
    private DataStore dataStore;
    #endregion

    #region Методы
    private void Start()
    {
        dataStore = FindObjectOfType<DataStore>();
    }

    /// <summary>
    /// Установить позицию персонажа в географических координатах
    /// </summary>
    /// <param name="position">Географическая координата</param>
    /// <param name="direction">Напраление пути</param>
    public void SetPosition(Vector2 position, float direction)
    {
        GeoPos = position;
        Direction = direction;
        transform.Translate(SetGeotransformToScreen());
    }

    /// <summary>
    /// Перевод географических корординат в корординаты на экране
    /// </summary>
    Vector2 SetGeotransformToScreen()
    {
        Rect map_rect = dataStore.CurrentMap.RectMap;

        Vector2 Size = new Vector2();

        Vector2 RePosition = GeoPos;

        Vector2 outVect = new Vector2();

        Size.x = 0;
        Size.y = 0;

        if (map_rect.left < map_rect.right)
        {
            Size.x = map_rect.right - map_rect.left;
            RePosition.x -= map_rect.left;
        }
        else
        {
            Size.x = map_rect.left - map_rect.right;
            RePosition.x -= map_rect.right;
        }

        if (map_rect.top < map_rect.bottom)
        {
            Size.y = map_rect.bottom - map_rect.top;
            RePosition.y -= map_rect.top;
        }
        else
        {
            Size.y = map_rect.top - map_rect.bottom;
            RePosition.y -= map_rect.bottom;
        }

        double correct = RePosition.x / Size.x;
        outVect.x = (float)(Map.GetComponent<RectTransform>().rect.width / correct) + Map.GetComponent<RectTransform>().rect.x;

        correct = RePosition.y / Size.y;
        outVect.x = (float)(Map.GetComponent<RectTransform>().rect.height / correct) + Map.GetComponent<RectTransform>().rect.y;

        return outVect;
    }
    #endregion
}
