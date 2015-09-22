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
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

// Add/remove this define to Player Settings (per platform)
// Only need to define if you miss ios/android pro plugin
#if !UNITY_MOBILE_FREE
using System.Net.Sockets;
using System.Net;
#endif

using Epigene.Network;
//------------------------------------------------------------------------------
namespace Epigene.Network
{
#if !UNITY_MOBILE_FREE

	public class HandleClientRequest
	{

		private ProcessServerFunction processServerMessage;
		private TcpClient _clientSocket;
		private NetworkStream _networkStream = null;

		

		public HandleClientRequest(TcpClient clientConnected, 
		                           ProcessServerFunction func)
		{
		    this._clientSocket = clientConnected;
		    processServerMessage = func;
		}
		public void StartClient()
		{
		    _networkStream = _clientSocket.GetStream();
		    WaitForRequest();
		}

		public void WaitForRequest()
		{
		    byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];

		    _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
		}

		private void ReadCallback(IAsyncResult result)
		{
		    NetworkStream networkStream = _clientSocket.GetStream();
		    try
		    {
		        int read = networkStream.EndRead(result);
		        if (read == 0)
		        {
		            _networkStream.Close();
		            _clientSocket.Close();
		            return;
		        }

		        byte[] buffer = result.AsyncState as byte[];
		        string data = Encoding.Default.GetString(buffer, 0, read);

		        Debug.Log("Data arrived:"+data);
		        processServerMessage(data);

		        //test echo
		        if(false)
		        {
		          Byte[]  sendBytes = Encoding.ASCII.GetBytes("Processed: " + data);
		          networkStream.Write(sendBytes, 0, sendBytes.Length);
		          networkStream.Flush();
		      }
		    }
		    catch (Exception ex)
		    {
		        throw;
		    }

		    this.WaitForRequest();
		}

	}

//------------------------------------------------------------------------------
	class TCPServer
	{

		private string ipAddress;
		private int port;
		private ProcessServerFunction processServerMessage;

		private System.Net.IPAddress localIPAddress;
		private IPEndPoint ipLocal;

		private TcpListener _listener;

		public TCPServer(string addr, int p, ProcessServerFunction func)
		{
			ipAddress = addr;
			port = p;
			processServerMessage = func;

			localIPAddress = System.Net.IPAddress.Parse(ipAddress);
			ipLocal = new IPEndPoint(localIPAddress, port);
		}

//------------------------------------------------------------------------------
		public void Start()
		{
		    // System.Net.IPAddress localIPAddress = System.Net.IPAddress.Parse(ipAddress);
		    //System.Net.IPAddress localIPAddress = 
			// Dns.Resolve(Dns.GetHostName()).AddressList[0]; --> WRONG!! any network card could be here!!
			// localIPAddress = System.Net.IPAddress.Parse(ipAddress);

		    Debug.Log("ip:"+localIPAddress);

		    // ipLocal = new IPEndPoint(localIPAddress, port);
		    _listener = new TcpListener(ipLocal);
		    _listener.Start();
		    WaitForClientConnect();
		}

		public void Stop()
		{
			_listener.Stop();
			ipLocal = null;
		}

		private void WaitForClientConnect()
		{
		    object obj = new object();
		    _listener.BeginAcceptTcpClient(
				new System.AsyncCallback(OnClientConnect), obj);
		}

		private void OnClientConnect(IAsyncResult asyn)
		{
		    try
		    {
		        TcpClient clientSocket = default(TcpClient);
		        clientSocket = _listener.EndAcceptTcpClient(asyn);
		        HandleClientRequest clientReq = 
					new HandleClientRequest(clientSocket, processServerMessage);
		        clientReq.StartClient();
		    }
		    catch (Exception se)
		    {
		        throw;
		    }

		    WaitForClientConnect();
		}
	}

#else //!UNITY_MOBILE_FREE
	
	public class TcpListener
	{
		public TcpListener(IPEndPoint a) {}
		public void Start() {}
		public void Stop() {}
		public void BeginAcceptTcpClient(AsyncCallback a, object o) {}
		public TcpClient EndAcceptTcpClient(IAsyncResult a) { return null; }
	}

#endif //!UNITY_MOBILE_FREE

}//namespace
//------------------------------------------------------------------------------