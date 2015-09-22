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

using UnityEngine;
using System.Collections;
using Epigene.GAME;
using Epigene.IO;
using Epigene.UI;
using Epigene.MODEL;
using System.Collections.Generic;

namespace TWISTBueBe
{
	public class EventSettings : MonoBehaviour
	{

		public class Attribute
		{
			public int 	  id;
			public string label;
			public double value;
			public string unit;
			
			/// <summary>
			/// get formatted text by value
			/// </summary>
			public string text
			{
				get { return (this.value.ToString("N1") + " " + unit); }
			}
			
			public string labelledText
			{
				get { return (label + ": " + this.value.ToString("N1") + " " + unit); }
			}
			//------------------------------------------------------------------------------
			
			public Attribute(int id)
			{
				this.id  = id;
				label = "";
				value = 0;
				unit  = "";
			}
		}
		
		public List<Attribute>  att = new List<Attribute>();
		public string			name 	 			= "";
		public string			label 	 			= "";
		public string			dataPath 			= "";
		public string			dataPathStandalone	= "";
		public bool				initialized			= false;

		// Use this for initialization
		void Start () {

		}

		public void Load()
		{
			if (name == "") return;
			string fullPath = "";
			#if UNITY_EDITOR
			fullPath = "file://" + Application.dataPath + "/Resources/ExternalFiles" + dataPath;
			#elif UNITY_STANDALONE
			if (GameManager.Instance.standaloneBetaMode)
			{
				fullPath = "file://" + Application.dataPath + dataPath;
			}
			else
			{
				TextAsset download = Resources.Load("ExternalFiles"+dataPathStandalone) as TextAsset;
				
				Initialize (download.text);
				return;
			}
			#elif UNITY_WEBPLAYER
			fullPath = Application.dataPath + dataPath;
			#endif
			
			DBModuleManager.Instance.Event(
				"CONFIG",
				fullPath,
				name);
		}
		
		public void Initialize(string dataString)
		{
			
			Dictionary<string,object> dict = 
				MiniJSON.Json.Deserialize(dataString) as Dictionary<string,object>;
			
			foreach(Dictionary<string,object> obj
			        in ((List<object>)dict["Attributes"]))
			{
				if(obj.ContainsKey("id"))
				{
					Attribute newAtt = new Attribute(int.Parse(obj["id"].ToString()));
					if(obj.ContainsKey("label"))
						newAtt.label = obj["label"].ToString();
					if(obj.ContainsKey("value"))
						newAtt.value = double.Parse(obj["value"].ToString());
					if(obj.ContainsKey("unit"))
						newAtt.unit = obj["unit"].ToString();
					
					att.Add(newAtt);
				}
			}
			
			Initialized();
		}
		
		public virtual void Initialized()
		{
			WebPlayerDebugManager.addOutput(name + " initialized.", 1);
		}
		
		public void ProcessConfigEvent(string eventId, string data)
		{
			if (eventId == name)
			{
				if (data == "Restart")
					Restart();
				else
					Initialize(data);
			}
		}
		
		public void Restart()
		{
			initialized = false;
			att.Clear();
			Load ();
		}
	}
}
