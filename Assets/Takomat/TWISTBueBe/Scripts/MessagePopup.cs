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

using Epigene;
using Epigene.UI;
using Epigene.MODEL;
using Epigene.GAME;


namespace TWISTBueBe
{
	/// <summary>
	/// Message popup to show a message
	/// </summary>
	public class MessagePopup : MonoBehaviour 
	{
		
		/// <summary>
		/// holder for visual content
		/// </summary>
		public MessagePopups messagePopup;
		
		/// <summary>
		/// holder for visual content
		/// </summary>
		public GameObject popup;

		/// <summary>
		/// text of answer 1
		/// </summary>
		public UIText txtMessage;
		
		/// <summary>
		/// check box for answer 1
		/// </summary>
		public Button btnOk;
		private UIText txtbtnOk;
		
		/// <summary>
		/// Id for this popup message
		/// </summary>
		private string id;
		
		/// <summary>
		/// UI manager
		/// </summary>
		private UIManager uiManager;
		
		/// <summary>
		/// Localization manager
		/// </summary>
		private I18nManager i18n;
		
		
		/// <summary>
		/// Initializa main components
		/// </summary>		
		void Awake () 
		{

			Log.Assert(popup, "Please assign the popup in "+gameObject.name);
			Log.Assert(txtMessage, "Please assign the txtMessage in "+gameObject.name);
			Log.Assert(btnOk, "Please assign the btnOk in "+gameObject.name);
			
			
			uiManager = UIManager.Instance;
			i18n = I18nManager.Instance;
			messagePopup = MessagePopups.Instance;
			GameObject obj = null;

			popup = GetChildObject("popup");
			obj = GetChildObject("popup/txt_Message");
			txtMessage = obj.GetComponent<UIText>();
			obj = GetChildObject("popup/btnOk");
			btnOk = obj.GetComponent<Button>();
			obj = GetChildObject("popup/btnOk/txt_btnOk");
			txtbtnOk = obj.GetComponent<UIText>();
			txtbtnOk.Text = I18nManager.Instance.Get("POPUP_BUTTON", "0031");
			id = "001";

			Hide();
			
			GameManager.Instance.RegisterEventHandler("POPUP_MESSAGE", EventHandler, gameObject);
			
		}//Awake()
	
		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDestroy()
		{
			GameManager.Instance.RemoveEventHandler("POPUP_MESSAGE", EventHandler);
		}
		
		/// <summary>
		/// Show the message by id
		/// </summary>
		public void Show(string id)
		{
			if (i18n.Contains("POPUP_MESSAGE", id))
				txtMessage.Text = i18n.Get("POPUP_MESSAGE", id);
			else
				txtMessage.Text = id;
			
			popup.SetActive(true);
			this.id = id;
			UIManager.Instance.ShowModal(popup);
			
		}//Show()
		
		/// <summary>
		/// Hide the popup
		/// </summary>
		public void Hide()
		{
			popup.SetActive(false);			
		}//Hide()
		
		/// <summary>
		/// Handle buttons
		/// </summary>
		public void OnClick(string button)
		{
			if(button == "btnOk")
			{
				if(messagePopup.xClick == false)
					messagePopup.xClick = true;
				
				GameManager.Instance.Event("POPUP_MESSAGE", id, "ok");
				Hide();
			}
			
		}//OnClick()
		
		
		/// <summary>
		/// Helper to get a child object 
		/// and report errors if needed
		/// </summary>
		GameObject GetChildObject(string name)
		{
			GameObject obj = GameObject.Find(gameObject.name+"/"+name);
			Log.Assert(obj, "Could not find object for:"+name);
			return obj;
			
		}//GetChildObject();
		
		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void EventHandler(string eventId, string param)
		{
			Log.Info("EVENT: "+eventId+" ,"+param);
			
			// Log.GameTimes("MessagePopup eventHandler:" + param);
			//add new mail
			if(param == "show")
			{
				if(eventId == "001" && messagePopup.xClick == true)
				{
					//don't show
				}
				else
					Show(eventId);
			}
			else if(param == "hide")
			{
				Hide();
			}
			
		}//EventHandler()


	}//class DecisionPopup
}//namespace