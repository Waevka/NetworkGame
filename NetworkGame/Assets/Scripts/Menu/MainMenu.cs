using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class UserInfo
{
    public UserInfo(string login, string pass)
    {
        m_Login = login;
        m_Password = pass;
    }

    public string m_Login;
    public string m_Password;
}

public class MainMenu : MonoBehaviour
{
    public TestNetworkScript network;

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
        network = FindObjectOfType<TestNetworkScript>();
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
        network.ButtonCreateServer();
        DisableCurrentMenu(ref m_ServerMenu);
        InfoMenu.Instance.WriteLine("Server created.");
    }

    public void ConnectButtonAction()
    {
        string input_login = m_CLogin.text;
        string input_password = m_CPassword.text;
        string input_addr = m_ServerAddress.text;
        string input_port = m_Port.text;

        if (File.Exists("users.txt"))
        {
            List<UserInfo> database_login = new List<UserInfo>();
            string[] lines = File.ReadAllLines("users.txt");
            foreach (string line in lines)
            {
                string[] uinfo = line.Split(',');
                database_login.Add(new UserInfo(uinfo[0], uinfo[1]));
            }

            foreach (UserInfo info in database_login)
            {
                if (input_login == info.m_Login && input_password == info.m_Password)
                {
                    Debug.Log("jestem");
                }
            }
        }

        network.ButtonConnectToServer();
        DisableCurrentMenu(ref m_ClientMenu);
        InfoMenu.Instance.WriteLine("Connected to server.");

        //if (m_ServerAddress.text.Length == 0)
        //{
        //    Debug.Log(m_ServerAddress.placeholder.GetComponent<Text>().text);
        //}
        //else
        //{
        //    Debug.Log(m_ServerAddress.text);
        //}
    }

    public void AddUserButtonAction()
    {
        StreamWriter sw = new StreamWriter("users.txt", true);
        if (m_SLogin.text.Length != 0 && m_SPassword.text.Length != 0)
        {
            sw.WriteLine(m_SLogin.text + "," + m_SPassword.text);
            m_SLogin.text = "";
            m_SPassword.text = "";
        }
        sw.Close();
        InfoMenu.Instance.WriteLine("New user added to database.");
    }
}
