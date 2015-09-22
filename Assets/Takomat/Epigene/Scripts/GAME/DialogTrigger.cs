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
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Epigene.UI;
//------------------------------------------------------------------------------
namespace Epigene.GAME
{

	/// <summary>
	/// Dialog trigger.
	/// </summary>
	public class DialogTrigger : ATrigger
	{

		/// <summary>
		/// Gets or sets the id of the dialog
		/// </summary>
		/// <value>The value.</value>
		public string Id
		{
			get { return id;}
			//set { this.id = value; }
		}
		private string id;

		/// <summary>
		/// coordinates
		/// x,z,z where z is scale
		/// </summary>
		public Vector3 npcPosition;

		/// <summary>
		/// coordinates
		/// x,z,z where z is scale
		/// </summary>
		public Vector3 position;

		/// <summary>
		/// EventId of the dialog
		/// this is the screenId.id 
		/// </summary>
		private string fullId;

		/// <summary>
		/// comment for debug, optional
		/// </summary>
		private string comment;
		/// <summary>
		/// type of the dialog
		/// bubble, multichocie
		/// </summary>
		private DialogType dialogtype;
		/// <summary>
		/// screen id where this bubble is valid
		/// requried
		/// </summary>
		private string screenId;
		/// <summary>
		/// Image of the character who is speaking.
		/// If image id not specified, it won't be shown
		/// optional
		/// </summary>
		private string characterImage;
		/// <summary>
		/// Name of the character who is speaking.
		/// If name is not specified, it won't be shown
		/// in the bubble name field		
		/// </summary>
		private string characterName;		
		/// <summary>
		/// Event to trigger if this dialog appears/activated.
		/// requried
		/// </summary>
		/// 
		private bool characterVisible;
		/// <summary>
		/// dialog id to trigger
		/// </summary>
		private string trigger;
		/// <summary>
		/// flag to store when trigger was already done
		/// </summary>
		private bool triggered;
		/// <summary>
		/// delay in ms before trigger
		/// </summary>
		private double triggerDelay;
		/// <summary>
		/// in case of trigger is delayed, we keep a reference
		/// of this trigger, so we can remove it
		/// </summary>		
		private ScriptTrigger<string> triggerDelayObject;
		/// <summary>
		/// In case of a delayed delete, we keep a reference
		/// of this trigger, so we can remove it
		/// </summary>
		private ScriptTrigger<string> deleteDelayObject;

//------------------------------------------------------------------------------
		// e.g.

	/*	{
			"id": "D002",
			"type": "bubble",
			"screen": "CP1_2_2",
			"char": "Player",
			"trigger": {
				"event": "D003"  COMMENT: trigger D003 immediately without delay
			},
			"fire": {
				"delay": "1000"   COMMENT: here this D002 dialog text is triggered 1 second after it has started
			},
			"delete": {
				"event": "D004"   COMMENT: delete this D002 in D004 
			},            
			"text": {
				"neutral": "002"  COMMENT: the text to show
			},
			"lifetime_in_days": "",
			"minLifetime_in_ms": "10000"
		},
		*/

		/// <summary>
		/// list of texts for each emotional levels
		/// min. required to have neutral field
		/// </summary>
		private Dictionary<string, string> texts;
		
		/// <summary>
		/// Fire event id
		/// </summary>
		private string fireEventId;
		/// <summary>
		/// time delay in ms before fire
		/// </summary>
		private double fireEventDelay;
		/// <summary>
		/// delete event id
		/// </summary>
		private string deleteEventId;
		/// <summary>
		/// time delay in ms before delete
		/// </summary>
		private double deleteEventDelay;
		/// <summary>
		/// helper flag for fire condition
		/// it's better to check bool instaed of strinng 
		/// because of performance
		/// </summary>
		private bool fireFlag;
		/// <summary>
		/// helper flag for delete condition
		/// it's better to check bool instaed of strinng 
		/// because of performance
		/// </summary>
		private bool delFlag;

//TODO -- not yet implemented
		/// <summary>
		/// ?? optional
		/// </summary>
		private string lifetimeInDays;
		/// <summary>
		/// minimum life time, trigger will be sent
		/// only after this time expired. 
		/// optional
		/// </summary>
		private string minLifetimeInMs;
// -----------------------------

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.DialogTrigger"/> class.
		/// </summary>
		public DialogTrigger(string id)
		{
			Clear();
			this.type = TriggerType.DIALOG;
			this.id = id;
			this.fullId = id;
			this.fireFlag = false;
			this.delFlag = false;
			this.triggered = false;			

		}//ctor()

