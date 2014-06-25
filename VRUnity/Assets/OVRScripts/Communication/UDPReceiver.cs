using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;


public class UDPReceiver : MonoBehaviour
{
	const int STEP_PORT_OFFSET = 0;
	const int HIP_ORIENTATION_PORT_OFFSET = 1;
	const int GUN_ORIENTATION_PORT_OFFSET = 2;
	const int FIRE_PORT_OFFSET = 3;
	const int VIEW_MODE_PORT_OFFSET = 4;
	const int SYNC_PORT_OFFSET = 5;
	
	const int SLEEP_TIME = 50;
	
	public SensorListener sensor;
	public int listeningPort;
	
	private bool mRunning;
	
	string receivedStepMessage = "";
	string receivedHipOrientationMessage = "";
	string receivedGunOrientationMessage = "";
	string receivedGunModeMessage = "";
	string receivedSyncMessage = "";
	string receivedViewModeMessage = "";
	
	
	List<UdpClient> clients = new List<UdpClient>();
	List<Thread> threads = new List<Thread>();
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,200,20),
			Network.player.ipAddress+":"+listeningPort);	
	}
	
    void Awake ()
	{
		mRunning = true;
		
		Thread stepThread = new Thread(()=>UDPReceiveData(listeningPort+STEP_PORT_OFFSET,ref receivedStepMessage));
		threads.Add (stepThread);
		
		Thread attitudeThread = new Thread(()=>UDPReceiveData(listeningPort+HIP_ORIENTATION_PORT_OFFSET,ref receivedHipOrientationMessage));
		threads.Add (attitudeThread);
		
		Thread gunAttitudeThread = new Thread(()=>UDPReceiveData(listeningPort+GUN_ORIENTATION_PORT_OFFSET,ref receivedGunOrientationMessage));
		threads.Add (gunAttitudeThread);
		
		Thread gunModeThread = new Thread(()=>UDPReceiveData(listeningPort+FIRE_PORT_OFFSET,ref receivedGunModeMessage));
		threads.Add (gunModeThread);
		
		Thread syncThread = new Thread(()=>UDPReceiveData(listeningPort+SYNC_PORT_OFFSET,ref receivedSyncMessage));
		threads.Add (syncThread);
		
		Thread viewModeThread = new Thread(()=>UDPReceiveData(listeningPort+VIEW_MODE_PORT_OFFSET,ref receivedViewModeMessage));
		threads.Add (viewModeThread);
		
		foreach(Thread thread in threads)
		{
			thread.Start();	
		}
		
	}
	
	public void stopListening()
	{
		mRunning = false;
	}
	
	
	void Update()
	{
		if (receivedStepMessage != "")
		{
			sensor.ParseStep(receivedStepMessage);
			receivedStepMessage = "";
		}
		
		if (receivedHipOrientationMessage != "")
		{
			sensor.ParseOrientation(receivedHipOrientationMessage);
			receivedHipOrientationMessage = "";
		}
		
		
		if (receivedGunOrientationMessage != "")
		{
			sensor.ParseGunOrientation(receivedGunOrientationMessage);
			receivedGunOrientationMessage = "";
		}
		
		if (receivedGunModeMessage != "")
		{
			sensor.ParseGunMode(receivedGunModeMessage);
			receivedGunModeMessage = "";
		}
		
		if (receivedSyncMessage != "")
		{
			sensor.ParseSync();
			receivedSyncMessage = "";
		}
		
		if (receivedViewModeMessage != "")
		{
			sensor.ParseViewMode(receivedViewModeMessage);
			receivedViewModeMessage = "";
		}
		
	}
	
	
	private void UDPReceiveData(int port, ref string  container)
	{
        UdpClient client = new UdpClient(port);
		clients.Add(client);
		
        while (mRunning)
		{
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
            byte[] data = client.Receive(ref anyIP); 

            string text = Encoding.UTF8.GetString(data); 
			container = text;				
			
			Thread.Sleep(SLEEP_TIME);
        }
	}
	
	void OnApplicationQuit()
	{
		stopListening(); 
		
		
		foreach(UdpClient client in clients)
		{
			if(client != null)
			{
				client.Close();	
			}
		}
		
		foreach(Thread thread in threads)
		{
			if(thread != null)
			{
				thread.Join(500);	
			}
		}
	}
	
}