using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// ��������� ���������� ChekBox
/// </summary>
public class CheakBox_UI : MonoBehaviour
{
    //�������� ��� ��������� ��������
    [SerializeField] private Sprite ImageON;
    [SerializeField] private Sprite ImageOFF;
    //������� ���������
    [SerializeField] private bool State = false;
    // ������� ��� ���������
    [SerializeField] private UnityEvent EventON;
    [SerializeField] private UnityEvent EventOFF;

    private DataStore dataStore;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(StateRevers);
        dataStore = FindObjectOfType<DataStore>();
    }

    public void StateRevers()
    {
        State = !State;
        if (State)
        {
            this.gameObject.GetComponent<Image>().sprite = ImageON;
            EventON.Invoke();
        }
        else
        {
            this.gameObject.GetComponent<Image>().sprite = ImageOFF;
            EventOFF.Invoke();
        }
        if(this.tag == "EndWayCB")
        {
            dataStore.ButtonsStatus[1] = Convert.ToInt32(State);
            dataStore.SendValueStick();
        }
        else
        {
            dataStore.ButtonsStatus[0] = Convert.ToInt32(State);
            dataStore.SendValueStick();
        }
    }

    public void OffBoxWithoutEvent()
    {
        State = false;
        this.gameObject.GetComponent<Image>().sprite = ImageOFF;
    }
}
