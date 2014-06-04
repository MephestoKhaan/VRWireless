package com.MephestoKhaan.gyroscopestreamer;

import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;

import android.os.AsyncTask;
import android.util.Log;


class SendMessage extends AsyncTask<String, Void, Void>
{
	private static String IP, PORT;
	
	public static void SetAddress(String ip, String port)
	{
		IP = ip;
		PORT = port;
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