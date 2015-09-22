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

#if EPIGENE_UI_46

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Epigene;
using Epigene.IO;
using Epigene.GAME;


namespace Epigene.UI
{

	/// General software design issues:
	/// - The uiManager is in contrast to Daikon UI 
	///   designed to be generate script based the UI
	///   like Qt.
	///   For example we do NOT have an attached "Button Component"
	///   like the "DF" Components, which are attached to a button.
	///   Just the SpriteRenderer Component of unity3d has to be attached.
	/// - Maybe in the future we add Components to the UI Epigene.UI

	public enum UIEvent {Click, Pressed, Release, Over, Update, OverExit};

	public enum UIType {Image, Button, ToggleButton, 
		Slider, Checkbox, Character, Skit, Text, Video, ChartPie, ChartLine, ALL};

	/// <summary>
	/// Delegate functions definition for changing screens
	/// </summary>
	public delegate void UIScreenFunction(UIScreen oldScreeen, UIScreen newScreen);




	/// Delegate functions definition for slider
	/// </summary>
	public delegate void UpdateFunction(float value);


	/// <summary>
	/// User interface manager. This class will manage the UI.
	/// 
	/// </summary>
	public sealed class UIManager
	{
		/// <summary>
		/// id for generated names.
		/// this could be advanced later
		/// </summary>
		private static int lastId;

		/*
		 * TODO 
		 * other name? rename to sprite manager? screenManager?
		 * add touch inputs
		 */

		enum MouseButton {Left = 0, Right, Middle};


		/// <summary>
		/// The start width of the screen
		/// </summary>
		private int scrWidth;

		/// <summary>
		/// Start height of the screen
		/// </summary>
		private int scrHeight;

		/// <summary>
		/// True if we are in full screen mode.
		/// </summary>
		private bool fullScreen;

		//public void SetIniFile(IniFile f
		public string FileName
		{
			get {return fileName;}
			//set {fileName = value;}
		}
		private string fileName;

		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static UIManager Instance
		{
			get 
			{
				if(instance != null && instance.images == null)
				{
					//dead singleton, recreate it
					Log.Warning("This is bad?");
					instance = new UIManager();
				}
				return instance; 
			}
		}
		private static UIManager instance = new UIManager();

		/// <summary>
		/// Add delegate to notify changing screens.
		/// </summary>
		/// <value>The function to call.</value>
		public static UIScreenFunction ScreenChanged
		{
			get { return screenChanged; }
			set { screenChanged += value;}
		}
		private static UIScreenFunction screenChanged;

		/// <summary>
		/// Get the number of registered screens
		/// </summary>
		public int NumberOfScreens
		{
			get{ return screens.Count; }
		}

		/// <summary>
		/// UI camera
		/// </summary>
		public static Camera UICamera
		{
			get { return uiCamera; }
			set { uiCamera = value; }
		}
		private static Camera uiCamera = Camera.main;

		/// <summary>
		/// Resource path for configurations
		/// </summary>
		public string ConfigResources
		{
			get { return configResources; }
			set { configResources = value; }
		}
		private string configResources;

		/// <summary>
		/// parent game object for all ui objects
		/// </summary>
		public GameObject UIParent
		{
			get { return uiParent; }
			set { Debug.Log("<color=cyan>*****</color>"); uiParent = value; }
		}
		private GameObject uiParent;

		/// <summary>
		/// parent game object for all hud objects
		/// </summary>
		public GameObject HudParent
		{
			get { return hudParent; }
			set { hudParent = value; }
		}
		private GameObject hudParent;

		public GameObject ModelParent
		{
			get { return modelParent; }
			set { modelParent = value; }
		}
		private GameObject modelParent;

		public UITooltip Tooltip
		{
			get{
				 return tooltip;
				}
			set{
				 this.tooltip = value;
				 if(value != null && hudParent != null)
				 	this.tooltip.transform.parent = hudParent.transform;
				}
		}
		private UITooltip tooltip;

		/// <summary>
		/// Game play manager
		/// </summary>
		private GameManager gpm;

		/// <summary>
		/// ini file
		/// </summary>
		private IniFile iniFile;

		/// <summary>
		/// List of available screens
		/// </summary>
		private Dictionary<string, UIScreen> screens;
		/// <summary>
		/// List of available UI items
		/// </summary>
		private Dictionary<int, UIObject> images;
		/// <summary>
		/// List of available dialogs
		/// </summary>
		private Dictionary<string, UIDialog> dialogs;
		/// <summary>
		/// List of available popups
		/// </summary>
		private Dictionary<string, GameObject> popups;
		/// <summary>
		/// List of available huds
		/// </summary>
		private Dictionary<string, GameObject> huds;
		/// <summary>
		/// List of available models
		/// </summary>
		private Dictionary<string, GameObject> models;

		/// <summary>
		/// active button
		/// </summary>
		private UIButton activeButton;
		private UIButton lastButton;

		/// <summary>
		/// active screen
		/// </summary>
		public UIScreen ActiveScreen
		{
			get { return activeScreen; }
		}
		private UIScreen activeScreen;

		/// <summary>
		/// original layer mask of the camera
		/// </summary>
		private LayerMask layerMask;

		private GameObject modalWindow;
		private Dictionary<int, GameObject> modals;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIManager"/> class.
		/// </summary>
		public UIManager()
		{
			// Log.Debug("<color=yellow>UIManager initialized.</color>");

			InternalInit();

			//save start screen size
			scrWidth = Screen.width;
			scrHeight = Screen.height;
			fullScreen = Screen.fullScreen;

			gpm = GameManager.Instance;

			lastId = 0;

			//InitUI();
			
		}//ctor()

		private void InternalInit()
		{
			screens = new Dictionary<string,UIScreen>();
			images = new Dictionary<int,UIObject>();
			dialogs = new Dictionary<string,UIDialog>();
			huds = new Dictionary<string, GameObject>();
			models = new Dictionary<string, GameObject>();
			popups = new Dictionary<string, GameObject>();

		}

		/// <summary>
		/// Helper to reset/recreate the ui parent objects
		/// </summary>
		public void InitUI(GameObject parent)
		{
    		Debug.Log("Init UI ************");

			//GameObject u = null;
			foreach(Transform t in parent.transform)
			{
				if(t.name == "UI")
				{
					//Log.Warning("========================================== UI already there!!!");
					uiParent = t.gameObject;
					// uiParent = u;
				}
			}

			if(!uiParent)
			{
				uiParent = new GameObject();
				uiParent.name = "UI";
				uiParent.transform.parent = parent.transform;
				hudParent = new GameObject();
				hudParent.name = "HUD";
				hudParent.transform.parent = uiParent.transform;
				modelParent = new GameObject();
				modelParent.name = "MODEL";
				modelParent.transform.parent = uiParent.transform;
			}
		}


