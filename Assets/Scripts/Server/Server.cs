using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour {
    
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList; //keeps track of disconnects

    public int port = 6321;
    private TcpListener server;
    private bool serverStarted;
    private string toClient;

    private void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Debug.Log("Server has been started on port " + port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted)
            return;

        foreach (ServerClient c in clients)
        {
            if (!serverStarted)
                return; 

            //is the client still connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            // check for message from the client
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast(disconnectList[i].clientName + " has disconnected", clients);
            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();
    }
    private void OnIncomingData(ServerClient c, string data)
    {
        string prompt = data;
        prompt = prompt.ToLower();

        //Testing for "Or" clause,(highest precedence)
        if (prompt.Contains("or"))
        {
            toClient = OrInput();
        }

        //Test for unclear inputs (expecting sentence of the form "why should ...")
        else if (prompt.Contains("why")
        || prompt.Contains("where")
        || prompt.Contains("what")
        || prompt.Contains("when")
        || prompt.Contains("who")
        || prompt.Contains("how"))
        {
            toClient = UnclearInput(prompt);
        }

        //Testing for good questions. (If were here we have a prompt like "Can I have ...")
        else if (prompt.Contains("can")
        || prompt.Contains("will")
        || prompt.Contains("should")
        || prompt.Contains("is")
        || prompt.Contains("would")
        || prompt.Contains("shall")
        || prompt.Contains("do")
        || prompt.Contains("does"))
        {
            toClient = GoodInput();
        }

        //If I forgot anything
        else
        {
            toClient = BadInput();
        }

        //output to the client
        try
        {
            StreamWriter writer = new StreamWriter(c.tcp.GetStream());
            writer.WriteLine(toClient);
            writer.Flush();
        }
        catch (Exception e)
        {
            Debug.Log("Write error: " + e.Message + " to client " + c.clientName);
        }
    }

    private string GoodInput()
    {
        System.Random rnd = new System.Random();
        int OutputSelect = rnd.Next(1, 15);
        //Console.WriteLine(OutputSelect); //debugging line
        if (OutputSelect == 1)
        {
            return "It's happening!";
        }
        if (OutputSelect == 2)
        {
            return "It is absolutely so";
        }
        if (OutputSelect == 3)
        {
            return "There is no doubt";
        }
        if (OutputSelect == 4)
        {
            return "You may rely on it";
        }
        if (OutputSelect == 5)
        {
            return "As the conch sees it, yes";
        }
        if (OutputSelect == 6)
        {
            return "Extremely likely";
        }
        if (OutputSelect == 7)
        {
            return "Outlook looking good";
        }
        if (OutputSelect == 8)
        {
            return "Yes";
        }
        if (OutputSelect == 9)
        {
            return "All evidence says yes";
        }
        if (OutputSelect == 10)
        {
            return "Don't bet any money on it";
        }
        if (OutputSelect == 11)
        {
            return "I don't think so";
        }
        if (OutputSelect == 12)
        {
            return "A little birdie told me no";
        }
        if (OutputSelect == 13)
        {
            return "Outlook not so good";
        }
        if (OutputSelect == 14)
        {
            return "Very doubtful";
        }
        return "";
    }
    private string BadInput()
    {
        System.Random rnd = new System.Random();
        int OutputSelect = rnd.Next(1, 6);
        //Console.WriteLine(OutputSelect); //debugging line
        if (OutputSelect == 1)
        {
            return "Instructions unclear, conch self destructed";
        }
        if (OutputSelect == 2)
        {
            return "Maybe ask again later?";
        }
        if (OutputSelect == 3)
        {
            return "I'm not gonna tell you";
        }
        if (OutputSelect == 4)
        {
            return "Impossible to say";
        }
        if (OutputSelect == 5)
        {
            return "Come back with a better question";
        }
        return "Try again later";
    }
    private string UnclearInput(string prompt)
    {
        System.Random rnd = new System.Random();
        int OutputSelect = rnd.Next(1, 5);//50% chance of complaining and 50% chance to give unique response
                                          //Console.WriteLine(OutputSelect); //debugging line
        if ((OutputSelect == 1) || (OutputSelect == 2))
        {
            if (prompt.Contains("who"))
            {
                return "I can't figure out who";
            }
            else if (prompt.Contains("what"))
            {
                return "I don't know what that is";
            }
            else if (prompt.Contains("when"))
            {
                return "Which timeline are we in right now?";
            }
            else if (prompt.Contains("where"))
            {
                return "Somewhere in the universe";
            }
            else if (prompt.Contains("why"))
            {
                return "I don't know why";
            }
            else if (prompt.Contains("how"))
            {
                return "How would I know?";
            }
        }
        if ((OutputSelect == 3) || (OutputSelect == 4))
        {
            toClient = BadInput();
        }
        return "Please try again.";
    }
    private string OrInput()
    {
        System.Random rnd = new System.Random();
        int OutputSelect = rnd.Next(1, 4);
        //Console.WriteLine(OutputSelect); //debugging line
        if (OutputSelect == 1)
        {
            return "Go left";
        }
        if (OutputSelect == 2)
        {
            return "Trust your feelings";
        }
        if (OutputSelect == 3)
        {
            return "Go right";
        }
        return "Please Ask Again";
    }

    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception e)
            {
                Debug.Log("Write error: " + e.Message + " to client " + c.clientName);
            }
        }
    }

    
} // end of Server : Monobehavior

//simple definition of client for server
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    //constructs ServerClient 
    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
