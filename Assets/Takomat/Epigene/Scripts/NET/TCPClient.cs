//------------------------------------------------------------------------------
// Copyright (c) 2014-2015 takomat GmbH and/or its licensors.
// All Rights Reserved.

// The coded instructions, statements, computer programs, and/or related material
// (collectively the "Data") in these files contain unpublished information
// proprietary to takomat GmbH and/or its licensors, which is protected by
// German federal copyright law and by international treaties.

// The Data may not be disclosed or distributed to third parties, in whole or in
// part, without the prior written consent of takoamt GmbH ("takomat").

// THE DATA IS PROVIDED "AS IS" AND WITHOUT WARRANTY.
// ALL WARRANTIES ARE EXPRESSLY EXCLUDED AND DISCLAIMED. TAKOMAT MAKES NO
// WARRANTY OF ANY KIND WITH RESPECT TO THE DATA, EXPRESS, IMPLIED OR ARISING
// BY CUSTOM OR TRADE USAGE, AND DISCLAIMS ANY IMPLIED WARRANTIES OF TITLE,
// NON-INFRINGEMENT, MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE OR USE.
// WITHOUT LIMITING THE FOREGOING, TAKOMAT DOES NOT WARRANT THAT THE OPERATION
// OF THE DATA WILL gameengine_dialogsmanagerBE UNINTERRUPTED OR ERROR FREE.

// IN NO EVENT SHALL TAKOMAT, ITS AFFILIATES, LICENSORS BE LIABLE FOR ANY LOSSES,
// DAMAGES OR EXPENSES OF ANY KIND (INCLUDING WITHOUT LIMITATION PUNITIVE OR
// MULTIPLE DAMAGES OR OTHER SPECIAL, DIRECT, INDIRECT, EXEMPLARY, INCIDENTAL,
// LOSS OF PROFITS, REVENUE OR DATA, COST OF COVER OR CONSEQUENTIAL LOSSES
// OR DAMAGES OF ANY KIND), HOWEVER CAUSED, AND REGARDLESS
// OF THE THEORY OF LIABILITY, WHETHER DERIVED FROM CONTRACT, TORT
// (INCLUDING, BUT NOT LIMITED TO, NEGLIGENCE), OR OTHERWISE,
// ARISING OUT OF OR RELATING TO THE DATA OR ITS USE OR ANY OTHER PERFORMANCE,
// WHETHER OR NOT TAKOMAT HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH LOSS
// OR DAMAGE.
//------------------------------------------------------------------------------
// This class is part of the epigene(TM) Software Framework.
// All license issues, as above described, have to be negotiated with the
// takomat GmbH, Cologne.
//------------------------------------------------------------------------------

using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// Add/remove this define to Player Settings (per platform)
// Only need to define if you miss ios/android pro plugin
#if !UNITY_MOBILE_FREE
using System.Net.Sockets;
using System.Net;
#endif

namespace Epigene.Network
{
#if !UNITY_MOBILE_FREE
    /// <summary>
    /// Send TCP data to network
    /// </summary>
    public class TCPClient
    {
    	public int defaultTimeout = 2000; //10000;
    	public int sleepTime = 30;

    	private string serverUrl;
    	private int serverPort;
        private int mutexTime = 1000;
		
		private TcpClient tcpClient;
		private Thread receiverThread;
		private Mutex responseMutex = null;
    	private Socket socket = null;

    	public bool Connected
    	{
    		get{return tcpClient != null;}
    	}

        //TODO:
		// delegete receiveFunction
		// mode = {BLOCKING, NON_BLOCKING, THREADING...}
		// 
		

		byte[] buffer;

    	public TCPClient(string url, int port)
    	{
    		serverUrl = url;
    		serverPort = port;
    		buffer = new byte[4096]; //TODO: this should be dynamic

    	}

    	public void Start()
    	{
    		try
    		{
    			if(tcpClient == null )
    			{
					tcpClient = new TcpClient(serverUrl, serverPort);				
					socket = tcpClient.Client;
    			}
			
	   		}
	   		catch
	   		{
	    		Debug.LogWarning("No one listen on TCP: " + serverUrl + ":" + serverPort);
	    		tcpClient = null;
	    		socket = null;
	   		}

	   		if(responseMutex == null)
	   		{
            	responseMutex = new Mutex(false, "ResponseMutex");
	   		}

    	}