		/// <summary>
		///Process inputs and update state of ui element.
		/// </summary>
		public void Update()
		{
			//Log.Debug("UI Update");
			//
			if(modalWindow != null)
			{
				if(!modalWindow.active)
				{
					//modal not active anymore, remove it
					modalWindow = null;
				}
			}
			
			//get mouse Position and check if we have something under the pointer
			Vector3 mp = Input.mousePosition;
			LayerMask mask = uiCamera.cullingMask;

			RaycastHit2D topHit;
			int topHitOrder = -1;

			Vector3 click = uiCamera.ScreenToWorldPoint(mp);
			RaycastHit2D[] hits = Physics2D.LinecastAll(click, click, mask);

			// Debug.Log("HITS:"+hits.Length);

			foreach (RaycastHit2D hit in hits)
			{
				if(hit.collider)
				{
					GameObject obj = hit.collider.gameObject;
					SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
					
					// Log.Debug("Collider:"+obj.name);
					if (spriteRenderer != null)
					{
						if (spriteRenderer.sortingOrder > topHitOrder)
						{
							topHit = hit;
							topHitOrder = spriteRenderer.sortingOrder;
						}
					}
					else 
					{
						MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
						SortingOrder sortingOrder = obj.GetComponent<SortingOrder>();
						if (meshRenderer != null)
						{
							if (meshRenderer.sortingOrder > topHitOrder)
							{
								topHit = hit;
								topHitOrder = meshRenderer.sortingOrder;
							}
						}
						else if (sortingOrder != null)
						{
							if (sortingOrder.sortingOrder > topHitOrder)
							{
								topHit = hit;
								topHitOrder = sortingOrder.sortingOrder;
							}
						}
						else //document button
						{
							if (topHitOrder < 1)
							{
								topHit = hit;
								topHitOrder = 1;
							} 
						}
					}
				}
			}
			// Log.Debug("TopHit:"+topHitOrder);

			if(topHitOrder > -1)
			{
				GameObject obj = topHit.collider.gameObject;
				
				//check if we are in modal mode
				if(modalWindow != null)
				{
					//check if obj is in the modals list
					if(!modals.ContainsKey(obj.GetInstanceID()))
					{
						if(lastButton != null)
						{
							lastButton.Release();
							lastButton = null;
						}
						//not in the list
						return;
					}
				}

				//check if this is a button.
				
				// Log.Info("HIT something:"+obj.name+" images:"+images.Count);
				UIButton button = null;
				if(images.ContainsKey(obj.GetInstanceID()))
				{	
					try
					{
						UIObject b = (UIObject)images[obj.GetInstanceID()];
						UIType t = b.GetType();
						// if(t ==  UIType.Button
						// ||	t.GetType().IsSubclassOf(typeof(UIButton)))
						if(t == UIType.Button 
							|| t == UIType.ToggleButton
							|| t == UIType.Slider
							|| t == UIType.Checkbox)
						{
							button = (UIButton)b;
						}
					}
					catch(System.Exception e)
					{
					 	Log.Debug("Could not find ui key:"+obj.name);
					 	Log.Debug("error :"+e.ToString());
					}
				}

				// if(button!=null)
				//  	Log.Debug("---------------- Button:"+button.Name);
				// else
				// 	Log.Debug("---------------- Button NULL");

				if(button != null && lastButton != null
				 	&& button != lastButton )
				{
					//Log.Debug("Release!!");
					lastButton.OverExit();
					lastButton.Release();
					lastButton = null;

					return;
				}
				else if(button == null && lastButton != null )
				{
					lastButton.OverExit();
					lastButton.Release();
					lastButton = null;

				}

				if(button != null && 
				   button.State != UIButtonState.OVER && 
				   button.State != UIButtonState.DISABLED && 
				   button.State != UIButtonState.ACTIVE_DISABLED && 
				   GameManager.Instance.hidTouch)
				{
					if(lastButton != null)
					{
						lastButton.Release();
					}
					lastButton = button;
					button.Push();
					
					activeButton = button;
				}
				else if(button != null && button.State != UIButtonState.DISABLED && button.State != UIButtonState.ACTIVE_DISABLED)
				{
					//Log.Debug("button:"+button.Name);
					if(Input.GetMouseButtonDown((int)MouseButton.Left))
					{
						//Log.Debug("button Push1:"+button.Name);
						if(lastButton != null)
						{
							lastButton.Release();
						}
						lastButton = button;
						button.Push();

						activeButton = button;
					}					
					else if(Input.GetMouseButton((int)MouseButton.Left))
					{
						//Log.Debug("button push:"+button.Name);
						//lastButton = button;
						if(activeButton == button)
						{
							button.Push();
							lastButton = button;
						}
						else if(lastButton != null)
						{
							lastButton.Release();
						}
				
					}				
					else if (Input.GetMouseButtonUp((int)MouseButton.Left))
					{
						//Log.Debug("button Up:"+button.Name+" ab:"+lastButton.Name+" s:"+button.State);
						
						if(activeButton == button)
						{								
							button.Click();
							button.Release();
						}
						if(lastButton != null)
							lastButton.Release();

						lastButton = null;
						activeButton = null;

					}
					else
					{
						if(lastButton != null && button != lastButton)
						{
							lastButton.OverExit();
							lastButton.Release();
						}
						else if(activeButton == null)
						{
							lastButton = button;
							button.Over();
						}
					}
				}
			}//tophit
			else if(lastButton != null)
			{
				lastButton.OverExit();
				//Log.Debug("***********************");
				lastButton.Release();
				lastButton = null;
				//WebPlayerDebugManager.addOutput("------------- not tophit, activeButton="+activeButton+", lastButton="+lastButton,1);
				
			}
		
		}//Update()

