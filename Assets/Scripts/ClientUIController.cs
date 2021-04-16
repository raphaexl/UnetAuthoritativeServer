﻿
using UnityEngine;
using UnityEngine.UI;

public class ClientUIController : MonoBehaviour
{
    public static  ClientUIController Instance;
  
    public  Text onClientConnected;
    public  Text onClientReceiveFromServer;

    [SerializeField]
    Button connectBtn;
    [SerializeField]
    GameObject toogleGameObject;
    [HideInInspector]
    public bool clientSidePrediction = false;
    [HideInInspector]
    public bool serverReconcilation = false;
    [HideInInspector]
    public bool lagCompensation = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        /*toogleGameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        toogleGameObject.transform.GetChild(1).GetComponent<Toggle>().isOn = false;
        toogleGameObject.transform.GetChild(2).GetComponent<Toggle>().isOn = false;*/
    }

    public  void OnClientSidePredictionValueChanged(bool value)
    {
        clientSidePrediction = value;
    }
    public void OnServerReconciliationValueChanged(bool value)
    {
        serverReconcilation = value;
    }
    public void OnLagCompensationValueChanged(bool value)
    {
        lagCompensation = value;
    }


    public void OnEnableToogle()
    {
        connectBtn.gameObject.SetActive(false);
        toogleGameObject.SetActive(true);
    }
}
