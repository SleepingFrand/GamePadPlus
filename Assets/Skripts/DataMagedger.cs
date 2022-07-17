using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

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
    /// ������ ����� ����� ����� 
    /// </summary>
    public Rect RectMap;
    /// <summary>
    /// ����� ����� ��� ����� (������)
    /// </summary>
    public List<string> names_WAY;
}

[System.Serializable]
public struct WAY
{
    /// <summary>
    /// ��� ����
    /// ������ *.json
    /// </summary>
    public string name_WAY;
    /// <summary>
    /// ������ ����� ������������������ ���� �� 1 �� ���������
    /// </summary>
    public List<Vector2> positionWayPoints;
}

public class DataMagedger : MonoBehaviour
{
    #region cashe-path
    /// <summary>
    /// ��������� ���� ����������
    /// </summary>
    private string path_local;
    /// <summary>
    /// ��� ����� � ������� ����
    /// </summary>
    private string path_map_cashe = "Maps";
    /// <summary>
    /// ���������� �����  �����
    /// </summary>
    private string _MapFileName = "map.json";
    #endregion

    #region variables
    /// <summary>
    ///  ������ � ���� ��� ��������� ����� � ������������ �� �����
    /// </summary>
    public List<string> pathsMap = new List<string>();
    #endregion

    [SerializeField] private Texture2D BaseImage;

    #region sup_voids
    /// <summary>
    /// �������� �� �� ��� ���������� ���������� � �� �� ��� � ��� ���� ����������� �� ������ ���� 
    /// </summary>
    /// <param name="path_file"> ������ ���������� ������� � �����</param>
    /// <returns>true � ������ ���������� ���� �������� false � ������ �� ���������� ������ 1 ��������</returns>
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

    #region void

    [System.Serializable]
    class SerializeMap
    {
        public float x, y;
        public byte[] bytes_image;

        public string name;

        public Rect geograf;

        public List<string> names_WAY;

       public void CreateSerializeMap(MAP map)
        {
            this.x = map.image_Map.rect.width;
            this.y = map.image_Map.rect.height;
            this.bytes_image = ImageConversion.EncodeToJPG(map.image_Map.texture);
            this.geograf = map.RectMap;
            this.names_WAY = map.names_WAY;
            this.name = map.name_map;
        }

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
    /// ��������� ������ �����
    /// </summary>
    /// <param name="map">������ �����</param>
    /// <returns>tru ���������� ������ ������� false �����������</returns>
    public void SaveMap(MAP map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, map.name_map, _MapFileName); //��������� ������ � ����� ������� �����
        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);     //��������� �����

        if (!Directory.Exists(pathFolder))
        {
            Directory.CreateDirectory(pathFolder);
            pathsMap.Add(map.name_map);
        }

        SerializeMap smap = new SerializeMap();
        smap.CreateSerializeMap(map);

        File.WriteAllText(pathMap,JsonUtility.ToJson(smap));                         //���������� ����� �����
    }
    /// <summary>
    /// ��������� ���� ���� �����
    /// </summary>
    /// <param name="name_map">��� �����</param>
    /// <param name="way">���� ����</param>
    /// <returns>tru ���������� ������ ������� false �����������</returns>
    public void SaveWay(MAP map, WAY way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, map.name_map, way.name_WAY + ".json");//��������� ������ � ����� ������� �����

        string pathFolder = Path.Combine(path_local, path_map_cashe, map.name_map);//��������� �����
        if (Directory.Exists(pathFolder) /*�������� �� ������������� ����������*/) 
        {
            File.WriteAllText(pathWay, JsonUtility.ToJson(way));//���������� ����� �����
            map.names_WAY.Add(way.name_WAY);
        }
        else
        {
            SaveMap(map);
            SaveWay(map, way);
        }
    }

    /// <summary>
    /// �������� ����� 
    /// </summary>
    /// <param name="name_map">������������ �����</param>
    /// <param name="map">��������� � ������� ������� ����������� �������� ��� ����������� true</param>
    /// <returns>true ���� �������� ������ ������� false ���� �������� �����������</returns>
    public bool TryLoadMap(string name_map, out MAP map) 
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, name_map , _MapFileName);//��������� ������ � ����� ������� �����

        pathMap = Path.GetFullPath(pathMap);
        if (CheakExistFile(pathMap))
        {
            SerializeMap smap;
            smap = JsonConvert.DeserializeObject<SerializeMap>(File.ReadAllText(pathMap));
            map = smap.DeSerializeMap();
            return true;
        }
        else 
        {
            map = new MAP();//�������
            return false;
        }
    }

    /// <summary>
    /// �������� ���� �����
    /// </summary>
    /// <param name="name_map">��� �����</param>
    /// <param name="name_way">��� ����</param>
    /// <param name="way">������������ ����������</param>
    /// <returns>tru �� ������ ������� false ��������� ������</returns>
    public bool TryLoadWay(string name_map, string name_way, out WAY way)
    {
        string pathMap = Path.Combine(path_local, path_map_cashe, name_map, name_way);//��������� ������ � ����� ������� �����
        if (CheakExistFile(pathMap))
        {
            way = JsonConvert.DeserializeObject<WAY>(pathMap);
            return true;
        }
        else
        {
            way = new WAY();//�������
            return false;
        }
    }

    /// <summary>
    /// �������� ����� 
    /// </summary>
    /// <param name="name_map">��� �����</param>
    /// <returns>true �������� ������ ������� false �������� �� ��������� � ����� � ����� ���� �������</returns>
    public bool TryRemoveMap(string name_map)
    {
        string pathFolder = Path.Combine(path_local, path_map_cashe, name_map);//��������� ������ � ����� ������� �����
        
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
    /// �������� ���� �����
    /// </summary>
    /// <param name="name_map">��� �����</param>
    /// <param name="name_way">��� ����</param>
    /// <returns>true �������� ������ ������� false �������� �� ��������� � ����� � ����� ���� �������</returns>
    public bool TruRemoveWay(string name_map, string name_way)
    {
        string pathWay = Path.Combine(path_local, path_map_cashe, name_map, name_way);//��������� ������ � ����� ������� �����

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
    /// �������� ����������� �� ����� �� ����������
    /// </summary>
    /// <param name="filepath">���������� � ������ �����</param>
    /// <param name="img">������������ ��������</param>
    /// <returns>tru �� ������ ������ , false  ���������� ������  </returns>
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
    /// ��������� �� �� ,��� �� ��������� ����������  ���������� ������� ����������� ����� 
    /// ��� ������ ��
    /// </summary>
    /// <returns>��� �������� ���������� ���������� false ,� ��� �������� ��� ���������  true</returns>
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
    #endregion
    
    #region construct_distruct-start_update

    // Start is called before the first frame update
    void Awake()
    {
        InitDataStore();
    }
    #endregion
}
