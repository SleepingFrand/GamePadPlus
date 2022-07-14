using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileMagadger_UIControl : MonoBehaviour
{
    [SerializeField] private int StateManedger = 0;
    [SerializeField] private GameObject[] PanelsManedger;
    [SerializeField] private GameObject PanelMain;
    [SerializeField] private bool MainState = false;

    private RectTransform RectTransform;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(SetStete_Main);
        RectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetState(int state)
    {
        PanelsManedger[StateManedger].SetActive(false);
        PanelsManedger[state].SetActive(true);
        StateManedger = state;
    }


    public void SetStete_Main()
    {
        MainState = !MainState;
        PanelMain.SetActive(MainState);

        RectTransform.anchoredPosition -= new Vector2(0, PanelMain.GetComponent<RectTransform>().rect.yMax * (MainState?2:-2));
    }
}