		/// <summary>
		/// Check if the tooltip can be visible at the moment
		/// or should be hidden
		/// </summary>
		public bool CheckTooltipVisibility(GameObject obj)
		{
			if (modalWindow != null && modalWindow.active == true && !modals.ContainsKey(obj.GetInstanceID()))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Helper to notify when screen is resizes.
		/// </summary>
		/// <param name="w">The new width.</param>
		/// <param name="h">The new height.</param>
		public void SetResolution(int w, int h, bool fullScreen)
		{
			Log.Debug("Screen resized, new dim:"+w+"x"+h);
			//screen dim resize.. do what you need for rescale.
			Screen.SetResolution(w, h, fullScreen);
			this.scrWidth = w;
			this.scrHeight = h;
			this.fullScreen = fullScreen;
		}

		/// <summary>
		/// Loads a sprite and return a list of textures.
		/// If the sprite is in multi mode, the list will contains each sprites.
		/// If the sprite is single mode, the list will contains only one item.
		/// </summary>
		/// <returns>The sprite array</returns>
		/// <param name="fileName">file name with path relative to Resource folder.</param>
		public static Sprite[] LoadSprite(string fileName)
		{
			//Log.Debug("Load sprite:<color=yellow>" + fileName + "</color>");
			if(fileName.Length == 0)
			{
				return null;
				//throw new System.ArgumentNullException("No fileName specified for LoadSprite!");
			}
			
			//check if localized version available
			Sprite[] sprList;
			string fName = string.Format("{0}_{1}", fileName, I18nManager.Instance.Language);
			// Log.Debug("search for langFileName:"+fName);
			sprList = Resources.LoadAll<Sprite>(fName);
			if(sprList.Length == 0)
			{
				//check again without localization
				fName = fileName;
				// Log.Debug("search for not localized version");			
				sprList = Resources.LoadAll<Sprite>(fName);
			}

			if(sprList == null)// || sprList.Length == 0)
			{
				Log.Debug("Loading Sprite <color=yellow>"+fName+"</color>");
				throw new System.ArgumentNullException("Cannot load sprites: "+fName);
			}
			// Log.Debug("Loading Sprite <color=yellow>"+fileName+"</color> image count:"+sprList.Length);
			//foreach(Sprite s in sprList)
			//{
			//	Log.Debug("Loaded sprite texture:<color=yellow>"+s.name+"</color>");
			//}
			
			return sprList;
		}//LoadSprite()
		
		/// <summary>
		/// Adds a UI button.		
		/// </summary>
		/// <param name="obj">Parent game object.</param>
		/// <param name="fileName">Resource file name with path, relative to Rersources folder.</param>
		public UIObject Add(UIType type, GameObject obj, string fileName, Sprite spr = null)
		{
			Log.Info("Add: <color=yellow>"+obj.name+" - id:"+obj.GetComponent<GUIText>()+"</color> type:"+type);

			//// Log.GameTimes("_______________-_-_-_-_-_ <color=yellow>"+fileName+" _____ obj: "+obj+"</color>, type: "+type);


			if(images.ContainsKey(obj.GetInstanceID()))
			{
			 	//give warning, then return existing one (ignore new registration)
			 	Log.Debug("This button already registered, ignoring new registration. <color=cyan>" + obj.name+"</color> ");
			 	return images[obj.GetInstanceID()];
			}

			Sprite[] sprList = new Sprite[1];
			if(fileName.Length == 0)
			{

				if(spr == null)
				{
					Log.Debug("This button <color=yellow>"+obj.name+"</color> has no image.");
				}
				else
				{
					Log.Debug("Sprite >>>>>>>>>>>>>>>>> "+spr.name);
					sprList[0] = spr;
				}
			}
			else
			{
				sprList = LoadSprite(fileName);
			}



			UIObject image = null;
			switch(type)
			{
				case UIType.Image:
					Log.Debug("Add UIImage");
					image = new UIImage(obj, sprList);
					break;

				case UIType.Button:
					Log.Debug("Add UIButton");
					image = new UIButton(obj, sprList);
					break;

				case UIType.ToggleButton:
					Log.Debug("Add UIToggleButton");
					image = new UIToggleButton(obj, sprList);
					break;
					
				case UIType.Slider:
					Log.Debug("Add UISlider");
					image = new UISlider(obj, sprList);
					break;

				case UIType.Character:
					Log.Debug("Add UICharacter");
					image = new UICharacter(obj, sprList);
					break;

				case UIType.ChartPie:
					Log.Debug("Add UIChartPie");
					image = new UIChartPie(obj);
					break;

				case UIType.ChartLine:
					Log.Debug("Add UIChartLine");
					image = new UIChartLine(obj);
					break;

				case UIType.Checkbox:
					Log.Debug("Add Checkbox");
					image = new UICheckbox(obj, sprList);
					break;

				default:
					Log.Exception("Invalid UIType to add:" + type);
					break;
			}

			//TODO remove this
			images.Add(obj.GetInstanceID(), image);
			
			//
			//images.Add(""+lastId++, image);
			//Log.Info("Button added:"+obj.name+" image:"+image.Name);

			return image;

		}//Add()


	    /// <summary>
	    /// Free up memory with dropping all ui objects.
	    /// Clears UI elements with removing them
	    /// from internal lists and destroys them.
	    /// </summary>
	    public void Clear(bool force = false)
	    {

	    	ClearUI(UIType.ALL);
	    	ClearScreens();

	    	if(force)
	    	{
	    		Debug.Log("Clear UI ************");
	    		//Disabled by default, since huds take care of reset event.	    	
	    		ClearHuds();
	    		ClearModels();
	    		ClearPopups();
	    		RemoveAllDialog();

	    		//remove ui root
	    		if(uiParent != null)
	    		{
	    			Object.Destroy(uiParent);
	    			uiParent = null;
	    		}
	    	}

	    }

		/// <summary>
		/// Remove all registered UIImages, where type is match
		/// </summary>
		public void ClearUI(UIType type)
		{
			List<int> dropList = new List<int>();

			foreach(KeyValuePair<int, UIObject> entry in images)
			{
				if(type == UIType.ALL
					|| type == entry.Value.GetType())
				{
					dropList.Add(entry.Key);
				}
			}
			foreach(int id in dropList)
			{
				images.Remove(id);
			}

			dropList.Clear();
			dropList = null;
			
		}//ClearUI()

		/// <summary>
		/// Add a new screen.
		/// </summary>
		/// <value>The screen to add.</value>
		public void AddScreen(UIScreen screen)
		{
			Log.Debug("Add screen: <color=yellow>"+screen.Name+"</color>");

			if(screens.ContainsKey(screen.Name))
			{
				throw new System.Exception("This screen already exist: <color=cyan>" + screen.name+"</color>");
			}

			//screen.SetActive(false);
			screens.Add(screen.Name, screen);

		}//AddScreen()

		/// <summary>
		/// Remove all registered screens.
		/// </summary>
		public void ClearHuds()
		{
			foreach(KeyValuePair<string, GameObject> entry in huds)
			{

			 	if(entry.Value != null)
				{
					Object.Destroy(entry.Value);
				}
			}

			huds.Clear();

		}//ClearScreens()

		public void ClearModels()
		{
			foreach(KeyValuePair<string, GameObject> entry in models)
			{
				
				if(entry.Value != null)
				{
					Object.Destroy(entry.Value);
				}
			}
			
			models.Clear();
			
		}

		public void ClearPopups()
		{
			foreach(KeyValuePair<string, GameObject> entry in popups)
			{
				if(entry.Value != null)
				{
					Object.Destroy(entry.Value);
				}
			}
			popups.Clear();
		}


		/// <summary>
		/// Remove all registered screens.
		/// </summary>
		public void ClearScreens()
		{
			foreach(KeyValuePair<string, UIScreen> entry in screens)
			{


			 	GameObject obj = entry.Value.gameObject;
			 	//Object.Destroy(obj);
			 	Object.DestroyImmediate(obj);
			}

			screens.Clear();

		}//ClearScreens()

		/// <summary>
		/// Activate a screen by name.
		/// This function will deactivat old screen and activate the new one.
		/// It will also send a notification to screenChanged(old, new) if any delegates is set.
		/// </summary>
		/// <param name="screenName">The name of the screen.</param>
		public UIScreen ActivateScreen(string screenName)
		{
			Log.Info("Activate screen:<color=yellow>"+screenName+"</color> leaving:"+activeScreen);

			//Log.Assert(screens.ContainsKey(screenName),
			// "No screen "+screenName+" registered in UIManager!");
			// 
			
			if(tooltip)
				tooltip.Hide();
			

			if(!screens.ContainsKey(screenName))
			{
				//try to add it from file
				string path = configResources + "UI/" + screenName;
				Log.Debug("Screen not yet exist, create it from file:"+path);
				Debug.Log("Screen not yet exist, create it from file:"+path);
				LoadResources(path, uiParent);
 
			}


			UIScreen oldScreen = activeScreen;

			if(oldScreen != null)
				oldScreen.gameObject.SetActive(false);


			//ClearUI(UIType.Image);
			//ClearUI(UIType.Button);

			activeScreen = screens[screenName];
			if(activeScreen != null)
			{
				activeScreen.gameObject.SetActive(true);
			}
			else
			{
				Log.Warning("??????? Where is this screen:"+screenName);
			}

			//send event about the change
			gpm.Event("SCREEN", "ENTER", screenName);

			return activeScreen;

		}//ActivateScreen()
		
		public UIScreen SlideScreen(string screenName)
		{
			Log.Info("Activate screen:<color=yellow>");


			if(tooltip)
				tooltip.Hide();
			
			if(!screens.ContainsKey(screenName))
			{
				//try to add it from file
				string path = configResources + "UI/" + screenName;
				Log.Debug("Screen not yet exist, create it from file:"+path);
				LoadResources(path, uiParent);
				
			}
			
			UIScreen newScreen = screens[screenName];
			//newScreen.gameObject.SetActive(true);
			
			//send event about the change
			gpm.Event("SCREEN", "ENTER", screenName);
			
			return activeScreen;
			
		}//ActivateScreen()

		/// <summary>
		/// Return the active screen Id
		/// </summary>
		public string GetScreenId()
		{
			return activeScreen.name;

		}//GetScreenId()

		/// <summmary>
		/// Get instance of a button
		/// </summary>
		public UIScreen GetScreen(string name)
		{
			if(!screens.ContainsKey(name))
			{
				throw new System.ArgumentException("Missing Screen name:"+name);
			}
			
			return screens[name];
						
		}//GetScreen()

		/// TODO remove or refactor..
		/// <summmary>
		/// Get instance of a button
		/// </summary>
		public UIObject Get(int id)
		{
			//int id =  System.Convert.ToInt32(name);
			if(!images.ContainsKey(id))
			{
				throw new System.ArgumentException("Missing UIImage/Button with name:"+id);
			}
			
			return images[id];
						
		}//Get()


		/// <summmary>
		/// Enable a button
		/// Helper function for delegate
		/// </summary>
		public static void EnableButton(UIButton b)
		{
			b.State = UIButtonState.RELEASE;
		}//EnableOkButton()

		/// <summmary>
		/// Disable a button
		/// Helper function for delegate
		/// </summary>
		public static void DisableButton(UIButton b)
		{
			b.State = UIButtonState.DISABLED;
		}//DisableButton()

		/// <summary>
		/// Check if the button already exist.
		/// </summary>
		public bool ContainsButton(int id)
		{
			return images.ContainsKey(id);

		}//ContainsButton()

		/// <summary>
		/// Check if the button already exist.
		/// </summary>
		public bool ContainsScreen(string name)
		{
			return screens.ContainsKey(name);

		}
		
		/// <summary>
		/// Check if the Popup already exist.
		/// </summary>
		public bool ContainsPopup(string id)
		{
			return popups.ContainsKey(id);
			
		}

		/// <summary>
		/// Check if the Hud already exist.
		/// </summary>
		public bool ContainsHud(string id)
		{
			return huds.ContainsKey(id);
		}

		/// <summary>
		/// Check if the Model already exist.
		/// </summary>
		public bool ContainsModel(string id)
		{
			return models.ContainsKey(id);
		}

		/// <summary>
		/// Add a dialog to available list
		/// </summary>
		public void AddDialog(UIDialog dialog)
		{
			if(!dialogs.ContainsKey(dialog.Id))
			{
				Log.Debug("ADD dialog:"+dialog.Id);
				dialogs.Add(dialog.Id, dialog);
			}
		}//AddDialog()

		/// <summary>
		/// Get a dialog from available list by id
		/// </summary>
		public UIDialog GetDialog(string id)
		{
			//Log.Debug("get id:"+id);

			if(dialogs.ContainsKey(id))
				return dialogs[id];
			else
				return null;
		}//GetDialog()

		/// <summary>
		/// Remove a dialog by id
		/// </summary>
		public void RemoveDialog(string id)
		{
			if(dialogs.ContainsKey(id))
			{
				dialogs.Remove(id);
				Log.Debug("REMOVE dialog:"+id);
			}
		}//RemoveDialog()

		/// <summary>
		/// Remove ALL dialogs
		/// </summary>
		public void RemoveAllDialog()
		{
			dialogs.Clear();
			
		}//RemoveAllDialogs()

		/// <summary>
		/// Remove an image type
		/// </summary>
		public void Remove(int id)
		{
			if(images.ContainsKey(id))
			{
				images.Remove(id);
				//Log.Debug("UI image Removed: "+id);
			}
			else
			{
				Log.Debug("Try to remove an none existing image:<color=yellow>"+id+"</color>");
			}
		}//Remove()



		/// <summary>
		/// Show and hide a layer by name
		/// If flag is set to true, it will show the layer
		/// if flag is false, it will hide the layer
		/// </summary>		
	    public static void ShowLayer(string layerName, bool flag) 
	    {

	    	Log.Debug("ShowLayer: "+layerName+","+flag+" uiCamera="+uiCamera);
	    	
	    	if(uiCamera == null)
	    	{
	    		uiCamera = uiCamera;
	    	}


	    	if(flag)
	    		uiCamera.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
	    	else
	    		uiCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));

	    }//ShowLayer()
	     
