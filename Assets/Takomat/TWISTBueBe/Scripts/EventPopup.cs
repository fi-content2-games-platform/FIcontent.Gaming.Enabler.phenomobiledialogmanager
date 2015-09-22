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
using Epigene.GAME;
using Epigene.UI;


namespace TWISTBueBe
{
	/// <summary>
	/// Event Popup 
	/// use it with the layer "popup" for now
	/// Usage:
	/// 1. add a link to your screen 
	///   public Popup popup;
	/// 2. link it to Objects/EventPopup manually in the editor (drag'n'drop)	
	/// 3. then call it from your script like this:
	///  popup.SetText("mytype", "title", "here comes the message");
	///  popup.Show(true);
	/// 4. it wil hide itself once clicked on button or you can hide it from script:
	///  popup.Show(false);

	/// </summary>
	public class EventPopup : MonoBehaviour 
	{

		/// <summary>
		/// main popup for each object
		/// </summary>
		public GameObject popup;
		/// <summary>
		/// text for the type
		/// </summary>
		public UIText txtType;
		/// <summary>
		/// title of this popup
		/// </summary>
		public UIText txtTitle;
		/// <summary>
		/// text of the popup
		/// </summary>
		public UIText txtMessage;
		/// <summary>
		/// background image of the popup
		/// </summary>
		public GameObject bgImage;
		/// <summary>
		/// Close button
		/// </summary>
		public GameObject button;

		/// <summary>
		/// Localization manager
		/// </summary>
		private I18nManager i18n;

		/// <summary>
		/// Check components
		/// </summary>
		void Awake()
		{
			Log.Assert(popup, "Missing popup holder!" +  gameObject.name);
			Log.Assert(txtTitle, "Missing txtTitle!" +  gameObject.name);
			Log.Assert(txtType, "Missing txtType!" +  gameObject.name);
			Log.Assert(txtMessage, "Missing txtMessage!" +  gameObject.name);
			Log.Assert(bgImage, "Missing bgImage!" +  gameObject.name);
			Log.Assert(button, "Missing button!" +  gameObject.name);

			Show(false);
			GameManager.Instance.RegisterEventHandler("EVENT_POPUP", EventHandler, gameObject);
			i18n = I18nManager.Instance;
		}//Awake()

		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDestroy()
		{
			GameManager.Instance.RemoveEventHandler("EVENT_POPUP", EventHandler);
		}

		/// <summary>
		/// registration of the ui
		/// </summary>
		void OnEnable()
		{
			//Show(true);

		}//OnEnable()

		/// <summary>
		/// un-register ui
		/// </summary>
		void OnDisable()
		{
			Show(false);
		}//OnDisable()

		/// <summary>
		/// Set text of the popup
		/// </summary>
		public void SetText(string type, string title, string message)
		{
			txtType.Text = type;
			txtTitle.Text = title;
			txtMessage.Text = message;

			//? force Show(true);
		}//SetText()

		/// <summary>
		/// Show or hide the popup
		/// </summary>
		public void Show(bool flag)
		{
			popup.SetActive(flag);
			/*button.SetActive(flag);
			bgImage.SetActive(flag);
			txtType.gameObject.SetActive(flag);
			txtTitle.gameObject.SetActive(flag);
			txtMessage.gameObject.SetActive(flag);*/
			if (flag)	
				UIManager.Instance.ShowModal(popup);

		}//Show()

		/// <summary>
		/// hanlder for the button
		/// </summary>
		public void OnClick(string buttonName)
		{
			Log.Info("Popup button clicked: "+buttonName);
			GameManager.Instance.Event ("TIMELINE",
			                            "SIM",
			                            "Unpause");
			Show(false);

		}//OnClick()

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
				if(eventId == "001")
				{
					//don't show
				}
				else
				{
					if ((i18n.Contains("EventPopup", eventId)) && 
					    (i18n.Contains("EventPopup", (int.Parse(eventId)+1).ToString("D3"))))
					{
						/*WebPlayerDebugManager.addOutput
							("Get EventPopup " + eventId +
							 " and " + (int.Parse(eventId)+1).ToString("D3"), 1);*/
						SetText(i18n.Get("EventPopup", "001"),
						        i18n.Get("EventPopup", eventId),
						        i18n.Get("EventPopup", (int.Parse(eventId)+1).ToString("D3")));
					}
					else
						SetText(i18n.Get("EventPopup", "001"),
						        "",
						        "");

					GameManager.Instance.Event ("TIMELINE",
					                            "SIM",
					                            "Pause");
					Show(true);
				}
			}
			else if(param == "hide")
			{
				GameManager.Instance.Event ("TIMELINE",
				                            "SIM",
				                            "Unpause");
				Show(false);
			}
			
		}//EventHandler()

	}//class EventPopup
}//namespace