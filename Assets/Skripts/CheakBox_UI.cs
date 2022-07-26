using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Реализует функционал ChekBox
/// </summary>
public class CheakBox_UI : MonoBehaviour
{
    //Картинки для состояний чекбокса
    [SerializeField] private Sprite ImageON;
    [SerializeField] private Sprite ImageOFF;
    //текущее состояние
    [SerializeField] private bool State = false;
    // события при состоянии
    [SerializeField] private UnityEvent EventON;
    [SerializeField] private UnityEvent EventOFF;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(StateRevers);
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
    }

    public void OffBoxWithoutEvent()
    {
        State = !State;
        this.gameObject.GetComponent<Image>().sprite = ImageOFF;
    }
}
