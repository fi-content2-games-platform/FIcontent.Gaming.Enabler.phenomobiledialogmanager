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
using System.Collections.Generic;
using UnityEngine;
using Epigene;
using System.IO;
using System.Xml;
using Epigene.GAME;
//------------------------------------------------------------------------------

namespace Epigene.IO
{
	
	public sealed class IPAppConfiguration 
	{
		public string serverName 	= "";
		public string serverIp		= "";
		public string serverPort	= "";
		public string myIp			= "";
		public string myPort		= "";
		public string headerFlag    = "";
		public bool	  initialized	= false;
		
		//------------------------------------------------------------------------------
		
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static IPAppConfiguration Instance
		{
			get{ return instance;}
		}
		static readonly IPAppConfiguration instance = new IPAppConfiguration();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Epigene.IO.AppConfiguration"/> class.
		/// </summary>
		IPAppConfiguration()
		{			
			GameManager.Instance.RegisterEventHandler("CONFIG", ProcessConfigEvent);
		}//AppConfiguration()

		public void Load(string dataPath)
		{
			Log.Debug("IPAppConfiguration initialized. Loading JSON...");
			#if UNITY_IOS
			TextAsset download = Resources.Load("ExternalFiles/assets/IPAppConfiguration") as TextAsset;
			Initialize (download.text);
			return;
			#endif
			string dataPathStandalone = dataPath.Remove(dataPath.LastIndexOf("."));
			string fullPath = "";
			#if UNITY_EDITOR 
			// For requested XML with Editor
			fullPath = "file://" + Application.dataPath + "/Resources/ExternalFiles/assets/" + dataPath;
			#elif UNITY_STANDALONE
			// For requested JSON with Standalone
			fullPath = "file://" + GameManager.Instance.mainGame.ExternalFilesPath + "/"+ dataPath;
			#elif UNITY_WEBPLAYER
			// For requested JSON with web player
			fullPath = Application.dataPath + dataPath;
			#endif

			DBModuleManager.Instance.Event(
				"CONFIG",
				fullPath,
				"IPAppConfig");
		}

		public void ProcessConfigEvent(string eventId, string data)
		{
			if (eventId == "IPAppConfig")
				Initialize(data);
		}

		public void Initialize(string dataString)
		{	
			Dictionary<string,object> dict = 
				MiniJSON.Json.Deserialize(dataString) as Dictionary<string,object>;
			
			foreach(Dictionary<string,object> obj
			        in ((List<object>)dict["IPAppConfiguration"]))
			{

				if(obj.ContainsKey("id"))
				{
					WebPlayerDebugManager.addOutput( obj["value"].ToString(), 1);
					switch (obj["id"].ToString())
					{
						case "serverName":
							serverName = obj["value"].ToString();
							break;
						case "serverIp":
							serverIp = obj["value"].ToString();
							break;
						case "serverPort":
							serverPort = obj["value"].ToString();
							break;
						case "myIp":
							myIp = obj["value"].ToString();
							break;
						case "myPort":
							myPort = obj["value"].ToString();
							break;
						case "useHeader":
							headerFlag = obj["value"].ToString();
							break;	
					}
				}
			}
			initialized = true;
			Debug.Log ("Send out------- IPCONFIG");
			WebPlayerDebugManager.addOutput("IPAppConfiguration loaded.",1);
			GameManager.Instance.Event("IPCONFIGED", "" ,"");
			GameManager.Instance.Event("CONFIG", "IPAPP_CONFIGURATION_LOADED" ,"");
		}
	}// IPAppConfiguration
	
}// Epigene.IO
