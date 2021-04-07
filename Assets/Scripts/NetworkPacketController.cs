using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPacketController : NetworkBehaviour
{
    [System.Serializable]
    public class Package
    {
        public float Horizontal;
        public float Vertical;
        public float Timestamp;
    }

    [System.Serializable]
    public class ReceivedPackage
    {
        public float X;
        public float Y;
        public float Z;
        public float Timestamp;
    }

    public NetworkPacketManager<Package> m_packageManager;
    public NetworkPacketManager<Package> PackageManager
    {
        get
        {
            if (m_packageManager == null)
            {
                m_packageManager = new NetworkPacketManager<Package>();
                if (isLocalPlayer)
                {
                    m_packageManager.OnRequiredPackageTransmit += TransmitPackageToServer;
                }
            }
            return m_packageManager;
        }
    }

    public NetworkPacketManager<ReceivedPackage> m_serverPackageManager;
    public NetworkPacketManager<ReceivedPackage> ServerPackageManager
    {
        get
        {
            if (m_serverPackageManager == null)
            {
                m_serverPackageManager = new NetworkPacketManager<ReceivedPackage>();
                if (isServer)
                {
                    m_serverPackageManager.OnRequiredPackageTransmit += TransmitPackageToClients;
                }
            }
            return m_serverPackageManager;
        }
    }

    private void TransmitPackageToServer(byte[] bytes)
    {
        CmdTransmitPackages(bytes);
    }

    private void TransmitPackageToClients(byte[] bytes)
    {
        RpcReceivedDataOnClient(bytes);
    }

    [Command] 
    private void CmdTransmitPackages(byte[] bytes)
    {
        PackageManager.ReceivePackages(bytes);
    }

    [ClientRpc]
    private void RpcReceivedDataOnClient(byte[] data)
    {
        ServerPackageManager.ReceivePackages(data);
    }

    public virtual void FixedUpdate()
    {
        PackageManager.Tick();
        ServerPackageManager.Tick();
    }
}
