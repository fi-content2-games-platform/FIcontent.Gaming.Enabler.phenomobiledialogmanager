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
using Epigene.MODEL;

using Epigene.GAME;
using Epigene.UI;

//------------------------------------------------------------------------------
namespace Epigene.VIEW 
{
	/// <summary>
	/// View representation of the MultiChoice model
	/// This script need to be attached to a popup_MultipleChoice gameobject.
	//TODO
	/// Next Design: Multiple NPC to manage here
	/// First approach is just a duplication of the NPC objects as sub Objects 
	/// to this DialogView
	/// </summary>
	public class DialogView : MonoBehaviour
	{

		/// <summary>
		/// Available types
		/// </summary>
		//public enum EmoType {POSITIVE, NEGATIVE, NEUTRAL};
		
		/// <summary>
		/// last dialog id to memories the last view state
		/// </summary>
		public string lastId;
		public string lastEventID;
		public string lastParam;

		/// <summary>
		/// Delegate functions to process selected value
		/// </summary>
		public delegate void ProcessAnswerFunction(
			UIDialog dialog, EmotionType answer);
		public ProcessAnswerFunction ProcessAnswer
		{
			set{ processAnswer = value; }
			//Answers were triggered twice if I left the plus here! (Jonas)
			//set { processAnswer += value; }
		}
		private ProcessAnswerFunction processAnswer;

		/// <summary>
		/// GameObject attached to
		/// </summary>
		public GameObject Parent
		{
			get { return gameObject; }
		}

//------------------------------------------------------------------------------
		/// <summary>
		/// NPC object to manage the character
		/// </summary>
		// public NPC Npc
		// {
		// 	get{ return npc; }
		// }
		// private NPC npc;

		/// <summary>
		/// NPC character
		/// </summary>		
		public UICharacter uiCharacter;
		/// <summary>
		/// Popup object
		/// </summary>
		private GameObject popup;

		/// <summary>
		/// Positive answer button
		/// </summary>
		private UIText positiveText;
		/// <summary>
		/// Negative answer button
		/// </summary>
		private UIText negativeText;
		/// <summary>
		/// Neutral answer button
		/// </summary>
		private UIText neutralText;


		/// <summary>
		/// Positive button
		/// </summary>
		private Button positiveButton;
		/// <summary>
		/// Negative button
		/// </summary>
		private Button negativeButton;
		/// <summary>
		/// Neutral button
		/// </summary>
		private Button neutralButton;

		/// <summary>
		/// MultiChoice button positions
		/// </summary>
		private Vector3[] multichoicePositions;

		/// <summary>
		/// SpeechBubble for NPC
		/// </summary>		
		private Dictionary<string,SpeechBubble> bubbles;

		/// <summary>
		/// level of emotion
		/// </summary>
		private EmotionType emotion;

		/// <summary>
		/// NPC dialog model
		/// </summary>
		private UIDialog npcDialog;

		/// <summary>
		/// player dialog model
		/// </summary>
		private UIDialog playerDialog;

		/// <summary>
		/// Game manager
		/// </summary>
		private GameManager gpm;

		/// <summary>
		/// ui manager
		/// </summary>
		private UIManager ui;

		/// <summary>
		/// helper to check screen id associeted
		/// </summary>
		public string StoryId
		{
			get{ return storyId; }
			set{ storyId = value; }
		}

		private string storyId;
		private string fileName;

		public bool ActivateLastDialogV
		{
			get {return activateLastDialogV;}
			set{ activateLastDialogV = value; }
		}
		bool activateLastDialogV = false;

		/// <summary>
		/// default npc position and scale (x,y,z)
		/// </summary>
		private Vector3 defNpcBubblePosition = new Vector3(-2.7f,1.8f,0f);

		/// <summary>
		/// default scale value for npc bubbles
		/// </summary>
		private Vector3 defNpcBubbleScale = new Vector3(40f,40f,40f);

		/// <summary>
		/// default npc position and scale (x,y,z)
		/// </summary>
		private Vector3 defPlayerBubblePosition = new Vector3(2.7f,-0.8f,0f);

		/// <summary>
		/// default scale value for npc bubbles
		/// </summary>
		private Vector3 defPlayerBubbleScale = new Vector3(40f,40f,40f);

		/// <summary>
		/// default npc position and scale (x,y,z)
		/// </summary>
		private Vector3 defNpcPosition = new Vector3(1f,-0.8f,1f);

		private GameObject tmpContainer;
		private UISkit skit;


