﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WOC_Network;

namespace WOC_Server
{
    public class ConnectionInfo
    {
        public Socket Socket;
        public byte[] Buffer;
    }

    public class SessionInfo
    {
        ConnectionInfo connection;
        Account account;
    }

    class Server
    {
        private object serverLock = new object();
        private bool showText = true;

        private int port;
        private Socket serverSocket;

        public AccountsHandler accountHandler = new AccountsHandler();
        private List<ConnectionInfo> connections = new List<ConnectionInfo>();

        private void SetupServerSocket()
        {
            IPEndPoint myEndpoint = new IPEndPoint(IPAddress.Any, port);

            // Create the socket, bind it, and start listening
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Blocking = false;

            serverSocket.Bind(myEndpoint);
            serverSocket.Listen((int)SocketOptionName.MaxConnections);
        }

        public void Start()
        {
            Console.Write("Starting TCP server... ");
            try
            {
                SetupServerSocket();
                for (int i = 0; i < 10; i++)
                    serverSocket.BeginAccept(
                        new AsyncCallback(AcceptCallback), serverSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail.");
                Console.WriteLine(e);
            }
            Console.WriteLine("Done. Listening.");
        }


        private void AcceptCallback(IAsyncResult result)
        {
            Console.WriteLine("Accept!");
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                // Finish Accept
                Socket s = (Socket)result.AsyncState;
                connection.Socket = s.EndAccept(result);
                connection.Socket.Blocking = false;
                connection.Buffer = new byte[255];
                lock (connections) connections.Add(connection);

                Console.WriteLine("New connection from " + s);

                // Start Receive
                connection.Socket.BeginReceive(connection.Buffer, 0,
                    connection.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), connection);
                // Start new Accept
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback),
                    result.AsyncState);
            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Socket exception: " + exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Exception: " + exc);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;
            try
            {
                int bytesRead = connection.Socket.EndReceive(result);
                if (0 != bytesRead)
                {
                    string packetReceived = Encoding.UTF8.GetString(connection.Buffer, 0, bytesRead);
                    lock (serverLock)
                    {
                        if (showText)
                        {
                            Console.Write(packetReceived);
                        }
                    }
                    lock (connections)
                    {
                        HandleClientMessage(connection, packetReceived);
                    }
                    connection.Socket.BeginReceive(connection.Buffer, 0,
                        connection.Buffer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), connection);
                }
                else CloseConnection(connection);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                CloseConnection(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                CloseConnection(connection);
            }
        }

        public void HandleClientMessage(ConnectionInfo connection, string message)
        {
            try
            {
                IPacketData packet = PacketData.FromJson(message);

                if (packet != null)
                {
                    switch (packet)
                    {
                        case PacketDataAccountCreate data:
                            AccountCreate(connection, data);
                            break;
                        case PacketDataAccountConnect data:
                            AccountConnect(connection, data);
                            break;
                        case PacketDataAccountDisconnect data:
                            AccountDisconnect(connection, data);
                            break;
                        case PacketDataAccountList data:
                            AccountList(connection, data);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Unknow JSON message : " + message);
                    Send(connection, "Wrong JSON sent to server : " + message);
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Error while parsing JSON message : " + message);
                Send(connection, "Couldn't parse this message : " + message);
            }
        }

        private void AccountCreate(ConnectionInfo connection, PacketDataAccountCreate data)
        {
            if (accountHandler.Create(data.name, data.password))
            {
                Send(connection, "Account creation successful");
            }
            else
            {
                Send(connection, "Account creation failed");
            }
        }

        private void AccountConnect(ConnectionInfo connection, PacketDataAccountConnect data)
        {
            if (accountHandler.Connect(data.name, data.password))
            {
                Send(connection, "Connect successful");
            }
            else
            {
                Send(connection, "Account connection failed");
            }
        }

        private void AccountDisconnect(ConnectionInfo connection, PacketDataAccountDisconnect data)
        {
            if (accountHandler.Disconnect(data.name, data.password))
            {
                Send(connection, "Disconnect successful");
            }
            else
            {
                Send(connection, "Failed to disconnect");
            }
        }

        private void AccountList(ConnectionInfo connection, PacketDataAccountList data)
        {
            Send(connection, accountHandler.List());
        }

        private void Send(ConnectionInfo connection, string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg + "\n");
            connection.Socket.Send(bytes, bytes.Length, SocketFlags.None);
        }

        public void LoadAccounts(string path)
        {
            accountHandler.AddFromJson(File.ReadAllText(path));
            Console.WriteLine("loaded accounts from {0}", path);
        }

        public void SaveAccounts(string path)
        {
            File.WriteAllText(path, accountHandler.ToJson());
            Console.WriteLine("saved accounts to {0}", path);
        }

        private void CloseConnection(ConnectionInfo ci)
        {
            ci.Socket.Close();
            lock (connections) connections.Remove(ci);
        }

        public void ShowText()
        {
            lock (serverLock)
            {
                if (showText == false)
                {
                    showText = true;
                    Console.WriteLine("Text output enabled.");
                }
            }
        }

        public void HideText()
        {
            lock (serverLock)
            {
                if (showText == true)
                {
                    showText = false;
                    Console.WriteLine("Text output disabled.");
                }
            }
        }

        public void DropAll()
        {
            lock (connections)
            {
                for (int i = connections.Count - 1; i >= 0; i--)
                {
                    CloseConnection(connections[i]);
                }
            }
        }

        public void SendAll(string line)
        {
            lock (connections)
            {
                foreach (ConnectionInfo conn in connections)
                {
                    Send(conn, line);
                }
            }
        }

        public bool TryStart(string line)
        {
            bool portReady = false;
            try
            {
                port = int.Parse(line);
                if (port > short.MaxValue || port < 2)
                {
                    Console.WriteLine("Invalid port number.");
                    Console.Write("Please enter port number: ");
                }
                else
                {
                    Start();
                    portReady = true;
                }
            }
            catch
            {
                Console.WriteLine("Invalid port number.");
                Console.Write("Please enter port number: ");
            }

            return portReady;
        }

        public void Shutdown()
        {
            lock (connections)
            {
                for (int i = connections.Count - 1; i >= 0; i--)
                {
                    CloseConnection(connections[i]);
                }
            }
        }
    }
}