	    /// <summary>
	    /// Togle a layer by name
	    /// </summary>
	    public static void ToggleLayer(string layerName)
	    {
	    	if(uiCamera != null)
	    		uiCamera.cullingMask ^= 1 << LayerMask.NameToLayer(layerName);
	    }//ToggleLayer()

	    /// <summary>
	    /// Return if a layer is visible or not
	    /// </summary>
	    public static bool IsLayerVisible(string layerName)
	    {
	    	LayerMask mask = LayerMask.NameToLayer(layerName);
	    	return (uiCamera.cullingMask & (1 << mask.value)) != 0;
	    	
	    }//IsLayerVisible()

	    /// <summary>
	    /// Blink an image by numberOfBlinks times.
	    /// The speed of blinks depends on the speedInMs parameter.
	    /// </summary>
	    public void BlinkImage(UIImage image, int numberOfblinks = 2, double speedInMs = 200)
	    {
			if(image.blinkNumber == 0)
			{
				image.blinkNumber = numberOfblinks*2;
				image.blinkSpeed = speedInMs/1000;
				BlinkToggle(image);
			}
			else image.blinkNumber = numberOfblinks*2;

	    }//BlinkImage()
		
		
		private ScriptTrigger<UIImage> timeTrigger;
	    /// <summary>
	    /// Helper for processing blinking of an image.
	    /// It will toggle the visible flag based on the preset values
	    /// </summary>
	    public void BlinkToggle(UIImage image)
	    {
	    	//Log.Info("BlinkToggle:"+image.Name);

	    	//TODO use reference here!!

	    	//TODO show/hide, but could use sprites states as well
			//image.Visible = !image.Visible;
			if(image.Sprite == 0)
				image.Sprite = 1;
			else if(image.Sprite == 1)
				image.Sprite = 0;

	    	if(--image.blinkNumber > 0)
	    	{

		    	TimerCondition relTimeCondBlink = 
					new TimerCondition(TimerType.Relative, image.blinkSpeed, gpm.DialogGame.TimeSource);
				timeTrigger = 
					new ScriptTrigger<UIImage>(1, 1, relTimeCondBlink.Check, null, BlinkToggle, image);

				gpm.DialogGame.AddTrigger(timeTrigger);
			}
			
			if(image.blinkNumber == 0)
			{
				image.Sprite = 0;
				//image.Visible = true;
			}
	    }