		/// <summmary>
		/// Reset all DialogView Model relevant data
		/// </summary>
		public void Reset()
		{
			lastId      = "";
			lastEventID = "";
			lastParam   = "";
		}//ResetAction()

//-------------------------------- basic unity API MonoBehavior class methods -----


		/// <summary>
		/// Initialization, read childe gameobject 
		/// and set the buttons to it's uitext components
		/// </summary>
		void Awake()
		{

			// Log.Info("Awake:" + gameObject.name);
			Reset();

			GameObject obj;

			popup = GameObject.Find(gameObject.name+"popup_MultipleChoice");
			// Log.Assert(popup, "Missing popup_MultipleChoice in "+gameObject.name);

			obj = GameObject.Find(gameObject.name+
				"popup_MultipleChoice/btn_MultipleChoice_Answer_Positive/Text");
			// Log.Assert(obj, "Missing Positive button in "+gameObject.name);
			positiveText = obj.GetComponent<UIText>();
			// Log.Assert(positiveText, "Missing positiveText UIText component!");

			obj = GameObject.Find(gameObject.name+"popup_MultipleChoice/btn_MultipleChoice_Answer_Negative/Text");
			// Log.Assert(obj, "Missing Negative button in MultipleChoice popup!");
			negativeText = obj.GetComponent<UIText>();
			// Log.Assert(negativeText, "Missing negativeText UIText component!");

			obj = GameObject.Find(gameObject.name+"popup_MultipleChoice/btn_MultipleChoice_Answer_Neutral/Text");
			// Log.Assert(obj, "Missing Neutral button in MultipleChoice popup!");
			neutralText = obj.GetComponent<UIText>();
			// Log.Assert(neutralText, "Missing neutralText UIText component!");

			multichoicePositions = new Vector3[3];
			obj = GameObject.Find(gameObject.name+"popup_MultipleChoice/btn_MultipleChoice_Answer_Neutral");
			neutralButton = obj.GetComponent<Button>(); 
			multichoicePositions[0] = neutralButton.transform.position;
			
			obj = GameObject.Find(gameObject.name+"popup_MultipleChoice/btn_MultipleChoice_Answer_Negative");
			negativeButton = obj.GetComponent<Button>(); 
			multichoicePositions[1] = negativeButton.transform.position;
			
			obj = GameObject.Find(gameObject.name+"popup_MultipleChoice/btn_MultipleChoice_Answer_Positive");
			positiveButton = obj.GetComponent<Button>(); 
			multichoicePositions[2] = positiveButton.transform.position;


			bubbles = new Dictionary<string, SpeechBubble>();
			ui = UIManager.Instance;
			gpm = GameManager.Instance;
			
			
			//create temp container for dynamic bubbles
			tmpContainer = new GameObject();
			tmpContainer.name = "bubbles";
			tmpContainer.transform.parent = gameObject.transform;			

			ActivateAll(false);

		}//Awake()


		/// <summary>
		/// Enable the dialogs
		/// </summary>
		void OnEnable()
		{
			// Log.Info("DialogView Enable");
			ActivateAll(false);
			lastId = "";
			GameManager.Instance.RegisterEventHandler("DIALOG", ProcessDialogEvent);
			GameManager.Instance.RegisterEventHandler("RESET", ProcessResetEvent);
		}//OnEnable()

		/// <summary>
		/// Disable the dialogs
		/// </summary>
		void OnDisable()
		{
			// Log.Info("DialogView Disable");
			ActivateAll(false);
			GameManager.Instance.RemoveEventHandler("DIALOG", ProcessDialogEvent);
			GameManager.Instance.RemoveEventHandler("RESET", ProcessResetEvent);

		}//OnDisable()

		/// <summary>
		/// Set defaults
		/// </summary>
		void Start () 
		{
			// Log.Info("DialogView Start");

		}
//---------------------------------------------- basic unity API interface END -
		/// <summary>
		/// helper to activate gameObject
		/// </summary>
		public void SetActive(bool flag)
		{
			gameObject.SetActive(flag);
		}//SetActive()

		/// <summary>
		/// Load Dialogs from file
		/// </summary>
		public void Load(string fileName)
		{

			// Log.Info("LOAD: "+fileName);

			this.fileName = fileName;
			GameManager.Instance.AddDialogFromFile(fileName);
			
		}//Load()

		/// <summary>
		/// Send a dialog event to activate and show a dialog
		/// </summary>
		public void ActivateDialog(string dialogId)
		{
			// Log.Info("Activate Dialog:"+dialogId);
			//TODO this might be in a separate function..
			//hide all dialogs first
			//ActivateAll(false);

			GameManager.Instance.Event("DIALOG", storyId+"."+dialogId, "fire");

		}//ActivateDialog()

