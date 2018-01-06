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
    private static MainMenu instance;
    public static MainMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MainMenu>();
            }
            return instance;
        }
    }

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

    private bool isConnectedToServer = false;


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
        string input_addr = m_ServerAddress.text;
        string input_port = m_Port.text;

        // adres IP
        if (input_addr.Length == 0)
        {
            input_addr = m_ServerAddress.placeholder.GetComponent<Text>().text;
        }

        // pobranie portu
        int port;
        if (input_port.Length == 0)
        {
            input_port = m_Port.placeholder.GetComponent<Text>().text;
        }

        int.TryParse(input_port, out port);

        if (!isConnectedToServer)
        {
            isConnectedToServer = network.ConnectToServer(input_addr, port);
        }

        if (isConnectedToServer)
        {
            DisableCurrentMenu(ref m_MainMenu);
            SetCurrentMenu(ref m_ClientMenu);
        }
    }

    public void ServerButtonAction()
    {
        DisableCurrentMenu(ref m_MainMenu);
        SetCurrentMenu(ref m_ServerMenu);
    }

    public void CreateServerButtonAction()
    {
        // TODO: Usunac pozniej - zahardkodowane zeby nie wpisywac przy kazdym odpaleniu builda
        if (!IsUserInDatabase("admin", "nimda"))
        {
            AddUser("admin", "nimda");
        }
        network.ButtonCreateServer();
        DisableCurrentMenu(ref m_ServerMenu);
        InfoMenu.Instance.WriteLine("Server created.");
    }

    public void ConnectButtonAction()
    {
        string input_login = m_CLogin.text;
        string input_password = m_CPassword.text;

        if (input_login.Length == 0)
        {
            input_login = "admin";
        }
        if (input_password.Length == 0)
        {
            input_password = "nimda";
        }

        if (isConnectedToServer)
        {
            string message = "login "+ input_login + " " + input_password;
            TestNetworkScript.Instance.SendNetworkMessageToServer(message);
        }
    }

    public void LogInResponse(string[] msg)
    {
        bool loginStatus = false;
        if (msg[2] == "1")
        {
            loginStatus = true;
        }

        if (loginStatus)
        {
            InfoMenu.Instance.WriteLine("User " + msg[3] + " singed in correctly.");
            network.InitializePlayer();

            DisableCurrentMenu(ref m_ClientMenu);
        }
        else
        {
            InfoMenu.Instance.WriteLine("Bad login or password of user: " + msg[3]);
        }
    }

    public void AddUserButtonAction()
    {
        if (m_SLogin.text.Length != 0 && m_SPassword.text.Length != 0)
        {
            AddUser(m_SLogin.text, m_SPassword.text);
            m_SLogin.text = "";
            m_SPassword.text = "";
        }
    }

    private void AddUser(string login, string pass)
    {
        StreamWriter sw = new StreamWriter("users.txt", true);
        sw.WriteLine(login + "," + pass);
        InfoMenu.Instance.WriteLine("New user added to database.");
        sw.Close();
    }

    public bool IsUserInDatabase(string login, string pass)
    {
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
                if (login == info.m_Login && pass == info.m_Password)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