		/// <summary>
		/// Ctor for supporting scripting
		/// </summary>
		public DialogTrigger(int priority, int numberOfRepeat, 
		                     ACondition.FireCondition fireCondition,
		                     ACondition.FireCondition deleteCondition, string id)
		{
			Clear();
			this.id = id;
			this.type = TriggerType.DIALOG;
			this.fullId = id;
			this.priority = priority;
			this.repeat = numberOfRepeat;
			this.fireCondition = fireCondition;
			this.deleteCondition = deleteCondition;
			this.fireFlag = false;
			this.delFlag = false;
			this.triggered = false;

			
		}//ctor()

		/// <summary>
		/// Create DialogTrigger from dictionary
		/// which was parsed by json
		/// </summary>
		public DialogTrigger(Dictionary<string,object> dialog)
		{
			// Log.Info("ctor DialogTrigger");
			Clear();

			///--- defaults
			this.priority = 1;  //set in json?
			this.repeat = 1;
			this.fireCondition = CheckFire;
			this.deleteCondition = CheckDelete;
			this.fireFlag = false;
			this.delFlag = false;

			///--- must have
			this.id = dialog["id"].ToString();
			this.type = TriggerType.DIALOG;
			this.screenId = dialog["screen"].ToString();
			// runtime performance use StringBuilder
			StringBuilder sb = new StringBuilder();
			sb.Append(this.screenId);
			sb.Append(".");
			sb.Append(this.id);
			this.fullId = sb.ToString();

			//set default fire condition to the same as the id
			this.fireEventId = this.fullId;
			this.deleteEventId = "";
			this.fireEventDelay = 0;
			this.deleteEventDelay = 0;
			this.triggerDelay = 0;
			this.triggerDelayObject = null;
			this.deleteDelayObject = null;
			this.triggered = false;
			
			//GameManager.Instance.AddEventHandler(EventHandler);

			Parse(dialog);

		}//ctor()

		~DialogTrigger()
		{
			//This is not safe, due to Unity.
			//remove of the event must be taken care of outside!		
			//GameManager.Instance.RemoveEventHandler(EventHandler);
		}

		/// <summary>
		/// Helper to connect or remove event handler
		/// </summary>
		public override void ConnectEventHandler(bool flag)
		{
			Log.Debug("Connect eventhandler:" + flag);

			if(flag)
				GameManager.Instance.RegisterEventHandler("DIALOG", EventHandler, null);
			else
				GameManager.Instance.RemoveEventHandler("DIALOG", EventHandler);

		}

