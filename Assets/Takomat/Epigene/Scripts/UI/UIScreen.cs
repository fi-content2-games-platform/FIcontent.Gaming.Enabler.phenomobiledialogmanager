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
using Epigene.AUDIO;
using Epigene.VIEW;
using Epigene.MODEL;
//------------------------------------------------------------------------------
namespace Epigene.UI
{

	/// <summary>
	/// In derived classes of UIScreen the specific logic 
	/// of a situation is implemented. UIScreen 
	/// sets up general things like EventHandler,
	/// which are valid for all situations.
	/// </summary>

	public abstract class UIScreen : MonoBehaviour 
	{

		/// <summary>
		/// Layer mask for this screen
		/// </summary>
		public LayerMask layerMask;

		/// <summary>
		/// Next screen name
		/// </summary>
		public string nextDefaultScreenName;

		/// <summary>
		/// screen name to go back
		/// </summary>
		public string backScreenName;

		/// <summary>
		/// The user interface manager.
		/// </summary>
		protected UIManager uiManager;

		/// <summary>
		/// Player manager
		/// </summary>
		protected GameManager gameManager;

		/// <summary>
		/// The audio manager
		/// </summary>
		protected AudioManager audioManager;

		/// <summary>
		/// Localization manager
		/// </summary>
		protected I18nManager i18n;

		/// <summary>
		/// DialogView reference.
		/// This could be optional, based on layerMask settings.
		/// </summary>
		protected DialogView dialogView;

//TODO add more npc
		//public NPC.Character npc = NPC.Character.NONE; 

		/// <summary>
		/// TODO: this MUST be configured in StartGame somehow, 
		///       now we just hardcoded for TWIST
		/// Path to objects
		/// </summary>
		// private string ObjectPath = "/TWISTBueBe/Buttons/";

		/// <summary>
		/// TODO: this MUST be configured in StartGame somehow, 
		///       now we just hardcoded for TWIST
		/// Path to configurations
		/// </summary>
		// private string configResourcePath = "Config/";

		/// <summary>
		/// Name of screen
		/// </summary>
		public string Name
		{
			get{ return gameObject.name; }
		}

		/// <summary>
		/// helper class for transform
		/// </summary>
		private class StoreTransform
		{
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 localScale;

			public StoreTransform(Vector3 position, Quaternion rotation, Vector3 localScale)
			{
				this.position = position;
				this.rotation = rotation;
				this.localScale = localScale;
			}
		}

		protected Dictionary<string, UIObject> screenObjects;
		protected List<string> huds;
		private List<StoreTransform> hudDefaultTransforms;
		protected List<string> popups;

		/// <summary>
		/// Init components
		/// </summary>
		void Awake()
		{
			InitScreen();
		}//Awake()

		/// <summary>
		/// Enable this instance.
		/// </summary>
		void OnEnable()
		{
			InitScreen();	
			InitDialog();

			Enter();

			if(dialogView)
				dialogView.SetActive(true);

		}//OnEnable()

		/// <summary>
		/// Disable this object
		/// </summary>
		void OnDisable()
		{
			Exit();
			Release();

		}//OnDisable()
		
		/// TODO this might goes to CTOR?
		/// <summary>
		/// Basic initialization
		/// </summary>
		virtual public void InitScreen()
		{
			Log.Assert(nextDefaultScreenName != "",
				"Missing nextDefaultScreenName, please set one in "+gameObject.name);

			uiManager = UIManager.Instance;
			gameManager = GameManager.Instance;
			audioManager = AudioManager.Instance;
			i18n = I18nManager.Instance;

			UIManager.UICamera.cullingMask = layerMask;

			//set every hud active, which used in this screen	
			if(huds != null && hudDefaultTransforms != null)
			{
				Log.Assert(hudDefaultTransforms.Count == huds.Count);

				for (int i = 0; i < huds.Count; i++)
				{
					GameObject obj = uiManager.GetHud(huds[i]);
					obj.SetActive(true);

					obj.transform.position = hudDefaultTransforms[i].position;
					obj.transform.localScale = hudDefaultTransforms[i].localScale;
					obj.transform.rotation = hudDefaultTransforms[i].rotation;
				}
			}
			
			//set every popup inactive, which used in this screen	
			if(popups != null)
			{
				foreach(string name in popups)
				{
					GameObject obj = uiManager.GetPopup(name);
					obj.SetActive(false);
				}
			}

		}//InitScreen()
//------------------------------------------------------------------------------
		/// <summary>
		/// Initialize dialogView if Dialog layer is on
		/// </summary>
		virtual public void InitDialog()
		{
			//Log.GameTimes ("UIScreen InitDialog"+gameObject.name);
			//if Dialog layer is used, set up the dialog
			if(UIManager.IsLayerVisible("Dialog"))
			{
				if(gameObject!=null)
				Log.Info ("InitDialog"+gameObject.name);
				else Log.Info ("InitDialog Null");

			}		
		}//InitDialog()

		/// <summary>
		/// release all dialogs
		/// </summary>
		virtual public void Release()
		{
			if(dialogView)
				dialogView.SetActive(false);

			UIManager.Instance.DisableHuds();

		}//Release()

//-------------------------------------------------------------------------------

		/// <summary>
		/// Activate or de-activate this screen
		/// </summary>
		virtual public void SetActive(bool flag)
		{
			Log.Info("Activated screen:"+Name);
			gameObject.SetActive(flag);
		}//SetActive()


