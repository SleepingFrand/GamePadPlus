using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��������� ������� �������
/// </summary>
public class AtionsSystem : MonoBehaviour
{
    /// <summary>
    /// ������ �������. ���������� ��� ���������� ������ � ������� ���������
    /// </summary>
    public static UnityAction UpdateValueForDataStore;

    /// <summary>
    /// ������ �������. ���������� ��� ���������� ������ � ������ ��������
    /// </summary>
    public static UnityAction UpdateValueOnSettings;
}