		/// <summary>
		/// Remove all dialogs
		/// </summary>
		public void RemoveAll()
		{
			//TODO implement
			//// Log.Warning("RemoveAll not yet implmeneted in DialogView.cs");
			// Log.Info("Remove all dialogs.");

			GMGame game = gpm.DialogGame;
			// Log.Info("game="+game);
			game.RemoveTriggers(TriggerType.DIALOG);
			
		}//RemoveAll()

		/// <summary>
		/// 
		/// </summary>
		public void ActivateLastDialog()
		{

/*			Debug.Log(gameObject.name+
			         " storyId:"+storyId+" - DialogView ActivateLastDialog: "+
			         " eventId:"+lastEventID+" param:"+lastParam);
*/
			if(lastParam!="show") return;
			ProcessDialogEvent(lastEventID, lastParam);
		}

		/// <summary>
		/// Process RESET events
		/// </summary>
		public void ProcessResetEvent(string eventId, string param)
		{
			if(eventId=="Models")
			{
				Reset();
			}
		}

		/// <summary>
		/// Process dialog events
		/// </summary>
		public void ProcessDialogEvent(string eventId, string param)
		{

			if(gameObject.activeSelf == false
				|| eventId == null || eventId.Length == 0)
				return;

			// Debug.Log ("eventId : " + eventId);
			// Debug.Log ("storyId : " + storyId);

			if(storyId == null || !eventId.StartsWith(storyId))
				return;

			// Debug.Log ("Save last ");
			lastEventID = eventId;
			lastParam   = param;

			if(param == "show")
			{
				UIDialog d = ui.GetDialog(eventId);

				//protect from too ealry or late process,
				//when dialog might not exist anymore
				if(d != null)
				{

					//// Log.Debug(gameObject.transform.parent.name+" SHOW Dialog:"+d.Id+","+d.Type);

					//for debug only
					lastId = d.Id;

					//get text, check type and show the right bubble/mc
					// // Log.GameTimes("Character "+d.CharacterImage.ToString());
					SpeechBubble bubble = GetBubble(d.CharacterImage);
					
					//show debug info for bubble
					if(GameManager.Instance.debugMode)
						bubble.DebugText = d.Id;
					else
						bubble.DebugText = "";			


					switch(d.Type)
					{
						case DialogType.BUBBLE:
							if(d.CharacterImage == "Player")
							{								
								//hide multi
								if(popup != null)
									popup.SetActive(false);


								uiCharacter = (UICharacter)ui.ActiveScreen.GetObject(d.CharacterImage);
								//// Log.Debug("d.CharImg:"+d.CharacterImage+" uiCharacter="+uiCharacter);
								if(uiCharacter != null)
								{

									d.Emotion = uiCharacter.Emotion;


									//set to settings from character
								    bubble.gameObject.transform.position = 
								    	uiCharacter.GameObject.transform.position + uiCharacter.bubblePosition;
								    bubble.gameObject.transform.localScale = uiCharacter.bubbleScale;
								    bubble.SetTail(uiCharacter.bubbleTail);



								//// Log.Debug("emotion: "+d.Emotion);
								
								}
								else
								{
									//set position
									bubble.gameObject.transform.position = defPlayerBubblePosition;
									bubble.gameObject.transform.localScale = defPlayerBubbleScale;
								}

								//d.Emotion = emotion;
								playerDialog = d;
								bubble.SetActive(true);
								bubble.Text = playerDialog.Text;
								bubble.Id = playerDialog.Id;
								bubble.Name = "";
							}
							else
							{
								//set emotion level and bubble position for character
								UIObject obj = ui.ActiveScreen.GetObject(d.CharacterImage);

								if (obj.GetType() == UIType.Character)
								{
									UICharacter uiCharacter = (UICharacter)obj;
									//// Log.Debug("d.CharImg:"+d.CharacterImage+" uiCharacter="+uiCharacter);
									if(uiCharacter != null)
									{
									 	//uiCharacter.Visible = true;					 	
										//d.Emotion = uiCharacter.Emotion;
										UICharacter player = (UICharacter)ui.ActiveScreen.GetObject("Player");

										if(uiCharacter.Name.StartsWith("Engineer.") || 
											uiCharacter.Name.StartsWith("CityHall."))
										{
											d.Emotion = uiCharacter.Emotion;
										}
										else if(player.Emotion != EmotionType.NEUTRAL)
										{
											d.Emotion = player.Emotion;
											uiCharacter.Emotion = player.Emotion;
										}
										else
										{
											d.Emotion = EmotionType.NEUTRAL;
											uiCharacter.Emotion = EmotionType.NEUTRAL;
										}

										if(uiCharacter.Name == "ArtistCommunity.ArtistGirl")
										{
											UICharacter uiCharacterArtist = (UICharacter)ui.ActiveScreen.GetObject("ArtistBoy");
											uiCharacterArtist.Emotion = uiCharacter.Emotion;
										}
										else if(uiCharacter.Name == "ArtistCommunity.ArtistBoy")
										{
											UICharacter uiCharacterArtist = (UICharacter)ui.ActiveScreen.GetObject("ArtistGirl");
											uiCharacterArtist.Emotion = uiCharacter.Emotion;
										}

										//set to settings from character
										bubble.gameObject.transform.position = 
											uiCharacter.GameObject.transform.position + uiCharacter.bubblePosition;
										bubble.gameObject.transform.localScale = uiCharacter.bubbleScale;
										bubble.SetTail(uiCharacter.bubbleTail);

										//// Log.Debug("--------------- TAIL"+uiCharacter.bubbleTail);
									}
								}
								else if (obj.GetType() == UIType.Skit)
								{
									skit = (UISkit)obj;
									if(skit != null)
									{
									 	skit.Show();

										bubble.gameObject.transform.position = skit.GameObject.transform.position + skit.bubblePosition;
										bubble.gameObject.transform.localScale = skit.bubbleScale;
										bubble.SetTail(skit.bubbleTail);
									}
								}
								else
								{
									//set to defaults
									bubble.gameObject.transform.position = defNpcBubblePosition;
									bubble.gameObject.transform.localScale = defNpcBubbleScale;
								}
								
								//show and set emotion level
								npcDialog = d;
								bubble.SetActive(true);
								bubble.Text = d.Text;
								bubble.Name = d.CharacterName;
								bubble.Id = d.Id;
							
							}
							break;

						case DialogType.MULTICHOICE:
							if(d.CharacterImage != "Player")
							{
								// Log.Error("Only Player can have multichoice dialog: "+d.Id);
								return;
							}

							//hide player							
							bubble.SetActive(false);

							playerDialog = d;
							popup.SetActive(true);
							UIManager.Instance.ShowModal(popup);

							ArrayList index = new ArrayList(new int[] {0, 1, 2});
							int[] 	  order = new int[3];

							if (!GameManager.Instance.debugMode)
							{
								for (int i = 0; i < 3; i++)
								{
									int rnd  = Random.Range(0, index.Count);
									order[i] = (int)index[rnd];
									index.RemoveAt(rnd);
								}
							}
							else
							{
								order[0] = 0;
								order[1] = 1;
								order[2] = 2;
							}

							neutralButton.transform.position  = multichoicePositions[order[0]];
							negativeButton.transform.position = multichoicePositions[order[1]];
							positiveButton.transform.position = multichoicePositions[order[2]];

							neutralText.Text  = playerDialog.GetText(EmotionType.NEUTRAL);
							positiveText.Text = playerDialog.GetText(EmotionType.POSITIVE);
							negativeText.Text = playerDialog.GetText(EmotionType.NEGATIVE);
							break;
					}
				}
			}
			else if(param == "hide")
			{
				// Log.Info(gameObject.transform.parent.name+" DialogView HIDE "+type+" event:"+eventId);

				if (skit != null)
				{
					skit.Hide();
					skit = null;
				}

				if(npcDialog != null && eventId == npcDialog.Id)
				{
					SpeechBubble bubble = GetBubble(npcDialog.CharacterImage);
					bubble.SetActive(false);
					npcDialog = null;
				}
				else if(playerDialog != null && eventId == playerDialog.Id)
				{
					if(playerDialog.Type == DialogType.BUBBLE)
					{
						SpeechBubble bubble = GetBubble("Player");
						bubble.SetActive(false);
					}
					else
						popup.SetActive(false);

					playerDialog = null;
				}
			}

		}//ProcessDialogEvent()

