using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

#region Классы
/// <summary>
/// Класс хранит имя карты и список имен маршрутов к ней
/// </summary>
public struct Shotcat
{
    public string MapName;
    public List<string> WayName;

    public Shotcat(string Mapname)
    {
        MapName = Mapname;
        WayName = new List<string>();
    }
}

/// <summary>
/// Класс хранит данные карты
/// </summary>
[System.Serializable]
public struct MAP
{
    /// <summary>
    /// Наименование карты
    /// </summary>
    public string name_map;
    /// <summary>
    /// Изображение карты
    /// </summary>
    public Sprite image_Map;
    /// <summary>
    /// RECT географический границ карты
    /// </summary>
    public Rect RectMap;
    /// <summary>
    /// Имена маршрутов для карты
    /// </summary>
    public List<string> names_WAY;
}

/// <summary>
/// Класс хранящий данные маршрута
/// </summary>
[System.Serializable]
public struct WAY
{
    /// <summary>
    /// Имя маршрута
    /// формат *.json
    /// </summary>
    public string name_WAY;
    /// <summary>
    /// Список позиций точек маршрута
    /// </summary>
    public List<Vector2> positionWayPoints;
}
#endregion

/// <summary>
/// Класс описывающий взаимодействие с файловой системой приложения
/// </summary>
public class DataMagedger : MonoBehaviour
{
    #region Приватные поля
    /// <summary>
    /// Локальный путь приложения
    /// </summary>
    private string path_local;
    /// <summary>
    /// Имя папки с файлами карт
    /// </summary>
    private string path_map_cashe = "Maps";
    /// <summary>
    /// Имя файла хранящий класс карты
    /// </summary>
    private string _MapFileName = "map.json";
    /// <summary>
    /// Стандартное изображение для карты
    /// </summary>
    [SerializeField] private Texture2D BaseImage; // Требуеться для создания карты, в случае отсутствия карт
    #endregion

    #region Поля
    /// <summary>
    ///  Хранит имена все созданные карты
    /// </summary>
    public List<string> pathsMap = new List<string>();
    #endregion

    #region Вспомогательные функции
    /// <summary>
    /// Проверка на существование файла и его заполненость
    /// </summary>
    /// <param name="path_file"> Полный путь к файлу</param>
    private bool CheakExistFile( string path_file) 
    {
        if ( File.Exists(path_file) == true /*есть файл*/
                    && new FileInfo(path_file).Length != 0 /*файл не пуст*/)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    #endregion

    #region Методы

    /// <summary>
    /// Сериализованная форма класса MAP, для записи в json файл
    /// </summary>
    [System.Serializable]
    class SerializeMap
    {
        // Данные изображения
        public float x, y; 
        public byte[] bytes_image;

        public string name;

        //Географицеские коордиинаты краев карты
        public Rect geograf;

        public List<string> names_WAY;

        /// <summary>
        /// Заполнить класс, исходя из полученной карты
        /// </summary>
        /// <param name="map"></param>
       public void CreateSerializeMap(MAP map)
        {
            this.x = map.image_Map.rect.width;
            this.y = map.image_Map.rect.height;
            this.bytes_image = ImageConversion.EncodeToJPG(map.image_Map.texture);
            this.geograf = map.RectMap;
            this.names_WAY = map.names_WAY;
            this.name = map.name_map;
        }

        /// <summary>
        /// Возвращает класс MAP, исходя из имеющихся данных
        /// </summary>
        public MAP DeSerializeMap()
        {
            MAP map = new MAP();

            Texture2D tex = new Texture2D((int)this.x, (int)this.y);
            ImageConversion.LoadImage(tex, this.bytes_image);
            map.image_Map = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
            
            map.RectMap = this.geograf;
            map.names_WAY = this.names_WAY;
            map.name_map = this.name;

            return map;
        }
    }

    /// <summary>
    /// Сохраняет карту в файл
    /// </summary>
    /// <param name="map">Экземпляр карты для созхранения</param>
    public void SaveMap(MAP map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, map.name_map, _MapFileName);  //получение строки к файлу конфигу карты
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);             //директори папки

        if (!Directory.Exists(pathFolder))
        {
            Directory.CreateDirectory(pathFolder);
        }

