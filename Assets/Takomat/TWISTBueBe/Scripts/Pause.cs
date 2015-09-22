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
using Epigene.AUDIO;
using Epigene.GAME;
//------------------------------------------------------------------------------
namespace TWISTBueBe
{
	/// <summary>
	/// Screen for settings
	/// </summary>
	public class Pause : UIScreen
	{
		/// <summary>
		/// Enter into this screen		
		/// </summary>
		public override void Enter()
		{

			Log.Info("ENTER "+Name);

			//GameManager.Instance.EventHandler = EventHandler;

			//connect event handlers
			GameManager.Instance.RegisterEventHandler("SCREEN", ProcessScreenEvent, gameObject);
			
		}//Enter()

		/// <summary>
		/// Exit from this screen
		/// </summary>
		public override void Exit()
		{
			Log.Info("EXIT "+Name);

		}//Exit()

		/// <summary>
		/// Event handling for all sceen events
		/// </summary>
		public void ProcessScreenEvent(string eventId, string data)
		{

			if(eventId == "Back")
			{
				GameManager.Instance.Event("TIMELINE",
											TimeLine.TimelineType.CP.ToString(), 
				                           	"Unpause");
				GameManager.Instance.Event("TIMELINE",
											TimeLine.TimelineType.SIM.ToString(), 
				                           	"Unpause");

				BackScreen();
			}
		}

	}//class Settings

}//namespace