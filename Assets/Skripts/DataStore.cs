using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс выполняющий роль главного хранилища
/// Выполняет роль хранения, манипуляции и сохранения данных
/// </summary>
public class DataStore : MonoBehaviour
{
    #region Поля
    /// <summary>
    /// Выбранная и загруженная карта
    /// </summary>
    public MAP CurrentMap;
    /// <summary>
    /// Выбранный и загруженный маршрут
    /// </summary>
    public WAY CurrentWay;

    /// <summary>
    /// Список класса данных для создания ярлыков на файлы
    /// </summary>
    public List<Shotcat> shotcats;
    #endregion

    #region Приватные поля
    /// <summary>
    /// Экземпляр объекта манимулирующим файлами
    /// </summary>
    private DataMagedger _dataManedger;
    #endregion

    #region Базовые методы
    //Проводим загрузку первой в списке карты и пути для нее
    //Генерируем ярлыки
    //Отправляем "бродкаст" на подгрузку данных из Хранилища
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

        shotcats = _dataManedger.GetShortName();

       AtionsSystem.UpdateValueForDataStore.Invoke();
    }
    #endregion

    #region Методы манипуляции с картой
    /// <summary>
    /// Подгружает и устанавливает карту из файла по имени
    /// </summary>
    /// <param name="name">Имя требуемой карты</param>
    public void ChengeMap(string name)
    {
        if (!_dataManedger.TryLoadMap(name, out CurrentMap))
        {
            Debug.Log("Error on load current map for name" + name);
        }

        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, CurrentMap.names_WAY[0], out CurrentWay))
        {
            Debug.Log("Error on load current way for name" + name);
        }

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }
    /// <summary>
    /// Сохраняет текущую карту
    /// </summary>
    public void SaveMap()
    {
        _dataManedger.SaveMap(CurrentMap);
    }
    /// <summary>
    /// Сохраняет карту по объекту
    /// </summary>
    /// <param name="other">Экземпляр карты</param>
    public void SaveMap(MAP other)
    {
        _dataManedger.SaveMap(other);
    }
    /// <summary>
    /// Создает новую карту с базовыми настройками
    /// </summary>
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
    /// <summary>
    /// Изменяет имя текущей карты
    /// </summary>
    /// <param name="NewName">Новое имя</param>
    public void ChangeNameMap(string NewName)
    {
        if (NewName == "" || NewName == CurrentMap.name_map)
            return;

        _dataManedger.ChangeName(CurrentMap, NewName);

        shotcats = _dataManedger.GetShortName();

        ChengeMap(NewName);
    }
    /// <summary>
    /// Удаляет текущую карту
    /// </summary>
    public void DeleteCurrentMap()
    {
        if (_dataManedger.TryRemoveMap(CurrentMap.name_map))
        {
            ChengeMap(_dataManedger.pathsMap[0]);
        }

        shotcats = _dataManedger.GetShortName();

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }
    #endregion

    #region Методы манипуляции с маршрутом
    /// <summary>
    /// Подгружает и устанавливает маршрут из файла по имени
    /// </summary>
    /// <param name="name">Имя требуемого маршрута</param>
    public void ChengeWay(string name)
    {
        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, name, out CurrentWay))
        {
            Debug.Log("Error on load current way by id 0");
        }

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }

    /// <summary>
    /// Сохраняет текущий маршрут
    /// </summary>
    public void SaveWay()
    {
        _dataManedger.SaveWay(CurrentMap, CurrentWay);
    }

    /// <summary>
    /// Создает новый маршрут с базовыми параметрами
    /// </summary>
    public void CreateWay()
    {

        WAY newWay = new WAY();

        newWay.positionWayPoints = new List<Vector2>();
        newWay.name_WAY = "NewWay_" + CurrentMap.names_WAY.Count.ToString();

        _dataManedger.SaveWay(CurrentMap, newWay);

        shotcats = _dataManedger.GetShortName();

        ChengeWay(newWay.name_WAY);
    }

    /// <summary>
    /// Изменяет имя текущего маршрута
    /// </summary>
    /// <param name="name">Новое имя маршрута</param>
    public void ChangeNameWay(string name)
    {
        if (name == "" || name == CurrentWay.name_WAY)
            return;

        _dataManedger.ChangeName(CurrentMap, name, CurrentWay);

        shotcats = _dataManedger.GetShortName();

        ChengeWay(name);
    }

    /// <summary>
    /// Удаляет тукущий маршрут
    /// </summary>
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
    #endregion
}