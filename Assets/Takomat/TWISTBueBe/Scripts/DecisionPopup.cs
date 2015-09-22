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
	/// Decision popup to manage yes/no answer
	/// </summary>
	public class DecisionPopup : MonoBehaviour 
	{
		
		/// <summary>
		/// holder for visual content
		/// </summary>
		public DecisionPopups decisionPopup;

		/// <summary>
		/// holder
		/// </summary>
		public GameObject popup;

		/// <summary>
		/// message text
		/// </summary>
		public UIText txtMessage;
		
		/// <summary>
		/// check box for yes
		/// </summary>
		public Button btnYes;
		
		/// <summary>
		/// check box for no
		/// </summary>
		public Button btnNo;
		
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
		/// set up basic components
		/// </summary>
		void Awake () 
		{
			Log.Assert(txtMessage, "Please assign the txtMessage in "+gameObject.name);
			Log.Assert(popup, "Please assign the popup in "+gameObject.name);
			
			
			uiManager = UIManager.Instance;
			i18n = I18nManager.Instance;
			decisionPopup = DecisionPopups.Instance;
			
			GameObject obj = null;
			
			popup = GetChildObject("popup");
			obj = GetChildObject("popup/txt_Message");
			txtMessage = obj.GetComponent<UIText>();
			obj = GetChildObject("popup/btn_Yes");
			btnYes = obj.GetComponent<Button>();
			obj = GetChildObject("popup/btn_Yes/Text");
			UIText btnYesText = obj.GetComponent<UIText>();
			btnYesText.Text = I18nManager.Instance.Get("POPUP_BUTTON", "001");
			obj = GetChildObject("popup/btn_No");
			btnNo = obj.GetComponent<Button>();
			obj = GetChildObject("popup/btn_No/Text");
			UIText btnNoText = obj.GetComponent<UIText>();
			btnNoText.Text = I18nManager.Instance.Get("POPUP_BUTTON", "002");
			id = "001";

			Hide();

			GameManager.Instance.RegisterEventHandler("POPUP_DECISION", EventHandler);

		}//Awake()

		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDestroy()
		{
			GameManager.Instance.RemoveEventHandler("POPUP_DECISION", EventHandler);
		}


		/// <summary>
		/// Show the message by id
		/// </summary>
		public void Show(string id)
		{
			string storyId = DialogManager.Instance.DialogView.StoryId;
			if(storyId.StartsWith("CP1_5_") || storyId.StartsWith("CP1_6_"))
			{
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
			//TODO get message
			//txtMessage.Text = i18n.Get(id);
			txtMessage.Text = I18nManager.Instance.Get("POPUP_DECISION", id);

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
			if(button == "btn_Yes")
			{
				//Log.Debug("BTN YES!");
				GameManager.Instance.Event("POPUP_DECISION", id, "yes");
				Hide();
			}
			else if( button == "btn_No")
			{
				//Log.Debug("BTN NO!");
				GameManager.Instance.Event("POPUP_DECISION", id, "no");
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
			//Log.Info("EVENT: "+eventId+" ,"+param);

			//add new mail
			if(param == "show")
			{
				Show(eventId);
			}
			else if(param == "hide")
			{
				Hide();
			}

		}//EventHandler()


	}//class DecisionPopup
}//namespace