    	public void Stop()
    	{
    		if(tcpClient != null)
    		{
    			tcpClient.Close();
    			tcpClient = null;
    		}
    		socket = null;
    	}

		public void Send(byte[] buffer, int size = 0, int timeout = -1, int offset = 0)
		{
			//TODO
			Stop();	// make sure no pending connection
			Start();

			if (socket == null)
				return;

			if(!socket.Connected)
			{
				Debug.Log(">>>>>> Socket not connected anymore");
				return;
			}

			if (size == 0)
				size = buffer.Length;
			if (timeout == -1)
				timeout = defaultTimeout;

			int startTickCount = Environment.TickCount;
			int sent = 0;  // how many bytes is already sent

			receiverThread = new Thread(new ThreadStart(ReadData));
			receiverThread.Start();

			do
			{
				if (Environment.TickCount > startTickCount + timeout)
					throw new Exception("Timeout.");
				try 
				{
					sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode == SocketError.WouldBlock ||
				    	ex.SocketErrorCode == SocketError.IOPending ||
				    	ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
					{
						// socket buffer is probably full, wait and try again
						Thread.Sleep(sleepTime);
				  	}
					else
					{
						Debug.Log("throw ex:"+ex);
				    	throw ex;  // any serious error occurr
					}
				}
			} while (sent < size);
		}

		private void ReadData()
		{
			Debug.Log("ReadData");

			try
			{
				if (responseMutex.WaitOne(mutexTime))
				{
					
					if (Receive(tcpClient, buffer))
					{
						string str = System.Text.Encoding.Default.GetString(buffer);
						Debug.Log("buffer: " + str);
					}
				}
			}
			finally
			{
				responseMutex.ReleaseMutex();
			}
		}
		
		public bool Receive(TcpClient tcpClient, byte[] buffer, int size = 0, int timeout = -1, int offset = 0)
		{
			if (tcpClient.Client == null 
				|| !tcpClient.Client.Connected)
				return false;

			if (size == 0)
				size = buffer.Length;
			if (timeout == -1)
				timeout = defaultTimeout;
	
			int startTickCount = Environment.TickCount;
			int received = 0;  // how many bytes is already received
			do
			{
				if (Environment.TickCount > startTickCount + timeout)
				{
					// Debug.Log("Receive timeout");
					// throw new Exception("Timeout.");
					break;
				}

				try
				{

					int s = tcpClient.Client.Receive(buffer, offset + received, size - received, SocketFlags.None);
					if(s > 0)	Debug.Log("receive.... "+s);

					received += s;
					if (s == 0 && received > 0) //TODO check with 64k+ packets, why 10923 bytes??
					{
						break;
					}
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode == SocketError.WouldBlock ||
						ex.SocketErrorCode == SocketError.IOPending ||
						ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
					{
						// socket buffer is probably empty, wait and try again
						Thread.Sleep(sleepTime);
					}
					else
					{
						Debug.Log("Throw ex2:"+ex);
						throw ex;  // any serious error occurr
					}
				}
			} while (received == 0); // < size);
			
		
			return (received > 0);
		}

	}//class

#else //!UNITY_MOBILE_FREE

	public class NetworkStream
	{
		public IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state) {return null;}
		public int EndRead(IAsyncResult result) { return 0; }
		public int Write(Byte[] a, Int32 b, Int32 c) { return 0; }
		public void Flush() {}
		public void Close() {}
	}

	public class TcpClient
	{
		public Socket Client;
		public int ReceiveBufferSize;

		public TcpClient(string a, int b) {}
		public NetworkStream GetStream() { return null; }
		public void Close() {}
	}

	public enum SocketFlags { None = 0 };
	public enum SocketError { None = 0, WouldBlock, IOPending, NoBufferSpaceAvailable };

	public class Socket
	{
		public bool Connected = false;

		public int Send(Byte[] a, Int32 b, Int32 c, SocketFlags d) { return 0; }
		public int Receive(Byte[] a, Int32 b, Int32 c, SocketFlags d) { return 0; }
	}

	public class SocketException : System.Exception
	{
		public SocketError SocketErrorCode = SocketError.None;

	}

#endif //!UNITY_MOBILE_FREE

}//namespace