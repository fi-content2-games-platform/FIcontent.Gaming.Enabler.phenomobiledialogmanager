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
using System.Collections;

using Epigene;

namespace Epigene.Network
{
	
	public delegate void ProcessServerFunction(string data);

	/// <summary>
	/// Network manager
	/// </summary>
	public sealed class NetworkManager
	{


		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static NetworkManager Instance
		{
			get { return instance; }
		}
		private static readonly NetworkManager instance = new NetworkManager();
		
		/// <summary>
		/// type of networking to use
		/// </summary>
		public enum Protocol {UDP, TCP, HTTP};
		private Protocol type;

		private TCPClient tcpClient = null;
		private TCPServer tcpServer = null;
		private HTTP http = null;

		private string gameServerIP;	//this is the address of the game server
		private string myIP; 			//this is the address of this machine
		private int listenerPort;		//aplication port to receive data from server
		private int serverPort;			//server port to send data

		public 	string 	GameServerIP { get {return gameServerIP; }}
		public 	string 	MyIP 		 { get {return myIP; }}
		public 	int 	ListenerPort { get {return listenerPort; }}
		public 	int 	ServerPort 	 { get {return serverPort; }}

		private ProcessServerFunction processServerMessage;

		public bool Connected
		{
			get{ 

				switch (this.type)
				{
					case Protocol.TCP:
						if(tcpClient == null)
						{
							return false;
						}
						return tcpClient.Connected;

					case Protocol.HTTP:
						return http != null;
				}
				return false;
			}
		}
		//private bool connected = false;

		// private UDO udp = null;

		/// <summary>
		/// constructor
		/// </summary>
		private NetworkManager()
		{
			Debug.Log("Network init");
		}//ctor()


		/// <summary>
		/// Connect to server to able to send message to server
		/// and create listener running in background for receiving messages.
		/// The protocol will define what type of network mode you want to use: TCP/HTTP/..
		/// Return true if connected or false if not
		/// </summary>
		public bool Connect(string myIP, 
		                    string gameServerIP, 
		                    int serverPort, 
		                    int listenerPort, 
		                    Protocol type, 
		                    ProcessServerFunction func)
		{
			//TODO in case of active conection Connected == true: 
			//should we release actual connection or return with error?

			if(Connected)
			{
				//we are already connected, don't brake it
				return true;
			}

			this.myIP = myIP;
			this.gameServerIP = gameServerIP;
			this.serverPort = serverPort;
			this.listenerPort = listenerPort;
			this.type = type;
			this.processServerMessage = func;
			

			switch (this.type)
			{
				case Protocol.TCP:
					Debug.Log("Starting TCP client connection");

					if(tcpClient == null)
					{
						tcpClient = new TCPClient(this.gameServerIP, this.serverPort);
						tcpClient.Start();
						
					}

					//create only once
					if(tcpServer == null)
					{
						Debug.Log("Starting TCP server at "+this.myIP+":"+this.listenerPort);
						tcpServer = new TCPServer(this.myIP, this.listenerPort, func);
						tcpServer.Start();
					}
					break;

				case Protocol.HTTP:
					http = new HTTP("http://192.168.1.104:2000");
					break;

				// case Protocol.UDP:
				// 	break;

				default:
					Debug.LogWarning("Not implemented yet!!");
					break;
			}

			if (tcpClient != null)
			{
				WebPlayerDebugManager.addOutput("Connected!", 4);
				Epigene.GAME.GameManager.Instance.Event(
					"NETWORKLAYER",
					"connected",
					"true");
			}

			return tcpClient != null;

		}

		public void Disconnect()
		{
			if(tcpServer != null)
			{
				tcpServer.Stop();
				tcpServer = null;
			}
			if(tcpClient != null)
			{
				tcpClient.Stop();
				tcpClient = null;
			}

			Epigene.GAME.GameManager.Instance.Event(
				"NETWORKLAYER",
				"connected",
				"false");
		}

		public byte[] Send(byte[] data)
		{
			byte[] response = null;

			if(!Connected)
			{
				//skip if not connected
				return null;
			}

			switch (this.type)
			{
				case Protocol.TCP:
					tcpClient.Send(data);
					break;

				case Protocol.HTTP:
					response = http.Send(data);
					break;

				// case Protocol.UDP:
				// 	break;

				default:
					Debug.LogWarning("Not implemented yet!!");
					break;
			}
			return response;
		}

	}//class NetworkManager

}//namespace