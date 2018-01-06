using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject m_MainMenu;
    public GameObject m_ClientMenu;
    public GameObject m_ServerMenu;

    // Client menu
    public InputField m_ServerAddress;
    public InputField m_Port;
    public InputField m_CLogin;
    public InputField m_CPassword;

    // Server menu
    public InputField m_SLogin;
    public InputField m_SPassword;


    void Awake()
    {
        DisableCurrentMenu(ref m_ClientMenu);
        DisableCurrentMenu(ref m_ServerMenu);
        SetCurrentMenu(ref m_MainMenu);
    }

    void SetCurrentMenu(ref GameObject menu)
    {
        menu.SetActive(true);
    }

    void DisableCurrentMenu(ref GameObject menu)
    {
        menu.SetActive(false);
    }

    public void BackButtonAction()
    {
        DisableCurrentMenu(ref m_ClientMenu);
        DisableCurrentMenu(ref m_ServerMenu);
        SetCurrentMenu(ref m_MainMenu);
    }

    public void ClientButtonAction()
    {
        DisableCurrentMenu(ref m_MainMenu);
        SetCurrentMenu(ref m_ClientMenu);
    }

    public void ServerButtonAction()
    {
        DisableCurrentMenu(ref m_MainMenu);
        SetCurrentMenu(ref m_ServerMenu);
    }

    public void CreateServerButtonAction()
    {

    }

    public void ConnectButtonAction()
    {
        if (m_ServerAddress.text.Length == 0)
        {
            Debug.Log(m_ServerAddress.placeholder.GetComponent<Text>().text);
        }
        else
        {
            Debug.Log(m_ServerAddress.text);
        }
    }

    public void AddUserButtonAction()
    {

    }
}
