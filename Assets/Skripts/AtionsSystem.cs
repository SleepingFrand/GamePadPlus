using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Реализует систему событий
/// </summary>
public class AtionsSystem : MonoBehaviour
{
    /// <summary>
    /// Группа методов. вызываемых при обновлении данных в главном хранилище
    /// </summary>
    public static UnityAction UpdateValueForDataStore;

    /// <summary>
    /// Группа методов. вызываемых для обновления данных в панеле настроек
    /// </summary>
    public static UnityAction UpdateValueOnSettings;
}