		/// <summary>
		/// Parse the trigger json definition
		/// </summary>
		private void Parse(Dictionary<string,object> dialog)
		{
			string t = dialog["type"].ToString();
			if(t == "bubble")
				this.dialogtype = DialogType.BUBBLE;
			else if(t == "multichoice")
				this.dialogtype = DialogType.MULTICHOICE;				                       

			// text part
			this.texts = new Dictionary<string,string>();
			Dictionary<string,object> txts =
				((Dictionary<string,object>)dialog["text"]);
			//neutral is a must to have			
			//this.texts.Add("neutral", txts["neutral"].ToString());

			// due to the fact: dont write too much in the 
			// definition file, we adapt neutral 
			// as much as possible from the definition

			// default 001, should not be used. 
			// It is an error actually, because than nothing is set.
			string strNeutral = "001"; 

			// find a suitable neutral text
			if (txts.ContainsKey ("neutral"))
						strNeutral = txts ["neutral"].ToString ();
			else if (txts.ContainsKey ("positive"))
				strNeutral = txts ["positive"].ToString ();
			else if (txts.ContainsKey ("negative"))
				strNeutral = txts ["negative"].ToString ();
			this.texts.Add("neutral", strNeutral);	

			//optional texts, use neutral by defaults
			if(txts.ContainsKey("positive"))
				this.texts.Add("positive", txts["positive"].ToString());
			else
				this.texts.Add("positive", strNeutral);

			if(txts.ContainsKey("negative"))
				this.texts.Add("negative", txts["negative"].ToString());
			else
				this.texts.Add("negative", strNeutral);


			//check character
			this.characterImage = dialog["character"].ToString();
			if(dialog.ContainsKey("character"))
			{
				Dictionary<string,object> chars = 
					((Dictionary<string,object>)dialog["character"]);

				// visible part
				//TODO  @Attila : Why must the visible flag be there before "image"
				// Because the other way around "image" and than "visible" 
				// does not read the "visible" flag in the chars array.-(
				// next step : see the flag as a constgant animation curve
				// holding the last visible state in a state value
				// like keyframing in Maya
				this.characterVisible = true;
				if(chars.ContainsKey("visible"))
				{
					
					string value = chars["visible"].ToString();
					// // // Log.GameTimes("Visible tag is"+value);
					
					// only Player is not schown
					if(value.Equals("false", StringComparison.Ordinal))
					{
						this.characterVisible = false;
						// // // Log.GameTimes("Visible tag set to false"+value);
					}
				}

				// image part
				if(chars.ContainsKey("image"))
				{
					this.characterImage = chars["image"].ToString();
					// only Player is not schown
					if(!(characterImage.Equals("Player", StringComparison.Ordinal)))
					{
						this.characterImage = chars["image"].ToString(); 
						this.characterName = this.characterImage;
					}
				}

				//parse positions with scale (z), 
				//default is zero position with normal scale
				npcPosition = new Vector3(0,0,1f);
				if(chars.ContainsKey("x"))
				{
					float x = Convert.ToSingle(chars["x"].ToString());
					this.npcPosition.x = x;
				}
				if(chars.ContainsKey("y"))
				{
					float y = Convert.ToSingle(chars["y"].ToString());
					this.npcPosition.y = y;
				}
				if(chars.ContainsKey("z"))
				{
					float z = Convert.ToSingle(chars["z"].ToString());
					this.npcPosition.z = z;
				}

				
				// if(chars.ContainsKey("name"))
				// {
				// 	this.characterName = chars["name"].ToString();
				// }
				
				// How do I get all the dictionary content 
				// foreach(string in chars.)
				/*
				foreach(var item in chars.Keys)
				{
					// // Log.GameTimes("Visible tag " + item + " " + chars[item].ToString());
				}

*/

			}

			//check 
			//this.trigger = dialog["trigger"].ToString();
			if(dialog.ContainsKey("trigger"))
			{
				Dictionary<string,object> triggers = ((Dictionary<string,object>)dialog["trigger"]);
				this.trigger = screenId+"."+triggers["event"].ToString();  	//event must to have if trigger defined
				/* We do not use trigger by default right now
				 * if(triggers.ContainsKey("delay")) 		//delay is optional
				{
					string s = triggers["delay"].ToString();
					if(s.Length > 0)
						this.triggerDelay = Convert.ToDouble(s);
				}
				*/
			}
			///--- optional fields
//------------------------------------------------------------------------------
			//fire condition determines when a dialog can be activated
			if(dialog.ContainsKey("fire"))
			{
				Dictionary<string,object> cond = ((Dictionary<string,object>)dialog["fire"]);
				if(cond.ContainsKey("event"))
				{
					//override default fire condition
					this.fireEventId = screenId+"."+cond["event"].ToString();
					// Log.Debug(this.fullId+".fire.event: "+fireEventId);
				}
				if(cond.ContainsKey("delay"))
				{
					string s = cond["delay"].ToString();
					if(s.Length > 0)
						this.fireEventDelay = Convert.ToDouble(s);
					
					// Log.Debug(this.fullId+".fire.delay: "+fireEventDelay);
				}
			}
		
			//delete condition determine when a dialog can be removed
			/* we do not use this right now
			if(dialog.ContainsKey("delete"))
			{
				Dictionary<string,object> cond = ((Dictionary<string,object>)dialog["delete"]);
				if(cond.ContainsKey("event"))
				{
					this.deleteEventId = screenId+"."+cond["event"].ToString();
					// Log.Debug(this.id+".delete.event: "+deleteEventId);
				}
				if(cond.ContainsKey("delay"))
				{
					string s = cond["delay"].ToString();
					if(s.Length > 0)
						this.deleteEventDelay = Convert.ToDouble(s);
					
					// Log.Debug(this.id+".delete.delay: "+deleteEventDelay);
				}
			}
			*/


			if(dialog.ContainsKey("condition"))
			{
				Dictionary<string,object> conditions = ((Dictionary<string,object>)dialog["condition"]);
				if(conditions.ContainsKey("fire"))
				{
					//override default fire condition
					fireEventId = conditions["fire"].ToString();
					//// Log.Debug(this.id+".conditions.fireCond: "+fireEventId);
				}
				if(conditions.ContainsKey("delete"))
				{
					deleteEventId = conditions["delete"].ToString();
					//// Log.Debug(this.id+".conditions.deleteCond: "+deleteEventId);
				}

			}

			//parse dialog positions
			if(dialog.ContainsKey("position"))
			{
				Dictionary<string,object> positions = ((Dictionary<string,object>)dialog["position"]);

				this.position = new Vector3(0,0,1f);
				if(positions.ContainsKey("x"))
				{
					float x = Convert.ToSingle(positions["x"].ToString());
					this.position.x = x;
				}
				if(positions.ContainsKey("y"))
				{
					float y = Convert.ToSingle(positions["y"].ToString());
					this.position.y = y;
				}
				if(positions.ContainsKey("z"))
				{
					float z = Convert.ToSingle(positions["z"].ToString());
					this.position.z = z;
				}
			}

			if(this.deleteEventId.Length == 0)
			{
				//use the trigger event for delete
				this.deleteEventId = this.trigger;
			}


			if(dialog.ContainsKey("comment"))
				comment = dialog["comment"].ToString();

			if(dialog.ContainsKey("lifetime_in_days"))
				comment = dialog["lifetime_in_days"].ToString();

			if(dialog.ContainsKey("minLifetime_in_ms"))
				comment = dialog["minLifetime_in_ms"].ToString();

		}//Parse()

