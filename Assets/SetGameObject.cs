using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SetGameObject : MonoBehaviour
{

    public GameObject Not_Connected, Connected_Idle, Connected_Start;

    public GameObject Transition1;

    const int PORT_NO = 81;
    public string SERVER_IP = "127.0.0.1";
    const string REGISTER_CONNECTION = "RC";
    const string INIT_TRANSITION = "TZ";
    TcpClient client;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start - Starting");
        Not_Connected.SetActive(true);
        Connected_Idle.SetActive(false);
        Connected_Start.SetActive(false);
        Debug.Log("Start - Setup Thread");




        (new Thread(() => Setup())).Start();
        

    }

    // Update is called once per frame

    bool started = false;
    bool changedState = false;
    string state = "unconnected";
    int t_count = 0;

    void Update()
    {
        if (changedState)
        {
            Debug.Log("Update - Changed!");
            if (state == "unconnected")
            {
                if (!started)
                {
                    Not_Connected.SetActive(true);
                    Connected_Idle.SetActive(false);
                    Connected_Start.SetActive(false);
                    changedState = false;
                    Debug.Log("Update - Unconnected");
                }
            }
            else if (state == "connected")
            {
                if (!started)
                {
                    Not_Connected.SetActive(false);
                    Connected_Idle.SetActive(true);
                    Connected_Start.SetActive(false);
                    changedState = false;
                    Debug.Log("Update - Connected");
                }
            }
            else if (state == "transition") //transiton!
            {
               /* t_count++;

                if(t_count == 1) { //load transition 1

                } else if (t_count == 2) //laod transition 2
                {

                } else //load .....
                {

                }*/
            }
        }
    }

    void Setup()
    {
        Debug.Log("Setup - Setup...");
        //attempt to connect once...
        string textToSend = REGISTER_CONNECTION;

        try
        {
            //---create a TCPClient object at the IP and port no.---
            Debug.Log("Setup - Connecting");
            client = new TcpClient(SERVER_IP, PORT_NO);
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);
            Debug.Log("Setup - Connected");

            //---and see if connected---
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            Debug.Log("Setup - Sending");
            //---read back the text---
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            Debug.Log("Setup - Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            if (Encoding.ASCII.GetString(bytesToRead, 0, bytesRead) == REGISTER_CONNECTION)
            {
                Debug.Log("Setup - REGISTER_CONNECTION");
                changedState = true;
                state = "connected";
            }

            Debug.Log("Listen Thread");
            (new Thread(() => Listen())).Start();

        }
        catch
        {
            Debug.Log("Setup - Setup Issue");
            changedState = true;
            state = "unconnected";
        }
    }

    void Listen()
    {
        Debug.Log("Listen - Listening...");
        try
        {
            Debug.Log("Listen - Reading");
            //---read the text---
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = client.GetStream().Read(bytesToRead, 0, client.ReceiveBufferSize);
            Debug.Log("Listen - Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            //---convert the data received into a string---
            string dataReceived = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

            if (dataReceived == INIT_TRANSITION) // TRANSITION!
            {
                state = "transition";
                changedState = true;
            }
            else
            {
                state = "connected";
                changedState = true;
            }
        }
        catch
        {
            state = "unconnected";
            changedState = true;
            Debug.Log("Listen - Issue");
        }
    }
}
