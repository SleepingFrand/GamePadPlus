using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CheakBox_UI : MonoBehaviour
{
    [SerializeField] private Sprite ImageON;
    [SerializeField] private Sprite ImageOFF;

    [SerializeField] private bool State = false;

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
}
