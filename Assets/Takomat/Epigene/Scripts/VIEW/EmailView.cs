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


using Epigene;
using Epigene.UI;
using Epigene.MODEL;
using Epigene.GAME;


namespace Epigene.VIEW 
{

	/// <summary>
	/// VIEW representation of Emails
	/// Emails are stored in Emails class (MODEL) and here 
	/// we only visualize them.
	/// </summary>
	public class EmailView : MonoBehaviour 
	{

		/// <summary>
		/// Refernce to Emails model
		/// </summary>
		private Emails emails;

		/// <summary>
		/// emails button
		/// </summary>
		//public UIButton btnEmails;
		public Button btnEmails;

		/// <summary>
		/// parent game object of inbox panel
		/// </summary>
		private GameObject inbox;
		
		/// <summary>
		/// parent game object of mail message panel
		/// </summary>
		private GameObject mail;

		/// <summary>
		/// UI manager
		/// </summary>
		private UIManager uiManager;

		/// <summary>
		/// Localization manager
		/// </summary>
		private I18nManager i18n;

		/// <summary>
		/// list of buttons per email
		/// </summary>
		private List<UIButton> buttons;

		/// <summary>
		/// uitext holds Date of the mail
		/// </summary>
		private UIText mailDate;
		/// <summary>
		/// uitext holds subject of the mail
		/// </summary>
		private UIText mailSubject;
		/// <summary>
		/// uitext holds message of the mail
		/// </summary>
		private UIText mailMessage;

		/// <summary>
		/// number of mails since last update
		/// </summary>
		private int lastNumber;

		//TODO add text colors for buttons
		private Color upColor = new Color32(103,105,107,255);
		private Color downColor = new Color32(103,105,107,255);
		private Color overColor = new Color32(103,105,107,255);
		private Color disabledColor = new Color32(103,105,107,255);
		private Color activeColor = new Color32(103,105,107,255);

		private float messageButtonPosY = 1.58f;


		private List<Mail> readMails;


		
		/// <summary>
		/// Initialize the components
		/// </summary>
		public virtual void Awake () 
		{
			//Log.GameTimes ("EmailView Awake");

			uiManager = UIManager.Instance;
			i18n = I18nManager.Instance;
			emails = Emails.Instance;


			GameObject obj = null;

			inbox = GetChildObject("Inbox");
			mail = GetChildObject("Mail");
			obj = GetChildObject("Mail/txt_Date");
			mailDate = obj.GetComponent<UIText>();
			obj = GetChildObject("Mail/txt_Subject");
			mailSubject = obj.GetComponent<UIText>();
			obj = GetChildObject("Mail/txt_Message");
			mailMessage = obj.GetComponent<UIText>();

			obj = GetChildObject("Mail/btn_Close_Mail/Text"); 
			UIText btnCloseText = obj.GetComponent<UIText>(); 
			btnCloseText.Text = I18nManager.Instance.Get("POPUP_BUTTON", "003");

			buttons = new List<UIButton>();
			readMails = new List<Mail>();

			
		}//Awake()
		//------------------------------------------------------------------------------
		

		/// <summary>
		/// Init basic components
		/// </summary>
		void Start()
		{

			lastNumber = 0;
			Show(false);

			GameManager.Instance.RegisterEventHandler("EMAIL", ProcessEmailEvent);

		}//Start()

		/// <summary>
		/// Disable emailview
		/// </summary>
		void OnDestroy()
		{
			GameManager.Instance.RemoveEventHandler("EMAIL", ProcessEmailEvent);
		}

				
		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void ProcessEmailEvent(string eventId, string param)
		{
			//add new mail
			if((param == "add")||(param == "addView"))
			{
				if(param == "add")
				{
					SendMail(eventId);
				} 
				else
				{
					ViewMail(eventId);
				}
				UpdateMails();
				
				//blink if new mails
				uiManager.BlinkImage(btnEmails.UIButton, 3, 500);
			}

		}


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



		private UIToggleButton CreateButtonSubject(string subject)
		{
			//TODO only create buttons for visible one
			//based on slider value m.id >= slider.value
			//Log.GameTimes ("Create Button + "+ i);
			UIToggleButton b = CreateButton(emails.Mails.Count-1);
			b.Text = subject;//.Substring(0,20);
			buttons.Add(b);

			return b;
		}

		/// <summary>
		/// Add an email from localization
		/// </summary>
		private void SendMail(string id)
		{
			//emails.Add(new Mail("Subject 1", "TEXT1: bla bla bladibladi ba.", "31. SEPTEMBER", "sender"));
			//// Log.GameTimes("SendMail+ " +id);
			string subject = i18n.Get(id, "001");
			string message = i18n.Get(id, "002");
			int lastcount = emails.Mails.Count;
			emails.Add(id, subject, message);
			if(lastcount<emails.Mails.Count)
			{
				CreateButtonSubject(subject);
			}
		}//SendMail()


		private void ViewMail(string id)
		{
			//// Log.GameTimes("ViewMail + " +id);
			Mail m = emails.GetWithId(id);
			if(m!=null)
			{
				//// Log.GameTimes("ViewMail + " +id);
				UIToggleButton button = CreateButtonSubject(m.subject);
				button.Active = m.isReaded;
				//TODO? button.Freez();
			}
		}//SendMail()



