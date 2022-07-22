using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

#region ������
/// <summary>
/// ����� ������ ��� ����� � ������ ���� ��������� � ���
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
/// ����� ������ ������ �����
/// </summary>
[System.Serializable]
public struct MAP
{
    /// <summary>
    /// ������������ �����
    /// </summary>
    public string name_map;
    /// <summary>
    /// ����������� �����
    /// </summary>
    public Sprite image_Map;
    /// <summary>
    /// RECT �������������� ������ �����
    /// </summary>
    public Rect RectMap;
    /// <summary>
    /// ����� ��������� ��� �����
    /// </summary>
    public List<string> names_WAY;
}

/// <summary>
/// ����� �������� ������ ��������
/// </summary>
[System.Serializable]
public struct WAY
{
    /// <summary>
    /// ��� ��������
    /// ������ *.json
    /// </summary>
    public string name_WAY;
    /// <summary>
    /// ������ ������� ����� ��������
    /// </summary>
    public List<Vector2> positionWayPoints;
}
#endregion

/// <summary>
/// ����� ����������� �������������� � �������� �������� ����������
/// </summary>
public class DataMagedger : MonoBehaviour
{
    #region ��������� ����
    /// <summary>
    /// ��������� ���� ����������
    /// </summary>
    private string path_local;
    /// <summary>
    /// ��� ����� � ������� ����
    /// </summary>
    private string path_map_cashe = "Maps";
    /// <summary>
    /// ��� ����� �������� ����� �����
    /// </summary>
    private string _MapFileName = "map.json";
    /// <summary>
    /// ����������� ����������� ��� �����
    /// </summary>
    [SerializeField] private Texture2D BaseImage; // ���������� ��� �������� �����, � ������ ���������� ����
    #endregion

    #region ����
    /// <summary>
    ///  ������ ����� ��� ��������� �����
    /// </summary>
    public List<string> pathsMap = new List<string>();
    #endregion

    #region ��������������� �������
    /// <summary>
    /// �������� �� ������������� ����� � ��� ������������
    /// </summary>
    /// <param name="path_file"> ������ ���� � �����</param>
    private bool CheakExistFile( string path_file) 
    {
        if ( File.Exists(path_file) == true /*���� ����*/
                    && new FileInfo(path_file).Length != 0 /*���� �� ����*/)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    #endregion

    #region ������

    /// <summary>
    /// ��������������� ����� ������ MAP, ��� ������ � json ����
    /// </summary>
    [System.Serializable]
    class SerializeMap
    {
        // ������ �����������
        public float x, y; 
        public byte[] bytes_image;

        public string name;

        //�������������� ����������� ����� �����
        public Rect geograf;

        public List<string> names_WAY;

        /// <summary>
        /// ��������� �����, ������ �� ���������� �����
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
        /// ���������� ����� MAP, ������ �� ��������� ������
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
    /// ��������� ����� � ����
    /// </summary>
    /// <param name="map">��������� ����� ��� �����������</param>
    public void SaveMap(MAP map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, map.name_map, _MapFileName);  //��������� ������ � ����� ������� �����
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);             //��������� �����

        if (!Directory.Exists(pathFolder))
        {
            Directory.CreateDirectory(pathFolder);
        }

        SerializeMap smap = new SerializeMap();
        smap.CreateSerializeMap(map);

        File.WriteAllText(pathMap,JsonUtility.ToJson(smap));                         //���������� ����� �����
    }
    /// <summary>
    /// ��������� ������� � ����
    /// </summary>
    /// <param name="name_map">��������� �����, ��������� ��������</param>
    /// <param name="way">��������� �������� ��� �����������</param>
    /// <returns>tru ���������� ������ ������� false �����������</returns>
    public void SaveWay(MAP map, WAY way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, map.name_map, way.name_WAY + ".json");    //��������� ������ � ����� ������� �����
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);                         //��������� �����

        if (Directory.Exists(pathFolder)/*�������� �� ������������� ����������*/) 
        {
            File.WriteAllText(pathWay, JsonUtility.ToJson(way));                                            //���������� ����� �����
        }
        else
        {
            //� ������ ���������� �����, ��� ������ �������� ��
            SaveMap(map);
            SaveWay(map, way);
        }
    }

    /// <summary>
    /// �������� ����� �� ����� 
    /// </summary>
    /// <param name="MapName">��� ��������� �����</param>
    /// <param name="Map">��������� � ������� ������� ����������� �������� ��� ����������� true</param>
    public bool TryLoadMap(string MapName, out MAP Map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, MapName , _MapFileName);//��������� ������ � ����� ������� �����
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
    /// P������� �������� �����
    /// </summary>
    /// <param name="NameMap">��� ����� ���������</param>
    /// <param name="NameWay">��� ���������� ��������</param>
    /// <param name="Way">������������ ����������</param>
    public bool TryLoadWay(string NameMap, string NameWay, out WAY Way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, NameMap, NameWay + ".json");//��������� ������ � ����� ������� �����
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
    /// �������� ����� 
    /// </summary>
    /// <param name="NameMap">��� ��������� �����</param>
    public bool TryRemoveMap(string NameMap)
    {
        string pathFolder = Path.Combine(path_local, path_map_cashe, NameMap);//��������� ������ � ����� ������� �����
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
    /// �������� �������
    /// </summary>
    /// <param name="NameMap">��� ����� ���������</param>
    /// <param name="NameWay">��� ��������</param>
    public bool TryRemoveWay(string NameMap, string NameWay)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, NameMap, NameWay + ".json");//��������� ������ � ����� ������� �����
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
    /// ����� ����� �����
    /// </summary>
    /// <param name="Map">��������� �����</param>
    /// <param name="NewName"> ����� ���</param>
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
    /// ����� ����� ��������
    /// </summary>
    /// <param name="Map">��������� �����</param>
    /// <param name="NenName"> ����� ���</param>
    /// <param name="Way">��������� ����</param>
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
    /// ��������� ����� ����� �������� �����������
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
    /// �������� ������ �������
    /// </summary>
    /// <returns></returns>
    public List<Shotcat> GetShortName()
    {
        List<Shotcat> out_value = new List<Shotcat>();
        FileInfo tempInfo;

        string pathFolder = Path.Combine(path_local, path_map_cashe);//��������� ������ ����� � �������

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
    /// ������������� ������� ��������� ����������
    /// ��� ������ ��
    /// </summary>
    public bool InitDataStore()
    {
        path_local = Application.persistentDataPath;//��������� ���������� ���� ���������� 

        string pathFolder = Path.Combine( path_local , path_map_cashe);//��������� ������ ����� � �������
        Debug.Log(path_local);
        if (!Directory.Exists(pathFolder))//�������� �� ������������� ����� � �������
        {
            //���� ��� ���������� �� ������ �
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
            //���� ���� �� ��������� �������� �������� ����������� ������
            string[] dirs = Directory.GetDirectories(pathFolder);
            foreach (string dir in dirs) 
            {
                if (CheakExistFile(Path.Combine(dir, _MapFileName)))
                {
                    DirectoryInfo DirInfo = new DirectoryInfo(dir);
                    pathsMap.Add(DirInfo.Name);//���������� ������� ����� �� ���������� ������
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
