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

using Epigene.GAME;
using Epigene.MODEL;
using Epigene.UI;
using Epigene.VIEW;
//------------------------------------------------------------------------------
namespace Epigene.GAME
{
//------------------------------------------------------------------------------
	/// <summary>
	/// Dialog manager class to manage the dialog play via triggers.
	/// It generally maanges game independant management task
	/// like loading a Story.
	/// A dialog is hierarchically :
	/// Story : Is a text file, in which a dialog junk is defined
	/// </summary>
	public sealed class DialogManager
	{
		public string NextInit
		{
			get {return nextInit;}
			set {nextInit = value;}
		}
		private string nextInit;

		public DialogView DialogView
		{
			get {return dialogView;}
		}
		private DialogView dialogView;

		public string ConfigResourcePath
		{
			get {return configResourcePath;}
			set {configResourcePath = value;}
		}
		// set a defualt path
		private string configResourcePath = "Config/Dialog/";

//------------------------------------------------------------------------------
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static DialogManager Instance
		{
			get{ return instance;}
		}
		private static readonly DialogManager instance = new DialogManager();

		/// <summary>
		/// ctor
		/// </summary>
		private DialogManager()
		{
			Log.Debug("DialogManager initialized.");
			GameObject dia = UIManager.Instance.GetHud("Dialogs");

			if(dia == null)
			{
				Log.Error("Dialog obj missing: Dialogs");
				return;
			}

			dialogView = dia.GetComponent<DialogView>();
			nextInit = "D001";

			//register event handler			
			GameManager.Instance.RegisterEventHandler("DIALOG", EventHandler);
			
			//TODO  Where to remove this EventHandler 
            // with  GameManager.Remove(EventHandler);
			// Destructor is not possible
		}//GameManager()
		
		/// <summary>
		/// Releases unmanaged resources and performs other 
		/// cleanup operations before the <see cref="GameManager"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~DialogManager()
		{
			
		}
//------------------------------------------------------------------------------
		public void LoadStory(string name, string dialogId,
		                       DialogView.ProcessAnswerFunction pa = null)
		{
			// Log.GameTimes ("InitDialog Name : "+ name);
			if(name!=null) dialogView.StoryId = name;
		
			dialogView.Load(configResourcePath+"dialogs_"+name);
			if(pa != null)
				dialogView.ProcessAnswer = pa;
			nextInit=dialogId;
			// Log.GameTimes("NextDialog : "+nextInit+  " "+ name);
			dialogView.ActivateDialog(dialogId);
		}


		public void LoadStory(string name, 
		                       DialogView.ProcessAnswerFunction pa = null)
		{
			LoadStory(name, "D001", pa);
		}

		public void LoadStoryNextInit(string name, 
		                       DialogView.ProcessAnswerFunction pa = null)
		{
			LoadStory(name, nextInit, pa);
		}

//------------------------------------------------------------------------------
		private void Restart()
		{

		}

		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void EventHandler(string eventId, string param)
		{
			//Log.Info(fullId+ ": EVENT("+eventType+"): " 
			// + eventId+","+param);
			if(eventId=="Restart")
			{
				Restart();
			}
		}
//----------------------------------  logic transitions --------------------

		public void HUD(string eventId, string param)
		{

			// Log.GameTimes("HUD(string "+eventId+", string "+param+")");
		}
		
		public void RemoveAll()
		{
			if(dialogView != null)
				dialogView.RemoveAll();
		}

	}//class DialogManager

}//namespace GameManager
//------------------------------------------------------------------------------