		/// <summary>
		/// Update buttons to match with mails.
		/// </summary>
		public void UpdateMails()
		{
			// this is needed, because  so all emails are viewed!!
			// otherwise there is blank line at a not viewed email
			if(lastNumber != emails.Mails.Count)
			{
				//drop old
				RemoveButtons();

				//update needed
				int i = 0;
				foreach(Mail m in emails.Mails)
				{
					//TODO only create buttons for visible one
					//based on slider value m.id >= slider.value
					Log.GameTimes ("Create Button + "+ i);
					UIButton b = CreateButton(i++);
					b.Text = m.subject;//.Substring(0,20);
					buttons.Add(b);
				}
				
				lastNumber = emails.Mails.Count;
			}
		}//UpdateMails()

		/// <summary>
		/// Remove each message buttons and un-register them.
		/// </summary>
		public void RemoveButtons()
		{
				foreach(UIButton b in buttons)
				{	
					//Log.GameTimes ("Remove Button + " + b.GameObject.name +  " " + b.GameObject.GetInstanceID());
					uiManager.Remove(b.GameObject.GetInstanceID());
					RemoveButton (b);
				}

				buttons.Clear();
		}//RemoveButtons()

		/// <summary>
		/// Create text buttons for inbox's messages
		/// </summary>
		public virtual UIToggleButton CreateButton(int pos)
		{
					//create button with mail subject as text
					GameObject obj = new GameObject();
					obj.name = "btn_Email_"+pos;
					obj.transform.parent = inbox.transform;
					obj.transform.position = new Vector3(0, messageButtonPosY - (pos * 0.5f), 0f);
					obj.AddComponent<SpriteRenderer>();
					BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
					col.size = new Vector2(5.5f, 0.4f);
					UIToggleButton button = (UIToggleButton)uiManager.Add(UIType.ToggleButton, obj, "Sprites/Email/btn_MessageHead");
					button.Register(UIEvent.Click, OnMessageClick);
					button.State = UIButtonState.RELEASE;
					button.Active = false;
					button.SortingOrder = 201;

					//create UIText
					GameObject t = new GameObject();
					t.name = obj.name+"-Text";
					t.transform.parent = obj.transform;

					t.transform.position = 	new Vector3(obj.transform.position.x, 
														obj.transform.position.y - 0.05f, 
														obj.transform.position.z);
					UIText txt = t.AddComponent<UIText>();
					txt.isLocalized = false;		
					txt.SortingOrder = button.SortingOrder + 1;
					txt.FontSize = 32;
					txt.maxLength = 40;
					//TODO use color here if message new
					//txt.FontColor = newColor;
					button.SetTextColors(upColor, downColor, overColor, disabledColor, activeColor);
					button.SetText(txt);
					
					return button;
		}//CreateButton()


		/// <summary>
		//TODO : remove Button from the hierarchy
		/// </summary>
		public void RemoveButton(UIButton button)
		{
			//create button with mail subject as text

			button.GameObject.transform.parent = null;
			button.GameObject.SetActive(false);
			Object.Destroy(button.GameObject);
		}

		/// <summary>
		/// Toggle the inbox visibility
		/// </summary>
		private void ToggleInbox()
		{
			//inbox.SetActive(!inbox.active);			
			Show(!inbox.active);

			//Log.Info("number of readed mails:"+readMails.Count+"  inbox:"+inbox.active);
			
		}//ToggleInbox()

		/// <summary>
		/// Handler for button clicks
		/// </summary>
		public void OnClick(string button)
		{
			Log.Debug("Email OnClick:"+button);


			//UIButton uib = (UIButton)uiManager.Get(button);
			Log.Debug("UIB:"+btnEmails.Name);

			if(button == btnEmails.Name)
			{
				Log.Debug("Toggle inbox");
				ToggleInbox();
			}

			else if(button == "btn_X_Inbox")
			{
				Show(false);
			}

			else if(button == "btn_Close_Mail")
			{
				Show(true);
			}


		}//OnClick()

		/// <summary>
		/// Handler for message head buttons
		/// Open the popup message with the right mail
		/// </summary>
		public void OnMessageClick(UIButton button)
		{
			Log.Debug("Message selected:"+button.Text);


			ShowMessage(button.Text);



		}//OnMessageClick


		/// <summary>
		/// Reset the slots after every screen change,
		/// This will make sure we have a valid state
		/// </summary>
		// public void ScreenChanged(UIScreen oldScreen, UIScreen newScreen)
		// {
		// 	Log.Debug("-- ScreenChanged --");
		// 	//if(gameObject.active && newScreen != null && oldScreen != null)
		// 	{
		// 		// Show(false);
		// 		// UpdateMails();

				
		// 	}

		// }//ScreenChanged()

		/// <summary>
		/// Show or hide the panels and emails button
		/// </summary>
		public void Show(bool flag)
		{
			btnEmails.UIButton.Visible = !flag;
			inbox.SetActive(flag);

			mail.SetActive(false);
			mailSubject.Text = "";
			mailMessage.Text = "";
			mailDate.Text = "";
			
			//TODO this should be depend on inter state
			if(flag)
			{
				//show buttons
				UpdateMails();
				uiManager.ShowModal(inbox);

			}
			else
			{

				//send events about mails has been readed
				foreach(Mail m in readMails)
				{
					GameManager.Instance.Event("EMAIL", m.id, "completed");
				}
				readMails.Clear();

			}

		}//Show()

		/// <summary>
		/// Show or hide the panels and emails button
		/// </summary>
		public void ShowMessage(string subject)
		{
			inbox.SetActive(false);
			mail.SetActive(true);

			Mail m = emails.Get(subject);

			mailDate.Text = m.date;
			mailSubject.Text = m.subject;
			mailMessage.wrapSize = 58;
			mailMessage.Text = m.message;

			m.isReaded = true;
			readMails.Add(m);

			uiManager.ShowModal(mail);

			Log.Debug("Mail id:"+m.id);

		}//Show()

	}//class EmailView


}//namespace