		/// <summary>
		/// Go the next screen
		/// </summary>
	    virtual public void NextScreen(string nextScreenName)
		{
			Log.Info(this.name + ": NextScreen("+
						  nextScreenName+")");
			uiManager.ActivateScreen(nextScreenName);
		}//NextScreen()


		/// <summary>
		/// Go the next default screen
		/// </summary>
		virtual public void NextScreen()
		{
			Log.Info(this.name + ": NextScreen():"+nextDefaultScreenName);
			NextScreen(nextDefaultScreenName);
		}//NextScreen()

		/// <summary>
		/// Go the back screen
		/// </summary>
		virtual public void BackScreen()
		{
			//TODO maybe a better way to handle ractivating Dialogs, if needed 
			// Last Emotion State is not handled yet
			DialogManager.Instance.DialogView.ActivateLastDialogV = false;
			// Debug.Log("GO BACK SCREEN from : "+uiManager.ActiveScreen.Name);		
			if(uiManager.ActiveScreen.Name == "Settings" 
				|| uiManager.ActiveScreen.Name == "Pause")
			{
				DialogManager.Instance.DialogView.ActivateLastDialogV = true;
			}

			foreach (KeyValuePair<string, UIObject> entry in screenObjects)
			{
				if (entry.Key.StartsWith("btn_"))
				{
					UIButton button = (UIButton)entry.Value;
					button.Reset();	
				}
			}

			uiManager.ActivateScreen(backScreenName);
			if(DialogManager.Instance.DialogView.ActivateLastDialogV)
			{
				DialogManager.Instance.DialogView.ActivateLastDialogV = false;
				//Debug.Log("ActivateLastDialog SCREEN:"+backScreenName);
				DialogManager.Instance.DialogView.ActivateLastDialog();		
			}
			// Log.GameTimes("GO BACK SCREEN:"+backScreenName);
		}//BackScreen()

		/// <summary>
		/// Update the settings
		/// </summary>
		virtual public void UpdateSettings()
		{
			Log.GameTimes (this.name+" UpdateSettings ActivateScreen ");
			//TODO load/save settings, use local variable for name
			//activate settings page,
			UIScreen setScreen = uiManager.ActivateScreen("Settings");
			setScreen.backScreenName = Name;
			
		}//UpdateSettings()

		/// <summary>
		/// This function handle the dialogView answer from player.
		/// Need to override it in the screen script. 
		/// </summary>
		virtual public void ProcessAnswer(UIDialog dialog, EmotionType answer)
		{
			Log.Warning("Please OVERRIDE ME!! in:"+gameObject.name);
		}//ProcessAnswer


		/// <summary>
		/// Create a screen with all required child.
		/// Load a screen from a json file
		/// </summary>
		public void CreateScreen(Dictionary<string,object> screen)
		{
			Parse(screen);
			InitScreen();
			//add to uiManager?

		}//Load()

		/// <summary>
		/// Parse the trigger json definition
		/// </summary>
		private void Parse(Dictionary<string,object> screen)
		{

			// Log.Debug("PARSE Screen: "+screen["id"].ToString());
			
			//TODO we might want to create the game object here?
			//set parameters
			gameObject.name = screen["id"].ToString();
			nextDefaultScreenName = screen["next"].ToString();
			backScreenName = screen["back"].ToString();
			string layers = screen["layers"].ToString();

			string[] listOfLayers = layers.Split(',');
			foreach(string layer in listOfLayers)
			{
				this.layerMask |= 1 << LayerMask.NameToLayer(layer);
			}

			//parse list of objects
			if(screen.ContainsKey("objects"))
			{
				uiManager = UIManager.Instance;

				screenObjects = uiManager.LoadResources(screen, gameObject);
				huds = uiManager.GetHudNames(screen);
				popups = uiManager.GetPopupNames(screen);

				hudDefaultTransforms = new List<StoreTransform>();
				foreach (string hud in huds)
				{
					GameObject obj = uiManager.GetHud(hud);

					StoreTransform transform = new StoreTransform(obj.transform.position, obj.transform.rotation, obj.transform.localScale);
					hudDefaultTransforms.Add(transform);
				}
			}
		}//Parse()

		/// <summary>
		/// Get and object from the screen.
		/// </summary>
		public UIObject GetObject(string name)
		{
			//check if exist
			if(screenObjects == null)
				return null;

			if(screenObjects.ContainsKey(name))
				return screenObjects[name];
			else
				return null;
		}//GetObject()
			
		

	//TODO:
	//make these abstracts later, when integration is done
		/// <summary>
		/// This function is called when the 
		/// game is enter to this screen.
		/// Use it to initialize the screen/game specific
		/// objects.
		/// AS an advancement design :
		/// Create Enter() sub methods : 
	    /// Put all default visibility in HandleVisibilityOnEnter 
		/// and all specific Visibility in a HandleDialogDependantLogic
	    /// An example is Simulation.cs		
		/// </summary>
		public virtual void Enter()
		{
			Log.Debug("---UIScreen ENTER--");
		}//Enter()

		/// <summary>
		/// This function is called when the 
		/// game exit from this screen.
		/// </summary>
		public virtual void Exit()
		{
			Log.Debug("---UIScreen EXIT--");
		}//Exit()

		/// <summary>
		/// process all type of event which related to this screen.
		/// </summary>
		public virtual void EventHandler(string eventId, string data)
		{
			Log.Debug("---UIScreen EventHandler--");
		}
		

	}//class UIScreen

}//namespace
//------------------------------------------------------------------------------