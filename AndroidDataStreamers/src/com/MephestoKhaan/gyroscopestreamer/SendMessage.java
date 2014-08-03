package com.MephestoKhaan.gyroscopestreamer;

import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;

import android.app.Application;
import android.content.Context;
import android.net.DhcpInfo;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.util.Log;


class SendMessage extends AsyncTask<String, Void, Void>
{
	private static String IP, PORT;
	public  static InetAddress broadcastIP;
	
	
	public static void SetAddress(String ip, String port, Context context)
	{
		IP = ip;
		PORT = port;
		
		try {
			broadcastIP = getBroadcastAddress(context);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	

    static InetAddress getBroadcastAddress(Context context) throws IOException 
    {
    	if(context == null)
    	{
    		return null;
    	}
        WifiManager wifi = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        DhcpInfo dhcp = wifi.getDhcpInfo();

        if(dhcp == null)
        {
        	return null;
        }
        
        
        int broadcast = (dhcp.ipAddress & dhcp.netmask) | ~dhcp.netmask;
        byte[] quads = new byte[4];
        for (int k = 0; k < 4; k++)
          quads[k] = (byte) ((broadcast >> k * 8) & 0xFF);
        return InetAddress.getByAddress(quads);
    }
	
    protected Void doInBackground(String... urls)
    {
		if(urls.length >= 3)
		{
			try
			{
				String ip = IP;
				int port = Integer.parseInt(PORT) + Integer.parseInt(urls[0]);
				String tag = urls[1];
				String message = urls[2];
				
				if(tag == "TCP")
				{
					sendOverTCP(InetAddress.getByName(ip), port, message);
				}
				else if(tag == "UDP")
				{
					sendOverUDP(InetAddress.getByName(ip), port, message);
				}
			}
			catch(IOException e)
			{
				Log.e("SEND", "error sendig "+urls[2]);
			}
		}
		
		return null;
    }
    
    
    private void sendOverTCP(InetAddress IP, int PORT, String message) throws IOException
    {
    	Socket socket = new Socket(IP,PORT);
		
		PrintWriter pw=new PrintWriter(new BufferedWriter(new OutputStreamWriter(socket.getOutputStream())));
		pw.println(message); 
		pw.flush();
		
		socket.close();
    }
    
    private void sendOverUDP(InetAddress IP, int PORT, String message) throws IOException
    {
    	
    	DatagramSocket socket = new DatagramSocket(PORT);
		socket.setBroadcast(true);
		DatagramPacket packet = new DatagramPacket(message.getBytes(), message.length(),IP , PORT);
		socket.send(packet);
		//Log.i("SEND",message);
		socket.close();
    }
    
    protected void onPostExecute(Void feed)
    {
    	
    }
    
 }