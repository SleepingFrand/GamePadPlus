using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

public class DataStore : MonoBehaviour
{
    #region cashe-path
    /// <summary>
    /// локальный путь приложения
    /// </summary>
    private string path_local;
    /// <summary>
    /// имя папки с файлами карт
    /// </summary>
    private string path_map_cashe = "Maps";
    /// <summary>
    /// расширение файла  карты
    /// </summary>
    private string _MapFileName = "map.json";
    #endregion

    #region variables
    /// <summary>
    ///  хранит в себе все созданные карты и наименования их путей
    /// </summary>
    public List<string> pathsMap = new List<string>();
    #endregion

    [SerializeField] private Texture2D BaseImage;

    #region sup_voids
    /// <summary>
    /// проверка на то что директория существует и на то что в ней есть необходимый не пустой файл 
    /// </summary>
    /// <param name="path_file"> полноя директория ведущая к файлу</param>
    /// <returns>true в случае успешности всех проверок false в случае не успешности хотябы 1 проверки</returns>
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

    #region void
    /// <summary>
    /// сохраняет конфиг карты
    /// </summary>
    /// <param name="map">конфиг карты</param>
    /// <returns>tru сохранение прошло успешно false провалилось</returns>
    public void SaveMap(MAP map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, map.name_map, _MapFileName); //получение строки к файлу конфигу карты
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);     //директори папки

        if (!Directory.Exists(pathFolder))
        {
            Directory.CreateDirectory(pathFolder);
            pathsMap.Add(map.name_map);
        }
        File.WriteAllText(pathMap,JsonUtility.ToJson(map));                         //сохранение файла карты
    }
    /// <summary>
    /// сохраняет файл пути карты
    /// </summary>
    /// <param name="name_map">имя карты</param>
    /// <param name="way">файл пути</param>
    /// <returns>tru сохранение прошло успешно false провалилось</returns>
    public void SaveWay(MAP map, WAY way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, map.name_map, way.name_WAY);//получение строки к файлу конфигу карты
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);//директори папки
        if (Directory.Exists(pathFolder) /*проверка на существование директории*/) 
        {
            File.WriteAllText(pathWay, JsonUtility.ToJson(way));//сохранение файла карты
        }
        else
        {
            SaveMap(map);
            SaveWay(map, way);
        }
    }

    /// <summary>
    /// загрузка карты 
    /// </summary>
    /// <param name="name_map">наименование карты</param>
    /// <param name="map">пременная в которую вернётся загруженное значение при возвращении true</param>
    /// <returns>true если загрузка прошла успешно false если загрузка провалилась</returns>
    public bool TryLoadMap(string name_map, out MAP map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, name_map , _MapFileName);//получение строки к файлу конфигу карты
        if (CheakExistFile(pathMap))
        {
            map = JsonConvert.DeserializeObject<MAP>(pathMap);
            return true;
        }
        else 
        {
            map = new MAP();//костыль
            return false;
        }
    }

    /// <summary>
    /// загрузка пути карты
    /// </summary>
    /// <param name="name_map">имя карты</param>
    /// <param name="name_way">имя пути</param>
    /// <param name="way">возвразяемая переменная</param>
    /// <returns>tru всё прошло успешно false произошла ошибка</returns>
    public bool TryLoadWay(string name_map, string name_way, out WAY way)
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, name_map, name_way);//получение строки к файлу конфигу карты
        if (CheakExistFile(pathMap))
        {
            way = JsonConvert.DeserializeObject<WAY>(pathMap);
            return true;
        }
        else
        {
            way = new WAY();//костыль
            return false;
        }
    }

    /// <summary>
    /// удаление карты 
    /// </summary>
    /// <param name="name_map">имя карты</param>
    /// <returns>true кдаление прошло успешно false удаление не произошло в связи с какой либо ошибкой</returns>
    public bool TryRemoveMap(string name_map)
    {
        string pathFolder = Path.Combine(path_local, path_map_cashe, name_map);//получение строки к файлу конфигу карты
        
        if (Directory.Exists(pathFolder))
        {
            Directory.Delete(pathFolder,true);
            return true;
        }
        else
        {
           return false;
        }
    }

    /// <summary>
    /// удалякет путь карты
    /// </summary>
    /// <param name="name_map">имя карты</param>
    /// <param name="name_way">имя пути</param>
    /// <returns>true кдаление прошло успешно false удаление не произошло в связи с какой либо ошибкой</returns>
    public bool TruRemoveWay(string name_map, string name_way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, name_map, name_way);//получение строки к файлу конфигу карты

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
    /// загрузка изображения из файла по директории
    /// </summary>
    /// <param name="filepath">директория с именем файла</param>
    /// <param name="img">возвращяемое значение</param>
    /// <returns>tru всё прошло удачно , false  пролизошла ошибка  </returns>
    public bool ChangeImage(ref MAP map) 
    {
        Texture2D texture = null;
        byte[] fileData;
        string filepath = EditorUtility.OpenFilePanel("Image map", "" , "*.png, *.jpeg, *.jpg");

        if (File.Exists(filepath))
        {
            fileData = File.ReadAllBytes(filepath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            map.image_Map = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// проверяет на то ,что по локальной директории  приложения имеются необходимые папки 
    /// или создаёт их
    /// </summary>
    /// <returns>при создании директории возвращяет false ,а при переборе уже созданной  true</returns>
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
            SaveMap(baseMap);
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
    #endregion
    
    #region cashe_datetype
    public struct MAP 
    {
        /// <summary>
        /// наименование карты
        /// </summary>
        public string name_map ;
        /// <summary>
        /// изображение карты
        /// </summary>
        public  Sprite image_Map;
        /// <summary>
        /// массив точек углов карты 
        /// </summary>
        public  Rect RectMap;
        /// <summary>
        /// имена путей для карты (файлов)
        /// </summary>
        public List<string> names_WAY;
    }
    public struct WAY 
    {
        /// <summary>
        /// имя пути
        /// формат *.json
        /// </summary>
        public string name_WAY;
        /// <summary>
        /// массив точек последовательности пути от 1 до последней
        /// </summary>
        public List<Vector2> positionWayPoints;
    }
    #endregion

    #region construct_distruct-start_update

    public GameObject Map;

    // Start is called before the first frame update
    void Start()
    {
        InitDataStore();


        if(TryLoadMap("BaseMap",out MAP map))
        {
            Map.GetComponent<Image>().sprite = map.image_Map;
        }
    }
    #endregion
}