		/// <summary>
		/// delegate function to check 
		/// if fire condition is true
		/// This will keep compatibility with other ctor
		/// </summary>
		public bool CheckFire()
		{
			//TODO check min time

			return fireFlag;			
		}//CheckFire()

		/// <summary>
		/// delegate function to check 
		/// if delete condition is true
		/// This will keep compatibility with other ctor
		/// </summary>
		public bool CheckDelete()
		{
			//TODO check min / exp time
			
			return delFlag;
		}//CheckDelete()
//------------------------------------------------------------------------------
		/// <summary>
		/// Fire this trigger.
		/// </summary>
		public override bool Fire()
		{
			//Log.Debug("fire:"+id);
			//call function(s) if Fire allowed
			if(base.Fire())
			{
				//TODO: send trigger event to UI or register delegate from UI?
				//// Log.Debug("----> Dialog:"+id+" fired event:"+trigger+","+emotion);	
				
				// //create Dialog model and send event				
				UIDialog d =  new UIDialog(id, 
				                           dialogtype, 
				                           screenId, 
				                           characterImage, 
				                           characterName,
				                           characterVisible,
				                           texts,
				                           position,
				                           npcPosition);
				UIManager.Instance.AddDialog(d);				
				GameManager.Instance.Event("DIALOG", fullId, "show");

				//only if not multichoice, 
				//when we need to wait for the selected item
				if(dialogtype != DialogType.MULTICHOICE)
				{
					if(triggerDelay == 0)
					{
						//wait for user action
						triggerDelayObject = null;
						//GameManager.Instance.Event(GameEvent.DIALOG, trigger, "fire");
					}
					else
					{
						triggerDelayObject = DelayedCommand(triggerDelay, "trigger");
					}
				
					if(deleteEventDelay > 0)
					{
						//TODO might need to add triggerDelay as well?
						deleteDelayObject = DelayedCommand(deleteEventDelay, "delete");
					}				

				}

				return true;
			}
			
			return false;
		}//Fire()

