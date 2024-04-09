using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class NetworkManager : MonoBehaviour
{
    static UdpClient srv;
    static UdpClient snd;
    static UdpClient srv_points;
    Thread thread;
    IPEndPoint remoteEP;
    IPEndPoint remoteEP_pt;
    static private AsyncCallback AC;
    static readonly object lockObject = new object();
    static readonly object lockObject2 = new object();
    bool msg_rec=false;
    string msg="";
    string msgs="";

    public string[] elements;

    void Start()
    {   
        srv=new UdpClient(5556);
        snd=new UdpClient(5580);
        srv_points=new UdpClient(5566);
        remoteEP=new IPEndPoint(IPAddress.Any,0);
        remoteEP_pt=new IPEndPoint(IPAddress.Any,0);
        thread=new Thread(new ThreadStart(Udpreceive));
        thread.Start();

    }
    
    void Udpreceive(){
        while(true){
            lock(lockObject){
                byte[] dgram = srv.Receive(ref remoteEP);
                msg=System.Text.Encoding.UTF8.GetString(dgram,0,dgram.Length);
                //Debug.Log(msg);    
                
            }
        }
    }

    public string GetLatestData()
        {
        if (!string.IsNullOrEmpty(msg))
        {
            
            return msg;
        }
        else
        {
            return null;
        }
}


void OnApplicationQuit()
    {
        if(thread != null && thread.IsAlive){
            thread.Abort(); // Force the thread to stop
        }
        CloseUdpClients();
    }
   
    void CloseUdpClients()
    {
        if (srv != null){
            srv.Close();
        }
        if (snd != null){
            snd.Close();
        }
        if (srv_points != null){
            srv_points.Close();
        }
    }
}
