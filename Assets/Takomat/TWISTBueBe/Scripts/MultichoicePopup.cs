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
using Epigene.GAME;



namespace TWISTBueBe
{
	/// <summary>
	/// Popup box for multiple choices using checkboxes.
	/// </summary>
	public class MultichoicePopup : MonoBehaviour 
	{

		/// <summary>
		/// holder for visual content
		/// </summary>
		public GameObject popup;
		/// <summary>
		/// text of popup title
		/// </summary>
		public UIText txtTitle;
		/// <summary>
		/// text of answer 1
		/// </summary>
		public UIText txt1;
		/// <summary>
		/// text of answer 2
		/// </summary>
		public UIText txt2;
		/// <summary>
		/// text of answer 3
		/// </summary>
		public UIText txt3;

		/// <summary>
		/// check box for answer 1
		/// </summary>
		public Button btnCheck1;
		/// <summary>
		/// check box for answer 2
		/// </summary>		
		public Button btnCheck2;
		/// <summary>
		/// check box for answer 3
		/// </summary>
		public Button btnCheck3;

		/// <summary>
		/// Id for this popup message
		/// </summary>
		private string id;

		/// <summary>
		/// stores last selected answer
		/// </summary>
		private int selectedSlot;



		/// <summary>
		/// Initializa main components
		/// </summary>		
		void Awake () 
		{
			Log.Assert(popup, "Please assign the popup in "+gameObject.name);
			Log.Assert(txtTitle, "Please assign the txtTitle in "+gameObject.name);
			Log.Assert(txt1, "Please assign the txt1 in "+gameObject.name);
			Log.Assert(txt2, "Please assign the txt2 in "+gameObject.name);
			Log.Assert(txt3, "Please assign the txt3 in "+gameObject.name);
			Log.Assert(btnCheck1, "Please assign the btnCheck1 in "+gameObject.name);
			Log.Assert(btnCheck2, "Please assign the btnCheck2 in "+gameObject.name);
			Log.Assert(btnCheck3, "Please assign the btnCheck3 in "+gameObject.name);



			Hide();			
		
			GameManager.Instance.RegisterEventHandler("POPUP_MULTI", EventHandler, gameObject);

		}//Awake()


		
		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDestroy()
		{
			GameManager.Instance.RemoveEventHandler("POPUP_MULTI", EventHandler);
		}

		/// <summary>
		/// Show the message by id
		/// </summary>
		public void Show(string id)
		{

			Log.Info("SHOW ID:"+id);

			//TODO get message
			//txtMessage.Text = i18n.Get(id);
			txtTitle.Text = id;

			txt1.Text = id + ": text1";
			txt2.Text = id + ": text1";
			txt3.Text = id + ": text1";

			//set checkbox status
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
			if(button == "btn_FinishMP")
			{				
				Hide();
			}
			else if( button == btnCheck1.Name)
			{
				Log.Debug("BTN CHECK 1");
				Select(1);
				btnCheck1.UIButton.Sprite = 5;
			}
			else if( button == btnCheck2.Name)
			{
				Log.Debug("BTN CHECK 2");
				Select(2);
			}
			else if( button == btnCheck3.Name)
			{
				Log.Debug("BTN CHECK 3");
				Select(3);
			}
		}//OnClick()

		/// <summary>
		/// Select which answer should be highlighted.
		/// </summary>
		public void Select(int selectedId)
		{
			//select the id
			//TODO
			//Show("selected: "+id);
			
			selectedSlot = selectedId;
			
			switch(selectedSlot)
			{
				case 1:
					btnCheck1.UIButton.State = UIButtonState.DISABLED;
					btnCheck2.UIButton.State = UIButtonState.RELEASE;
					btnCheck3.UIButton.State = UIButtonState.RELEASE;
					break;

				case 2:
					btnCheck1.UIButton.State = UIButtonState.RELEASE;
					btnCheck2.UIButton.State = UIButtonState.DISABLED;
					btnCheck3.UIButton.State = UIButtonState.RELEASE;
					break;

				case 3:
					btnCheck1.UIButton.State = UIButtonState.RELEASE;
					btnCheck2.UIButton.State = UIButtonState.RELEASE;
					btnCheck3.UIButton.State = UIButtonState.DISABLED;
					break;										
			}

			GameManager.Instance.Event("POPUP_MULTI", id, selectedSlot.ToString());
			

		}//Select()

		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void EventHandler(string eventId, string param)
		{

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


	}//class MultichoicePopup

}//namespace