		/// <summary>
		/// Activate / deactivete all child
		/// </summary>
		private void ActivateAll(bool flag)
		{
			popup.SetActive(flag);
			foreach(KeyValuePair<string,SpeechBubble> item in bubbles)
			{

				item.Value.SetActive(flag);
			}
			

		}//ActivateAll()

		/// <summary>
		/// Process multi choice answer and set npc emotions
		/// </summary>
		private void ProcessSelection(string param)
		{
			UICharacter player = (UICharacter)ui.ActiveScreen.GetObject("Player");
			//player.Emotion = EmotionType.NEUTRAL;

			if(param == "btn_MultipleChoice_Answer_Positive")
			{
				emotion = EmotionType.POSITIVE;
				//if(uiCharacter != null)
					player.Emotion = EmotionType.POSITIVE;

				if(playerDialog != null)
					playerDialog.Emotion = EmotionType.POSITIVE;
				
				// Log.GameTimes ("ProcessSelection : " + 
				//               playerdialog.Id + " " +
				//               EmotionType.POSITIVE);
				GameManager.Instance.Event("DIALOG", playerDialog.Id, "answered");
				if(processAnswer != null)
					processAnswer(playerDialog, EmotionType.POSITIVE);
			}
			else if(param == "btn_MultipleChoice_Answer_Negative")
			{
				emotion = EmotionType.NEGATIVE;
				if(uiCharacter != null)
				{
					player.Emotion = EmotionType.NEGATIVE;
					playerDialog.Emotion = EmotionType.NEGATIVE;
				}
				// Log.GameTimes ("ProcessSelection : " + 
				 //              playerdialog.Id + " " +
				 //              EmotionType.NEGATIVE);
				GameManager.Instance.Event("DIALOG", playerDialog.Id, "answered");
				if(processAnswer != null)
					processAnswer(playerDialog, EmotionType.NEGATIVE);
			}
			else if(param == "btn_MultipleChoice_Answer_Neutral")
			{
				emotion = EmotionType.NEUTRAL;
				if(uiCharacter != null)
				{
					player.Emotion = EmotionType.NEUTRAL;
					playerDialog.Emotion = EmotionType.NEUTRAL;
				}
				// Log.GameTimes ("ProcessSelection : " + 
				//               playerdialog.Id + " " +
				//               EmotionType.NEUTRAL);
				GameManager.Instance.Event("DIALOG", playerDialog.Id, "answered");
				if(processAnswer != null)
					processAnswer(playerDialog, EmotionType.NEUTRAL);
			}
			else
			{
				// Log.Error("Invalid param '"+param+"' in "+gameObject.name);
			}

			if(player != null && uiCharacter != null)
			{
				//player.Emotion = uiCharacter.Emotion;
			}

		}//ProcessAnswer()

//------------------------------------------------------------------------------
		/// <summary>
		/// Hide all elements
		/// </summary>
		public void Hide()
		{
			ActivateAll(false);
		}//Hide()

