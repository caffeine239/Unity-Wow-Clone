using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    public static NetworkHandler mainNetwork;
    public Socket mSocket = null;
    public bool ConnectedToWorld = false;
    public byte[] DataBuffer;
    public AuthenticationSession _authSession;
    public WorldSession _worldSession;
    public Queue QueuedPackets = null;

    public static void OnLoad()
    {
        if (mainNetwork != null)
        {
            return;
        }

        GameObject go = new GameObject("Network Handler");
        NetworkHandler newManager = go.AddComponent<NetworkHandler>();
        mainNetwork = newManager;
        mainNetwork.QueuedPackets = new Queue();
    }
    // Update is called once per frame
    void Update()
    {
        if (mSocket != null)
        {
            if (IsConnected)
            {
                if (QueuedPackets.Count > 0)
                {
                    PacketReader pkt = (PacketReader)QueuedPackets.Dequeue();
                    Debug.Log("Invoking: [" + pkt.Opcode + "]");
                    PacketHandler.InvokeHandler(pkt, _worldSession, pkt.Opcode);
                }
            }

            while (mSocket.Available > 0)
            {
                DataBuffer = new byte[mSocket.Available];
                mSocket.Receive(DataBuffer, DataBuffer.Length, SocketFlags.None);

                if(ConnectedToWorld)
                {

                }
                else
                {
                    _authSession.SocketResponse(DataBuffer);
                }

            }

        }
    }

    public bool ConnectToSocket(string address, int port, bool worldConnect = false)
    {
        if (worldConnect)
        {
            Disconnect();
        }

        Regex DnsMatch = new Regex("[a-zA-Z]");
        IPAddress ASAddr;

        try
        {
            if (DnsMatch.IsMatch(address))
                ASAddr = Dns.GetHostEntry(address).AddressList[0];
            else
                ASAddr = IPAddress.Parse(address);

            IPEndPoint ASDest = new IPEndPoint(ASAddr, port);

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            mSocket.Connect(ASDest);

            ConnectedToWorld = worldConnect;

            return true;
        }
        catch
        {
            LoginUIHandler.LoginUIInstance.DisplayDialogUI("Unable To Connect.", "Okay", "", LoginUIHandler.LoginUIInstance.HideDialogUI);
            return false;
        }
    }
    public void Send(byte[] packet)
    {
        if (mSocket.Connected)
        {
            mSocket.Send(packet);
        }
    }
    public bool IsConnected
    {
        get
        {
            bool connected = !((mSocket.Poll(1000, SelectMode.SelectRead) && (mSocket.Available == 0)) || !mSocket.Connected);
            try
            {
                if(!connected)
                    ConnectedToWorld = false;

                return connected;
            }
            catch
            {
                ConnectedToWorld = false;
                return false;
            }
        }
    }
    public void Disconnect()
    {
        mSocket.Shutdown(SocketShutdown.Both);
        mSocket.Disconnect(false);
        mSocket.Dispose();
    }
}
