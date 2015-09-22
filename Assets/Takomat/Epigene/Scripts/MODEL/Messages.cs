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

using Epigene.UI;

namespace Epigene.MODEL
{

	/// <summary>
	/// Mail data
	/// </summary>
	public class Messages
	{		
		/// <summary>
		/// Question
		/// </summary>
		public string Message
		{
			get { return message;}
		}
		private string message;
		
		/// <summary>
		/// localization id
		/// </summary>
		public string ID
		{
			get {return id;}			
		}
		private string id;
		
		/// <summary>
		/// ctor set up question with answers
		/// by question id from localization db.
		/// Every id start with a letter which 
		/// describe the type.
		/// Use id without the prefix.
		/// </summary>
		public Messages(string id, string message)
		{
			this.id = id;
			this.message = message;

		}//ctor()
		
	}//class MessagePopup
	
	/// <summary>
	/// Manager for messagepopup.
	/// It's a singleton which will keep 
	/// and id for the active question and 
	/// manage the selected answer.
	/// </summary>
	public sealed class MessagePopups
	{
		
		public bool XClick
		{
			get{ return xClick;}
			set{ xClick = value;}
		}
		public bool xClick = false;

		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static MessagePopups Instance
		{
			get { return instance; }
		}
		private static readonly MessagePopups instance = new MessagePopups();
		
		/// <summary>
		/// list for questions and answers by id
		/// </summary>
		private List<Messages> Messages
		{
			get{ return messages;}
		}
		private List<Messages> messages;
		
		/// <summary>
		/// ctor
		/// This function will read the localization db
		/// and set up each MessagePopup item from 
		/// the section "MessagePopup"
		/// </summary>
		private MessagePopups()
		{
			messages = new List<Messages>();
			
		}//ctor()

	}//class MessagePopup

}//namespace