		/// <summary>
		/// Close the bubble by name
		/// </summary>
		public void CloseBubble(SpeechBubble obj)
		{
			//handle close from bubble
			UIDialog d = ui.GetDialog(obj.Id);
			// // Log.GameTimes("_________________________________________ X clicked on:"+obj.Id+ ", Emotion "+obj.name);// + d.Emotion);

			// emotion tagging :
			string closeEmotion = "close"+ d.Emotion;
			//if(closeEmotion.Contains("close")) // // Log.GameTimes("Contains:");
			GameManager.Instance.Event("DIALOG", obj.Id, closeEmotion);
		}//Close()

				/// <summary>
		/// Re Init the button on screen change. 
		/// This will make sure we have a valid button after screen 
		/// has been changed.
		/// In case the button is disabled, this function will do nothing.
		/// </summary>
		public void ScreenChanged(UIScreen oldScreeen, UIScreen newScreen)
		{

			//// Log.Info(" ---------------------------------------------- ##################### screen:"+newScreen);
			if(gameObject.active && newScreen != null)
			{
				ActivateAll(false);
				//ActivateAll(true);
			}

		}//ScreenChanged()

		/// <summary>
		/// Get or create a new bubble for one npc
		/// </summary>
		public SpeechBubble GetBubble(string npcId)
		{
			if(!bubbles.ContainsKey(npcId))
			{
				GameObject obj = MainGame.CreateGameObject(MainGame.hudPrefabs +"SpeechBubble");
				obj.name = "bubble-"+npcId;
				obj.transform.parent = tmpContainer.transform;
				SpeechBubble bubble = obj.GetComponent<SpeechBubble>();
				// Log.Assert(bubble != null, "Can't find SpeecBubble in gameobject:"+obj.name);

				//TODO use delegates?
				bubble.handler = gameObject;
				bubble.onClose = "CloseBubble";

				bubbles.Add(npcId, bubble);
			}

			return bubbles[npcId];
			
		}

	}//class MultiChoiceView
}//namespace
//------------------------------------------------------------------------------