	    /// <summary>
	    /// Use this function to show (activate) an object in modal mode.
	    /// While the object is active, it will capture all
	    /// user inputs and gets all focus.
	    /// Deactivating the object will trigger the uimanager to
	    /// go back to normal mode (hide and forget).
	    /// </summary>
	    public void ShowModal(GameObject obj)
	    {
	    	Log.Debug("ShowModal "+obj.name);

	    	modalWindow = obj;
	    	modalWindow.SetActive(true);
		
			modals = new Dictionary<int, GameObject>();
			Transform[] allChildren = modalWindow.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in allChildren)
			{
				if(!modals.ContainsKey(child.gameObject.GetInstanceID()))
				{
					modals.Add(child.gameObject.GetInstanceID(), child.gameObject);
				}
			}
		}//ShowModal

		public void RemoveModal()
		{
			modalWindow = null;
			modals.Clear();
		}

		
		/// <summary>
		/// Get name of used popups.
		/// </summary>
		public List<string> GetPopupNames(Dictionary<string,object> dict)
		{
			List<string> popups = new List<string>();
			
			foreach(Dictionary<string,object> obj
			        in ((List<object>)dict["objects"]))
			{
				if(obj.ContainsKey("type") 						
				   && obj.ContainsKey("id"))
				{
					string objType = obj["type"].ToString();
					if(objType == "popup")
					{
						//// Log.GameTimes("_______________-_-_-_-_-_-_______________ PopupNames: "+obj["id"].ToString());
						popups.Add(obj["id"].ToString());
					}
				}
			}
			return popups;
		}

	    /// <summary>
	    /// Get name of used huds.
	    /// </summary>
	    public List<string> GetHudNames(Dictionary<string,object> dict)
	    {
	    	List<string> huds = new List<string>();

	    	foreach(Dictionary<string,object> obj
					in ((List<object>)dict["objects"]))
				{
					if(obj.ContainsKey("type") 						
						&& obj.ContainsKey("id"))
					{
						string objType = obj["type"].ToString();
						if(objType == "hud")
						{
							huds.Add(obj["id"].ToString());
						}
					}
				}
			return huds;
	    }

   		/// <summary>
	    /// Create a screen from a json file
	    /// including all ui elements
	    /// </summary>
	    public void LoadResources(string fileName, GameObject gameObject = null)
		{
			
			Log.Info("AddScreen from json:"+fileName);

			Dictionary<string, object> dict = GameManager.LoadJSon(fileName);

			//create dialog triggers for each dialogs			
			foreach(Dictionary<string,object> d in ((List<object>)dict["screens"]))
			{
				Log.Debug("Create Screen:"+d["id"]);

				string id = d["id"].ToString();
				Log.Assert(id.Length > 0, "You must specify an id for a screen!");

				GameObject screenObject = new GameObject(id);
				if(gameObject)
					screenObject.transform.parent = gameObject.transform;

				Canvas canvas = screenObject.AddComponent<Canvas>();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				screenObject.AddComponent<UnityEngine.UI.CanvasScaler>();

				UnityEngine.UI.GraphicRaycaster gr =screenObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
				// screenObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
				// screenObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);

				screenObject.AddComponent<SortingOrder46>();
				screenObject.SetActive(false);

				GameObject eventSystem = GameObject.Find("EventSystem");
				if (eventSystem == null)
				{
					eventSystem = new GameObject("EventSystem");
					eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
					eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
					eventSystem.AddComponent<UnityEngine.EventSystems.TouchInputModule>();
				}

				string scriptName = d["script"].ToString();
#if UNITY5
				UIScreen screen = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(screenObject, "Assets/Takomat/epigenecore/Scripts/UI46/UIManager.cs (1315,23)", scriptName) as UIScreen;
#else
				UIScreen screen = screenObject.AddComponent(scriptName) as UIScreen;
#endif
				Log.Debug("Create sname:"+scriptName+"  Screen2:"+screen);
				screen.CreateScreen(d);
				AddScreen(screen);
			}		

		}//LoadResources()

		/// <summary>
		/// Load object resources from dictionary
		/// TODO make more generic helpers and way of parse similar stuff.
		/// ie. transform parameter should be one parser for all
		/// </summary>
		public Dictionary<string, UIObject> LoadResources(
			Dictionary<string,object> dict, 
		    GameObject                gameObject = null)
		{
			//parse list of objects
			if(dict.ContainsKey("objects"))
			{

				// Log.Debug("Number of objects:"+((List<object>)dict["objects"]).Count);

				Dictionary<string, UIObject> screenObjects = new Dictionary<string, UIObject>();
			
				foreach(Dictionary<string,object> obj
					in ((List<object>)dict["objects"]))
				{
					 
					//Log.Debug("objects:"+obj["id"].ToString()+" type:"+obj["type"].ToString());
	

					if(obj.ContainsKey("type") 
						&& obj.ContainsKey("id"))
					{
						//string objId = obj["id"].ToString();
						
						//get id and prefix it by parent name
						string objId = obj["id"].ToString();						
						if(gameObject!=null) objId = gameObject.name+"."+objId;

						
						string objType = obj["type"].ToString();						
						// Log.Debug("Create ui object:"+objId+":"+objType);

						
						//create objects based on types
						if(objType == "image")
						{
							// Log.Debug("Create Image:"+objId);
							Debug.Log("Create Image:"+objId + "go:" + gameObject.GetComponent<SortingOrder46>());
							//TODO add creation of an image gameObject here or there?

							UIImage image = new UIImage(obj, gameObject.GetComponent<SortingOrder46>());
							if(gameObject)
							{
							 	image.GameObject.transform.SetParent(gameObject.transform, false); //GameObject.Find("Canvas").transform;
							 	Debug.Log("+++image.order: " + image.order);
							 	image.UpdateSortingOrder();
							}

							image.Name = objId;
							images.Add(image.GameObject.GetInstanceID(), image);

							screenObjects.Add(obj["id"].ToString(),image);
						}
						else if(objType == "button")
						{
							
							Log.Debug("Create Button:<color=yellow>"+objId+"</color>");
							UIButton button = new UIButton(obj);
							if(gameObject)
							{
								button.GameObject.transform.SetParent(gameObject.transform, false);
							 	button.Name = objId;
							 	button.order = gameObject.GetComponent<SortingOrder46>();
							 	button.UpdateSortingOrder();

							}

							images.Add(button.GameObject.GetInstanceID(), button);
							screenObjects.Add(obj["id"].ToString(),button);
						}

						else if(objType == "togglebutton" )
						{
							Log.Debug("Create Toggle Button:<color=yellow>"+objId+"</color>");
							UIToggleButton button = new UIToggleButton(obj);
							if(gameObject)
							{
								button.GameObject.transform.SetParent(gameObject.transform, false);
								button.Name = objId;

							 	button.order = gameObject.GetComponent<SortingOrder46>();
							 	button.UpdateSortingOrder();
							}

							//TODO FIX_TOGGLE this hack! move button init to Settings.cs to Enter()
							// for SoundButton:
							// 	bool mutedMusic = AUDIO.AudioManager.Instance.MuteMusic;
							// 	bool mutedSfx = AUDIO.AudioManager.Instance.MuteSfx;
							// 	if(((mutedMusic) && (objId == "Settings.btn_Sound")) ||		// sets Sound button
							// 	   ((mutedSfx) && (objId == "Settings.btn_SoundEffects")))	// sets SoundEffects button
							// 	{
							// 		//button.SoundState = UISoundButton.UISoundButtonState.SOUNDOFF;
							// 		//button.SaveState = "Off";
							// 		button.Active = false;
							// 	}

							//images.Add(objId, button);
							images.Add(button.GameObject.GetInstanceID(), button);
							screenObjects.Add(obj["id"].ToString(),button);

						}
						else if(objType == "text")
						{
							
							Log.Debug("Create TEXT:<color=yellow>"+objId+"</color>");
							
							GameObject txt = new GameObject();
							if(gameObject)
							{
								txt.transform.SetParent(gameObject.transform, false);
							}
							txt.SetActive(false);
							txt.name = objId;
							UIText uiText = txt.AddComponent<UIText>();
							uiText.Parse(obj);
						 	// uiText.order = gameObject.GetComponent<SortingOrder46>();
						 	// uiText.UpdateSortingOrder();

							//images.Add(objId, text);
							txt.SetActive(true);
							screenObjects.Add(obj["id"].ToString(),uiText);
						}
						else if(objType == "popup")
						{
							//set prefab default to id and reset if defined							
							string prefab = (obj.ContainsKey("prefab")) 
								? obj["prefab"].ToString() : obj["id"].ToString();
							
							GameObject popup = GetPopup(obj["id"].ToString(), prefab);
							popup.SetActive(true);

							if(obj.ContainsKey("position"))
							{
								
								string pos = obj["position"].ToString();
								float px = System.Convert.ToSingle(pos.Split(',')[0]);
								float py = System.Convert.ToSingle(pos.Split(',')[1]);
								
								Log.Debug("HUD "+objId+" Position:"+px+","+py);
								popup.transform.position = new Vector3(px, py, 0);
								//this.gameObject.name = dict["position"].ToString();
							}
							if(obj.ContainsKey("rotation"))
							{
								
								string rot = obj["rotation"].ToString();
								float rx = System.Convert.ToSingle(rot.Split(',')[0]);
								float ry = System.Convert.ToSingle(rot.Split(',')[1]);
								
								//Log.Debug("Rotation:"+rot+":"+rx+","+ry);
								popup.transform.localRotation = Quaternion.Euler(new Vector3(rx, ry, 0));
							}
							if(obj.ContainsKey("scale"))
							{
								
								string sc = obj["scale"].ToString();
								float sx = System.Convert.ToSingle(sc.Split(',')[0]);
								float sy = System.Convert.ToSingle(sc.Split(',')[1]);
								
								popup.transform.localScale = new Vector3(sx, sy, 0);
								Log.Debug("Scale:"+sx+","+sy);
							}
							if(dict.ContainsKey("layer"))
							{
								string layer = dict["layer"].ToString();
								popup.layer = LayerMask.NameToLayer(layer);
								ChangeLayers(popup, layer);
								
							}
						}
						else if(objType == "hud")
						{

							//set prefab default to id and reset if defined							
							string prefab = (obj.ContainsKey("prefab")) 
									? obj["prefab"].ToString() : obj["id"].ToString();
							
							//Log.Debug("Create HUD as:"+prefab);
							GameObject hud = GetHud(obj["id"].ToString(), prefab);
							hud.SetActive(true);

							//Log.Warning("dict"+dict["layer"].ToString());
							if(obj.ContainsKey("position"))
							{

								string pos = obj["position"].ToString();
								float px = System.Convert.ToSingle(pos.Split(',')[0]);
								float py = System.Convert.ToSingle(pos.Split(',')[1]);

								Log.Debug("HUD "+objId+" Position:"+px+","+py);
								hud.transform.position = new Vector3(px, py, 0);
								//this.gameObject.name = dict["position"].ToString();
							}
							if(obj.ContainsKey("rotation"))
							{

								string rot = obj["rotation"].ToString();
								float rx = System.Convert.ToSingle(rot.Split(',')[0]);
								float ry = System.Convert.ToSingle(rot.Split(',')[1]);

								//Log.Debug("Rotation:"+rot+":"+rx+","+ry);
								hud.transform.localRotation = Quaternion.Euler(new Vector3(rx, ry, 0));
							}
							if(obj.ContainsKey("scale"))
							{
								string[] sc = obj["scale"].ToString().Split(',');
								float sx = System.Convert.ToSingle(sc[0]);
								float sy = System.Convert.ToSingle(sc[1]);
								float sz = 0;
								if (sc.Length > 2)
								{
									sz = System.Convert.ToSingle(sc[2]);
								}

								hud.transform.localScale = new Vector3(sx, sy, sz);
								Log.Debug(string.Format("Scale: {0}, {1}, {2}", sx, sy, sz));
							}
							// if(obj.ContainsKey("order"))
							// {
							// 	int order = System.Convert.ToInt32(obj["order"].ToString());
							// 	SortingOrder = order;

							// }
							if(obj.ContainsKey("layer"))
							{
								string layer = obj["layer"].ToString();
								hud.layer = LayerMask.NameToLayer(layer);
								ChangeLayers(hud, layer);

							}

							
						}
						else if(objType == "model")
						{
							
							//set prefab default to id and reset if defined							
							string prefab = (obj.ContainsKey("prefab")) 
								? obj["prefab"].ToString() : obj["id"].ToString();
							
							//Log.Debug("Create HUD as:"+prefab);
							GameObject hud = GetModel(obj["id"].ToString(), prefab);
							hud.SetActive(true);
							
							//Log.Warning("dict"+dict["layer"].ToString());
							if(obj.ContainsKey("position"))
							{
								
								string pos = obj["position"].ToString();
								float px = System.Convert.ToSingle(pos.Split(',')[0]);
								float py = System.Convert.ToSingle(pos.Split(',')[1]);
								float pz = System.Convert.ToSingle(pos.Split(',')[2]);
								
								Log.Debug("Model "+objId+" Position:"+px+","+py+","+pz);
								hud.transform.position = new Vector3(px, py, pz);
								//this.gameObject.name = dict["position"].ToString();
							}
							if(obj.ContainsKey("rotation"))
							{
								
								string rot = obj["rotation"].ToString();
								float rx = System.Convert.ToSingle(rot.Split(',')[0]);
								float ry = System.Convert.ToSingle(rot.Split(',')[1]);
								
								//Log.Debug("Rotation:"+rot+":"+rx+","+ry);
								hud.transform.localRotation = Quaternion.Euler(new Vector3(rx, ry, 0));
							}
							if(obj.ContainsKey("scale"))
							{
								
								string sc = obj["scale"].ToString();
								float sx = System.Convert.ToSingle(sc.Split(',')[0]);
								float sy = System.Convert.ToSingle(sc.Split(',')[1]);
								
								hud.transform.localScale = new Vector3(sx, sy, 0);
								Log.Debug("Scale:"+sx+","+sy);
							}
							// if(obj.ContainsKey("order"))
							// {
							// 	int order = System.Convert.ToInt32(obj["order"].ToString());
							// 	SortingOrder = order;
							
							// }
							if(obj.ContainsKey("layer"))
							{
								string layer = obj["layer"].ToString();
								hud.layer = LayerMask.NameToLayer(layer);
								ChangeLayers(hud, layer);
								
							}
							
							
						}
						else if(objType == "character")
						{
							
							Log.Debug("Create Character:<color=yellow>"+objId+"</color>");
							UICharacter uiChar = new UICharacter(obj);
							if(gameObject)
							{
								uiChar.GameObject.transform.SetParent(gameObject.transform, false);
								// uiChar.GameObject.transform.parent = gameObject.transform;
								uiChar.Name = objId;

							 	uiChar.order = gameObject.GetComponent<SortingOrder46>();
							 	uiChar.UpdateSortingOrder();

							}

							//TODO we might want this later
							//NPCView npcview = uiChar.GameObject.AddComponent<NPCView>();
							//npcview.id = objId;
							//npcview.image = uiChar;

							//read bubble info if any
							//TODO move this somewhere else? Should a bubble exist outside of uichar?
							if(obj.ContainsKey("bubble"))
							{
								foreach(Dictionary<string,object> bobj
									in ((List<object>)obj["bubble"]))
								{
									//TODO parse text parameter. (as text section?) This part is parsed
									//by UIText already, so we should only pass it over.
									//size (of text area), font, fontSize, anchor, alignment, wrapsize
									if(bobj.ContainsKey("position"))
									{

										string pos = bobj["position"].ToString();
										float px = System.Convert.ToSingle(pos.Split(',')[0]);
										float py = System.Convert.ToSingle(pos.Split(',')[1]);
										uiChar.bubblePosition = new Vector3(px, py, 0);
										Log.Debug("HUD "+objId+" bubble.Position:"+px+","+py);
									}

									if(bobj.ContainsKey("scale"))
									{

										string sc = bobj["scale"].ToString();
										float sx = System.Convert.ToSingle(sc.Split(',')[0]);
										float sy = System.Convert.ToSingle(sc.Split(',')[1]);
										uiChar.bubbleScale = new Vector3(sx, sy, 1);
										//Log.Debug("---------- bubble.scale:"+sx+","+sy);
									}

									if(bobj.ContainsKey("tail"))
									{

										uiChar.bubbleTail = bobj["tail"].ToString();
										
									}

								}
							}
							


							images.Add(uiChar.GameObject.GetInstanceID(), uiChar);
							screenObjects.Add(obj["id"].ToString(),uiChar);

						}
						else if(objType == "skit")
						{
							UISkit uiSkit = new UISkit(obj);
							if(gameObject)
							{
								uiSkit.GameObject.transform.parent = gameObject.transform;
								uiSkit.Name = objId;
							}

							if(obj.ContainsKey("bubble"))
							{
								foreach(Dictionary<string,object> bobj in ((List<object>)obj["bubble"]))
								{
									//TODO parse text parameter. (as text section?) This part is parsed
									//by UIText already, so we should only pass it over.
									//size (of text area), font, fontSize, anchor, alignment, wrapsize
									if(bobj.ContainsKey("position"))
									{

										string pos = bobj["position"].ToString();
										float px = System.Convert.ToSingle(pos.Split(',')[0]);
										float py = System.Convert.ToSingle(pos.Split(',')[1]);
										uiSkit.bubblePosition = new Vector3(px, py, 0);
									}

									if(bobj.ContainsKey("scale"))
									{

										string sc = bobj["scale"].ToString();
										float sx = System.Convert.ToSingle(sc.Split(',')[0]);
										float sy = System.Convert.ToSingle(sc.Split(',')[1]);
										uiSkit.bubbleScale = new Vector3(sx, sy, 1);
									}

									if(bobj.ContainsKey("tail"))
									{
										uiSkit.bubbleTail = bobj["tail"].ToString();
									}
								}
							}

							images.Add(uiSkit.GameObject.GetInstanceID(), uiSkit);
							screenObjects.Add(obj["id"].ToString(), uiSkit);
						}
						else if(objType == "video")
						{
							// Debug.Log("---------------------------- -- " + gameObject);
							//create canvas
							
							GameObject canvas = new GameObject(objId);
							if(gameObject)
							{
							 	canvas.transform.parent = gameObject.transform;
							}

							UIVideo video = new UIVideo(canvas, obj);

							images.Add(video.GameObject.GetInstanceID(), video);
							screenObjects.Add(obj["id"].ToString(), video);
						}
					}
					else
					{
						Log.Error("Object missing id and/or type definition");
					}
				}

				return screenObjects;

			}

			return null;
		}//LoadResources()
		
		/// <summary>
		/// Helper to get the game object for a popup.
		/// TODOstatic??
		/// </summary>
		public GameObject GetPopup(string id, string prefab = null)
		{
			if(popups.ContainsKey(id))
			{
				return popups[id];
			}
			
			if(prefab == null)
				prefab = id;

			GameObject popup = MainGame.CreateGameObject(MainGame.popupPrefabs+prefab);
			popup.name = id;
			popup.transform.parent = hudParent.transform;
			//// Log.GameTimes("_______________-_-_-_-_-_-_______________ Popup: "+id+", "+MainGame.popupPrefabs+prefab);
			popups.Add(id,popup);
			
			return popup;
			
		}//GetPopup()

		/// <summary>
		/// Helper to get the game object for a hud.
		/// TODOstatic??
		/// </summary>
		public GameObject GetHud(string id, string prefab = null)
		{
			//Log.Debug("GetHud : " + id);
			
			if(hudParent == null)
			{
				Log.Warning("No parent set for huds, cannot load hud:"+id);
				return null;
			}
			
			
			if(huds.ContainsKey(id))
			{
				Log.Debug ("HUD found in list:" + id);
				return huds[id];
			}

			if(prefab == null)
				prefab = id;

			//create a new hud from prefab.
			//Log.GameTimes ("GetHud - Create : " + id);
			GameObject hud = MainGame.CreateGameObject(MainGame.hudPrefabs+prefab);
			hud.name = id;
			hud.transform.SetParent(hudParent.transform);
			huds.Add(id,hud);
			
			//Log.Debug ("HUD loaded:" + id);

			return hud;
			
		}//GetHud()

		/// <summary>
		/// Helper to get the game object for a hud.
		/// TODOstatic??
		/// </summary>
		public GameObject GetModel(string id, string prefab = null)
		{
			//Log.Debug("GetHud : " + id);
			
			if(modelParent == null)
			{
				Log.Warning("No parent set for models, cannot load hud:"+id);
				return null;
			}
			
			
			if(models.ContainsKey(id))
			{
				Log.Debug ("Model found in list:" + id);
				return models[id];
			}
			
			if(prefab == null)
				prefab = id;
			
			//create a new hud from prefab.
			//Log.GameTimes ("GetHud - Create : " + id);
			GameObject model = MainGame.CreateGameObject(MainGame.modelPrefabs+prefab);
			model.name = id;
			model.transform.parent = modelParent.transform;
			models.Add(id,model);
			
			//Log.Debug ("HUD loaded:" + id);
			
			return model;
			
		}//GetHud()

		/// <summary>
		/// Disable all the huds
		/// </summary>
		public void DisableHuds()
		{
			foreach(KeyValuePair<string,GameObject> key in huds)
			{
				GameObject obj = key.Value;
				if(obj)
					obj.SetActive(false);
			}
		}

		/// <summary>
		/// Create a texture from game object.
		/// It will clone the object and the camera,
		/// create a screenshot of the camera and save it.
		/// Set rect to the size of the new image.
		/// </summary>
		public static Texture2D CreateTexture(GameObject obj, Rect rect, float orthSize)
		{
			Vector3 oldPosition;
			bool 	oldState;
			int	  	oldLayer;

			//create a temp folder
			GameObject tmp = new GameObject();
			tmp.name = "VRCAMERA";
			
			//WebPlayerDebugManager.addOutput ("created VRCAMERA", 3);
			
			//create gameobject clone
			GameObject targetObject = (GameObject)GameObject.Instantiate(obj);
			
			ChangeLayers(targetObject, "Camera");
			targetObject.layer = LayerMask.NameToLayer("Camera");
			targetObject.transform.position = Vector3.zero;
			targetObject.transform.parent = tmp.transform;
			
			//workaround:
			//move real ScoreBar to Camera, shoot screenshot, move it back
			oldPosition 			= obj.transform.position;
			oldState 				= obj.activeSelf;
			oldLayer 				= obj.layer;
			obj.transform.position 	= new Vector3 (0, 50, 0);
			obj.SetActive(true);
			ChangeLayers (obj, "Default");

			
			//make sure all alphas are ok
			// if(targetObject.renderer != null
			// 	&& targetObject.renderer.material != null)
			// {
			// 	targetObject.renderer.material.color = Color.white;
			// }
			// else
			// {
			// 	WebPlayerDebugManager.addOutput("Missing renderer or material", 3);
			// }
			
			//WebPlayerDebugManager.addOutput ("Layer Changed", 3);
			
			Camera camera = GetVirtualCamera(orthSize);
			camera.transform.parent = tmp.transform;
			
			//save the camera image into png
			Texture2D texture = Screenshot(camera, rect);
			
			//remove camera, clone
			GameObject.Destroy(tmp);
			obj.transform.position = oldPosition;
			obj.SetActive (oldState);
			obj.layer = oldLayer;
			
			return texture;
			
		}//CreateTexture()
		
		/// <summary>
		/// Get a virtual camera, which can render the object alone.
		/// The new camera cloned from the current main camera
		/// or from a it's prefab in case of no main camera.
		/// </summary>
		public static Camera GetVirtualCamera(float orthSize)
		{		
			GameObject cobj = null;
			
			Camera camera = null;
			if(uiCamera)
			{
				Log.Debug("Use main camera");
				//cobj = (GameObject)GameObject.Instantiate(uiCamera);
				camera = (Camera)GameObject.Instantiate(uiCamera);
			}
			else
			{
				Log.Debug("Use Prefab camera");
				//create from prefab
				cobj = CreateGameObject("MainCamera");
				cobj.GetComponent<AudioListener>().enabled = false;
				cobj.transform.position = Vector3.zero;
				camera = cobj.GetComponent<Camera>();
			}
			
			camera.tag = "Camera";
			//workaround
			camera.transform.position = new Vector3 (0, 50, 0);
			camera.orthographicSize = orthSize;
			camera.backgroundColor = new Color (255F, 255F, 255F, 0F);
			//TODO
			//camera.cullingMask = 1 << LayerMask.NameToLayer("Camera");
			
			camera.Render();
			
			return camera;
		}//GetVirtualCamera()
		
		/// <summary>
		/// Save the rectangle from the camera view
		/// as a texture.
		/// </summary>
		public static Texture2D Screenshot(Camera camera, Rect rect)
		{
			
			//create temp textures, one for the camera to render
			RenderTexture renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 16, RenderTextureFormat.ARGB32);
			//and one for the file
			Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
			//Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
			
			//set renderTexture
			RenderTexture.active = renderTexture;
			//RenderTexture oldTexture = camera.targetTexture;
			camera.targetTexture = renderTexture;
			camera.Render();
			//read the content of the camera into the texture
			texture.ReadPixels(rect, 0, 0);
			texture.Apply();
			//release the renders
			RenderTexture.active = null;
			//camera.targetTexture = null;
			//camera.targetTexture = oldTexture;			
			
			return texture;
			
		}//Screenshot()
		
		/// <summary>
		/// Save a texture into a file as png
		/// </summary>
		public static void SaveTextureToFile( Texture2D texture, string fileName)
		{
			Log.Debug("Save texture to png:"+fileName);
			
			
			if(texture != null 
			   && fileName != null 
			   && fileName.Length > 0)
			{
				byte[] bytes = texture.EncodeToPNG();
				//only if running in native mode
				
				#if UNITY_WEBPLAYER
				//TODO save it somewhere or stream?
				#else
				System.IO.File.WriteAllBytes( fileName, bytes );
				#endif
				
			}
			
		}//SaveTextureToFile


		/// <summary>
		/// Helper to create a gameobject from a prefab.
		/// </summary>
		public static GameObject CreateGameObject(string prefab)
		{

			string path = "Game/Prefabs/"+prefab;
	

			GameObject obj = (GameObject)Resources.Load(path);
			if(!obj)
			{
				Debug.Log("Can't find prefab to load:"+path);
			}

			return (GameObject)GameObject.Instantiate(obj);
		
		}//CreateGameObject()

		/// <summary>
		/// Change layers in game objet 
		/// and in all of it's children
		/// </summary>
		public static void ChangeLayers(GameObject obj, string name)
	    {
	    	Log.Debug("------ Change "+obj.name+" layer to:"+name);

	    	obj.layer = LayerMask.NameToLayer(name);
		    foreach(Transform child in obj.transform)
		    {
		    	//child.gameObject.layer = LayerMask.NameToLayer(name);
		    	ChangeLayers(child.gameObject, name);
		    }
	    }//ChangeLayersInChildren

	    /// <summary>
	    /// Helper to load a prefab by name under specific parent
	    /// </summary>
		public GameObject LoadPrefab(string prefab, string name, GameObject parent)
		{
			GameObject obj = MainGame.CreateGameObject(prefab);
			obj.name = name;
			obj.transform.parent = parent.transform;

			return obj;
		}

	    /// <summary>
		/// Create a new push or toggle button
		/// </summary>
		public UIImage CreateImage(string id, string filename, GameObject parent, GameObject obj = null)
		{
			if (obj == null)
			{
				obj = new GameObject();
			}

			obj.SetActive(false);
			obj.transform.parent = parent.transform;
			obj.name = id;
			

			SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
			if(spriteRenderer == null)
				obj.AddComponent<SpriteRenderer>();
				
			Sprite[] sprList = UIManager.LoadSprite(filename);
			if(sprList.Length < 1)
				Log.Error("Cannot find sprite:"+filename);

			obj.SetActive(true);

			return (UIImage)Add(UIType.Image, obj, filename);
		}

	}//class UIManager

}//namespace

#endif //EPIGENE_UI_46
