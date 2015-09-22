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
using System.Collections.Generic;
using System.Threading;

using Epigene;
using Epigene.IO;


namespace Epigene.Network
{
	public delegate void ProcessServerData(Dictionary<string, object> dict);

	public class NetworkLayer : MonoBehaviour 
	{
		public  string myIP 		= "";	//my local computer where unity runs
		public  string gameServerIP	= "";	//ip of game server
		public  int serverPort		= 0;
		public  int listenerPort	= 0;
		public  ProcessServerData processServerData;	//delegate to process data in game (thread safe)
		public	bool headerFlag		= false;

		protected NetworkManager networkManager;
		protected Mutex dataMutex = null;
		protected int mutexTime = 1000;
		protected bool dataFlag;

		protected bool connected = false;

		protected List<Dictionary<string, object>> arrivedDataList;

		/// <summary>
		/// Setup the data packet header to game command mode (0x3031)
		/// </summary>	
		public virtual void Awake () 
		{

			arrivedDataList = new List<Dictionary<string, object>>();
			dataMutex = new Mutex(false, "Epigene.NetworkLayer.dataMutex");
			dataFlag = false;
			Debug.Log ("Hallo NetworkLayer " + IPAppConfiguration.Instance.serverIp);
		}

		public virtual void OnEnable() 
		{
			Debug.Log(">>>>>>>>>> Net Connection >>>>>>>>>>>"+ gameServerIP + ":"+serverPort+" "+ myIP + ":"+ listenerPort);
			networkManager = NetworkManager.Instance;
			connected = networkManager.Connect(
				myIP, gameServerIP, 
			    serverPort, listenerPort, NetworkManager.Protocol.TCP, ProcessServerMessage);

			Debug.Log( (connected) ? "Connected to server!" : "Can't connect to server!");

			if(!connected)
			{
				//disable this object since no connection available
				gameObject.SetActive(false);
			}

		
		}

		public virtual void OnDisable()
		{
			Debug.Log("Disconnect from network!");
			networkManager.Disconnect();
		}

		/// <summary>
		/// Check if new data is arrived 
		/// and notify the game in a thread safe way
		/// </summary>
		public virtual void Update()
		{
			// //check if new data arrived
			
			if(!connected)
				return;


			//lock & read
			Dictionary<string, object> dict = null;
			try
			{
				if(dataMutex.WaitOne(mutexTime))
				{
					if(dataFlag && arrivedDataList.Count > 0)
					{
						dict = arrivedDataList[0];
						arrivedDataList.RemoveAt(0);
						LogDict(dict);				
					}
					if (arrivedDataList.Count <= 0)
						dataFlag = false;
				}
			}
			finally
			{
				dataMutex.ReleaseMutex();
			}
			
			//notify game
			if(dict != null && processServerData != null)
			{
				processServerData(dict);
			}
		}

		/// <summary>
		/// Send filter plants command to server
		/// TODO finalize this!
		/// </summary>
		public void AddFilterPlant(int fieldId1, int fieldId2)
		{
			// example 2.1.2 Calculation pipeline:
			// var  gameChangeData =
			// {
			// 	"command": "addedSmallFilterPlants",
			// 	"valueName": "FilterPlant",
			// 	"fieldId1": "1",
			// 	"fieldId2": "2"
			// }a

			if(!connected)
				return;
			Debug.Log("Send gamedata to server: AddFilterPlant");

			DataPacket data = new DataPacket();
			if((data.packet_header != null) && (headerFlag))
			{
				data.packet_header.command = 0x3033; //TODO: SetCommand(enum)
			}
			data.SetValue("command", "addedSmallFilterPlants");
			data.SetValue("valueName", "FilterPlant"); 
			data.SetValue("fieldId1", ""+fieldId1);
			data.SetValue("fieldId2", ""+fieldId2);
			data.SetValue("listenerPort", listenerPort.ToString());

			//create packet
			byte[] packet = data.Serialize();
			Debug.Log("packet: " + System.Text.Encoding.Default.GetString(packet));

			//TODO add to "stack" and guard
			networkManager.Send(packet);
		}

		public virtual void ProcessServerMessage(string data)
		{
			
			Debug.Log(">>>>>>>>>>> DATA ARRIVED FROM SERVER >>>>>>>>>>>>>");
			Debug.Log(data);
			Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

			Dictionary<string,object> dict = 
				MiniJSON.Json.Deserialize(data) as Dictionary<string,object>;

			try
			{
				if(dataMutex.WaitOne(mutexTime))
				{
					arrivedDataList.Add(dict);
					dataFlag = true;
				}
			}
			finally
			{
				dataMutex.ReleaseMutex();
			}

		}
		/// <summary>
		/// helper to log the content of dict
		/// </summary>
		public void LogDict(Dictionary<string, object> dict)
		{
			foreach(KeyValuePair<string, object> entry in dict)
			{
				Debug.Log(entry.Key + "=" + entry.Value.ToString());
			}
		}
	}
}//namespace