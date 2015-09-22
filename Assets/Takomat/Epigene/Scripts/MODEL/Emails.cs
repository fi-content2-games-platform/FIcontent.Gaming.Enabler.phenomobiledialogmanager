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
using Epigene.GAME;
using Epigene.UI;
using Epigene.IO;

namespace Epigene.MODEL
{

	/// <summary>
	/// Mail data
	/// </summary>
	public class Mail
	{
		/// <summary>
		/// id of the mail
		/// </summary>
		public string id;

		/// <summary>
		/// sender of the mail
		/// </summary>
		public string from;
		/// <summary>
		/// subject of the mail
		/// </summary>
		public string subject;
		/// <summary>
		/// message part of the mail
		/// </summary>
		public string message;
		/// <summary>
		/// date of mail
		/// </summary>
		public string date;
		/// <summary>
		/// flag to indicate if it was already been read.
		/// </summary>
		public bool isReaded = false;

		/// <summary>
		/// Default ctor
		/// </summary>
		public Mail(string id, string subject, string message, string date, string from = "")
		{
			this.id = id;
			this.isReaded = false;
			this.from = from;
			this.subject = subject;
			this.message = message;
			this.date =  date;
		}//ctor()

	}//class Mail

	/// <summary>
	/// Model for manage emails in the game.
	/// </summary>
	public sealed class Emails
	{
		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static Emails Instance
		{
			get { return instance; }
		}
		private static readonly Emails instance = new Emails();

		/// <summary>
		/// Get list of mails.
		/// We use List instead of Dictionary
		/// because we want to keep an order
		/// </summary>
		public List<Mail> Mails
		{
			get{ return mails;}
		}
		private List<Mail> mails;

		/// <summary>
		/// default ctor
		/// </summary>
		private Emails()
		{
			mails = new List<Mail>();

		}//ctor()

		/// <summary>
		/// Add a mail
		/// </summary>
		public void Add(string id, string subject, string message)
		{
			//TODO add date based on current game time
			string date = "";
			string month = "";
			string currentDate = "";
			
			//GameObject objCP = UIManager.Instance.GetHud("TimeLineCP");			
			//Slider slider = objCP.GetComponent<TimeLine>().slider.GetComponent<Slider>();

			System.DateTime dateTime = System.DateTime.Now;
			if (GameManager.Instance.Get("CP").IsStarted)
			{
				dateTime = GameManager.Instance.Get("CP").Time();
			}

			if (id == Z0000_GameObjectives.Instance.emailCPAcceptedUUID)
				dateTime.AddDays(1);

			currentDate = dateTime.Month.ToString ();
			switch (currentDate) 
			{
				case "1":
					month = "JANUAR";
					break;
				case "2":
					month = "FEBRUAR";
					break;
				case "3":
					month = "MÄRZ";
					break;
				case "4":
					month = "APRIL";
					break;
				case "5":
					month = "MAI";
					break;
				case "6":
					month = "JUNI";
					break;
				case "7":
					month = "JULI";
					break;
				case "8":
					month = "AUGUST";
					break;
				case "9":
					month = "SEPTEMBER";
					break;
				case "10":
					month = "OKTOBER";
					break;
				case "11":
					month = "NOVEMBER";
					break;
				case "12":
					month = "DEZEMBER";
					break;
				default:
					break;
			}
			date = dateTime.Day.ToString () + ". " + month;
			Add(new Mail(id, subject, message, date));
		}

		/// <summary>
		/// Add a mail
		/// </summary>
		public bool Add(Mail mail)
		{

			foreach(Mail m in mails)
			{
				//avoid to add the same mail again
				if(mail.id == m.id || mail.subject == m.subject)
				{
					//Log.GameTimes ("Did not add second time " + m.id);
					return false;
				}
			}	
			//Log.GameTimes ("Mail added " + mail.id);
			mails.Add(mail);
			return true;

		}//Add()



		/// <summary>
		/// Add a mail
		/// </summary>
		public void AddWithView(Mail mail)
		{
			if(Add (mail)==true) 
			{
				//Log.GameTimes ("Mail Viewed " + mail.id);
				GameManager.Instance.Event("EMAIL", 
			                           mail.id , "addView");
			}
		}//Add()


		/// <summary>
		/// Get one mail by subject
		/// </summary>
		public Mail Get(string subject)
		{
			foreach(Mail m in mails)
			{
				if(m.subject == subject)
					return m;
			}

			throw new System.Exception("No mail exist with subject:"+subject);
			//return null;
		}


		/// <summary>
		/// Get one mail by subject
		/// </summary>
		public Mail GetWithId(string id)
		{
			foreach(Mail m in mails)
			{
				if(m.id == id)
					return m;
			}
			
			throw new System.Exception("No mail exist with id:"+id);
			//return null;
		}

	}//class Emails

}//namespace