        SerializeMap smap = new SerializeMap();
        smap.CreateSerializeMap(map);

        File.WriteAllText(pathMap,JsonUtility.ToJson(smap));                         //сохранение файла карты
    }
    /// <summary>
    /// Сохраняет маршрут в файл
    /// </summary>
    /// <param name="name_map">Экземпляр карты, владельца маршрута</param>
    /// <param name="way">Экземпляр маршруда для созхранения</param>
    /// <returns>tru сохранение прошло успешно false провалилось</returns>
    public void SaveWay(MAP map, WAY way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, map.name_map, way.name_WAY + ".json");    //получение строки к файлу конфигу карты
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);                         //директори папки

        if (Directory.Exists(pathFolder)/*проверка на существование директории*/) 
        {
            File.WriteAllText(pathWay, JsonUtility.ToJson(way));                                            //сохранение файла карты
        }
        else
        {
            //В случае отсутствия карты, для начало сохраним ее
            SaveMap(map);
            SaveWay(map, way);
        }
    }

    /// <summary>
    /// Загрузка карты из файла 
    /// </summary>
    /// <param name="MapName">Имя требуемой карты</param>
    /// <param name="Map">пременная в которую вернётся загруженное значение при возвращении true</param>
    public bool TryLoadMap(string MapName, out MAP Map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, MapName , _MapFileName);//получение строки к файлу конфигу карты
        pathMap = Path.GetFullPath(pathMap);

        if (CheakExistFile(pathMap))
        {
            SerializeMap smap;
            smap = JsonConvert.DeserializeObject<SerializeMap>(File.ReadAllText(pathMap));
            Map = smap.DeSerializeMap();
            return true;
        }
        else 
        {
            Map = new MAP();
            return false;
        }
    }

    /// <summary>
    /// Pагрузка маршрута карты
    /// </summary>
    /// <param name="NameMap">Имя карты владельца</param>
    /// <param name="NameWay">Имя требуемого маршрута</param>
    /// <param name="Way">возвразяемая переменная</param>
    public bool TryLoadWay(string NameMap, string NameWay, out WAY Way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, NameMap, NameWay + ".json");//получение строки к файлу конфигу карты
        pathWay = Path.GetFullPath(pathWay);

        if (CheakExistFile(pathWay))
        {
            Way = JsonConvert.DeserializeObject<WAY>(File.ReadAllText(pathWay));
            return true;
        }
        else
        {
            Way = new WAY();
            return false;
        }
    }

    /// <summary>
    /// Удаление карты 
    /// </summary>
    /// <param name="NameMap">Имя удаляемой карты</param>
    public bool TryRemoveMap(string NameMap)
    {
        string pathFolder = Path.Combine(path_local, path_map_cashe, NameMap);//получение строки к файлу конфигу карты
        pathFolder = Path.GetFullPath(pathFolder);

        if (Directory.Exists(pathFolder))
        {
            pathsMap.Remove(NameMap);
            Directory.Delete(pathFolder,true);
            return true;
        }
        else
        {
           return false;
        }
    }

    /// <summary>
    /// Удалякет маршрут
    /// </summary>
    /// <param name="NameMap">Имя карты владельца</param>
    /// <param name="NameWay">Имя маршрута</param>
    public bool TryRemoveWay(string NameMap, string NameWay)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, NameMap, NameWay + ".json");//получение строки к файлу конфигу карты
        pathWay = Path.GetFullPath(pathWay);

        if (File.Exists(pathWay))
        {
            
            File.Delete(pathWay);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Смена именя карты
    /// </summary>
    /// <param name="Map">Экземпляр карты</param>
    /// <param name="NewName"> Новое имя</param>
    /// <returns></returns>
    public bool ChangeName(MAP Map, string NewName)
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, Map.name_map);
        pathMap = Path.GetFullPath(pathMap);

        if (Directory.Exists(pathMap))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pathMap);

            dirInfo.MoveTo(Path.Combine(dirInfo.Parent.FullName, NewName));
        }

        Map.name_map = NewName;

        SaveMap(Map);

        return true;
    }
    /// <summary>
    /// Смена именя маршрута
    /// </summary>
    /// <param name="Map">Экземпляр карты</param>
    /// <param name="NenName"> Новое имя</param>
    /// <param name="Way">Экземпляр пути</param>
    /// <returns></returns>
    public bool ChangeName(MAP Map, string NewMap, WAY Way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, Map.name_map, Way.name_WAY + ".json");

        pathWay = Path.GetFullPath(pathWay);

        if (File.Exists(pathWay))
        {
            FileInfo fileInfo = new FileInfo(pathWay);

            fileInfo.MoveTo(Path.Combine(Path.Combine(path_local, path_map_cashe, Map.name_map), NewMap + ".json"));
        }

        Map.names_WAY[Map.names_WAY.FindIndex((string wayname) => { return wayname == Way.name_WAY; })] = NewMap;

        Way.name_WAY = NewMap;

        SaveMap(Map);
        SaveWay(Map, Way);

        return true;
    }

    /// <summary>
    /// Сорздание карты через открытие изображения
    /// </summary>
    public bool CreateMap(out MAP map) 
    {
        map = new MAP();
        Texture2D texture = null;
        byte[] fileData;
        string filepath = EditorUtility.OpenFilePanelWithFilters("Image map", "" , new string[] { "Image files", "png,jpg,jpeg", "All files", "*" });

        if (File.Exists(filepath))
        { 
            fileData = File.ReadAllBytes(filepath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            map.image_Map = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            FileInfo fileinfo = new FileInfo(filepath);
            map.name_map = Path.GetFileNameWithoutExtension(fileinfo.Name);

            map.RectMap = new Rect(0, 0, 0, 0);
            map.names_WAY = new List<string>();

            WAY newWay = new WAY();
            newWay.name_WAY = "StartWay";
            map.names_WAY.Add(newWay.name_WAY);
            SaveWay(map, newWay);

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Получить список ярлыков
    /// </summary>
    /// <returns></returns>
    public List<Shotcat> GetShortName()
    {
        List<Shotcat> out_value = new List<Shotcat>();
        FileInfo tempInfo;

        string pathFolder = Path.Combine(path_local, path_map_cashe);//получение строки папки с картами

        foreach (DirectoryInfo item in new DirectoryInfo(pathFolder).GetDirectories())
        {
            out_value.Add(new Shotcat(item.Name));
            foreach (FileInfo item2 in item.GetFiles())
            {
                if(item2.Name != _MapFileName)
                    out_value[out_value.Count - 1].WayName.Add(Path.GetFileNameWithoutExtension(item2.Name));
            }
        }
        return out_value;
    }

    /// <summary>
    /// Инициализация файлого менеджера приложения
    /// или создаёт их
    /// </summary>
    public bool InitDataStore()
    {
        path_local = Application.persistentDataPath;//получение локального пути приложения 

        string pathFolder = Path.Combine( path_local , path_map_cashe);//получение строки папки с картами
        Debug.Log(path_local);
        if (!Directory.Exists(pathFolder))//проверка на существование папки с картами
        {
            //если нет директории то создаём её
            Directory.CreateDirectory(pathFolder);

            MAP baseMap = new MAP();
            baseMap.name_map = "BaseMap";
            baseMap.image_Map = Sprite.Create(BaseImage, new Rect(0, 0, BaseImage.width, BaseImage.height), new Vector2(0.5f, 0.5f), 100.0f);
            baseMap.RectMap = new Rect(48.86026839871063f, 2.290795676794097f, 48.85176748433558f, 2.307318084325832f);
            baseMap.names_WAY = new List<string>();
            WAY baseWay = new WAY();
            baseWay.name_WAY = "BaseWay";
            baseMap.names_WAY.Add(baseWay.name_WAY);
            SaveWay(baseMap, baseWay);

           return false; 
        }
        else
        {
            //если есть то запускаем протокол загрузки необходимых данных
            string[] dirs = Directory.GetDirectories(pathFolder);
            foreach (string dir in dirs) 
            {
                if (CheakExistFile(Path.Combine(dir, _MapFileName)))
                {
                    DirectoryInfo DirInfo = new DirectoryInfo(dir);
                    pathsMap.Add(DirInfo.Name);//добовление конфига карты во внутренний список
                }
            }
            return true;
        }
    }


    void Awake()
    {
        InitDataStore();
    }
    #endregion
}