		/// <summary>
		/// Delete this trigger if required
		/// </summary>
		public override bool FireDelete()
		{
			//// Log.Debug("fireDelete:"+fullId);
			//we only want to remove if delete condition is true
			//if(base.FireDelete())
			if(deleteCondition != null && deleteCondition())
			{
				this.repeat = 0;
				UIManager.Instance.RemoveDialog(id);
				GameManager.Instance.Event("DIALOG", fullId, "hide");

				// Log.Debug("--- DELETE:  "+fullId);
				// Now as the trigger can be deleted,
				// remove the EventHandler 
				GameManager.Instance.RemoveEventHandler("DIALOG", EventHandler);
				return true;
			}

			return false;
		}//FireDelete()


		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void EventHandler(string eventId, string param)
		{

			//// Log.Info(fullId+ ": EVENT("+eventType+"): " + eventId+","+param);

			if(eventId == fireEventId && param == "fire")
			{
				//// Log.Debug("FIRE : "+id);

				if(fireEventDelay > 0)
				{
					DelayedCommand(fireEventDelay, "fire");
				}
				else
				{
					fireFlag =  true;
				}
			}

			if(eventId == fullId && param.Contains("close"))
			{
				//user force
				// Log.Debug("User force "+fullId+" to close, trigger activated:"+trigger);

				//remove trigger in case we have a delayed trigger in the list
				try
				{
					if(triggerDelayObject != null)
						GameManager.Instance.DialogGame.RemoveTrigger(triggerDelayObject);

					if(deleteDelayObject != null)
						GameManager.Instance.DialogGame.RemoveTrigger(deleteDelayObject);

					// Log.Debug("delayed commands are removed");
				}
				catch
				{
					// Log.Debug("Delayed commands are already removed");
				}

				Trigger();
				delFlag = true;

			}
			//check answers if multichoice
			//and trigger next dialog
			else if(dialogtype == DialogType.MULTICHOICE 
				&& eventId == fullId 
				&& param == "answered")
			{
				Trigger();
			}								
			//delete valid if event received
			else if((eventId == deleteEventId)
				|| (eventId == fullId && param == "hide"))
			{

				delFlag = true;		
			}
			
			//remove if screen changed
			// if(eventType == GameEvent.SCREEN 
			// 	&& eventId != screenId)
			// {

			// 	//delFlag = true;
			// }

			//TODO 

		}//CheckEvent()
//------------------------------------------------------------------------------
		/// <summary>
		/// helper to create trigger for wait and trigger
		/// </summary>
		public ScriptTrigger<string> DelayedCommand(double time, string param)
		{
			time /= 1000.0;
			// Log.Debug("Delayed "+fullId+" Command: " + param + " delay:"+time);

			// Relative means here, that the time is ADDED to the current gameTime
			TimerCondition relTimeCond = new TimerCondition(TimerType.Relative, 
			                      time, GameManager.Instance.DialogGame.TimeSource);
			ScriptTrigger<string> timeTrigger = 
					new ScriptTrigger<string>(1, 1, relTimeCond.Check, null, TimeExpired, param);
			GameManager.Instance.DialogGame.AddTrigger(timeTrigger);

			return timeTrigger;

		}//DealyedCommand()

		/// <summary>
		/// Helper for timer.
		/// This function will be called when a timer is expired.
		/// </summary>
		public void TimeExpired(string param)
		{

			// Log.Debug("TimeExpired: "+fullId+":"+param+ " delFlag:"+delFlag);

			if(param == "trigger")
			{
				Trigger();
				triggerDelayObject = null;
			}
			else if(param == "fire" && !delFlag)
			{					
				fireFlag = true;
			}
			else if(param == "delete")
			{
				delFlag = true;
				deleteDelayObject = null;
			}

		}//TimerCondition()

		/// <summary>
		/// Send fire event for dialog which needs to be triggered.
		/// This function also make sure that the same trigger cannot 
		/// be sent multiple times.
		/// </summary>
		private void Trigger()
		{
			if( !triggered && !delFlag 
				&& trigger != null && trigger.Length > 0)
			{
				GameManager.Instance.Event("DIALOG", trigger, "fire");
				triggered = true;
			}
		}//Trigger()


	}//class DialogTrigger

}//namespace
//------------------------------------------------------------------------------

