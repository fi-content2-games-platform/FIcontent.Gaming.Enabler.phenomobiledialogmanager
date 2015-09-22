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
using System.Collections.Generic;
using Epigene;
using Epigene.UI;
using Epigene.GAME;
using Epigene.MODEL;
using Epigene.AUDIO;

//------------------------------------------------------------------------------
using Epigene.VIEW;


namespace TWISTBueBe
{
	///<summary>
	/// This class is responsible for create and manage the HUD menu system.
	/// It uses multiple sub menus and show/activate the one based on which
	/// is the activate menu at the moment.
	/// The users must click on the corrsponting menu button in the hud
	/// to activate the submenu.
	///</summary>
	public class HUD : MonoBehaviour 
	{
		/// delegate function for construction build function
		/// position is the location of the placement, 
		/// type define the construction model
		public delegate void BuildConstructionFunction(
			Vector3 position, string type);

		/// <summary>
		/// Add delegate to notify the world
		/// about construction building
		/// </summary>
		/// <value>The function to call.</value>
		private BuildConstructionFunction buildFunction;

		public void AddBuildConstructionFunction(BuildConstructionFunction func)
		{
			buildFunction += func;
		}

		public void RemoveBuildConstructionFunction(BuildConstructionFunction func)
		{
			buildFunction -= func;
		}

//------------------------------------------------------------------------------
		///ui elements to bind to this class
		public 	GameObject mainMenu;
		public 	GameObject buildMenu;
		public  UIText txtBuildMenu;
		public 	GameObject financeMenu;
		public  UIText txtFinanceMenu;
		public 	GameObject staticsMenu;
		public  UIText txtStaticsMenu;
		public 	Button buildButton;
		public 	Button financeButton;
		public 	Button statisticsButton;
		public 	UIText financeYear;
		public 	Image financeArrow;
		public 	Button btnMap;
		//public GameObject cityHallButton;

		public GameObject buildInfoPanel;
		public GameObject cursorIcon_Pipe;
		public GameObject cursorIcon_Filter;

		private double 	lastSewageChargeAddition = 0;
		private double 	lastWaterTariffAddition = 0;
		private int		popupDecision = 0;
		private int		buildConfirmation = 0;
		public 	Slider	finance_Slider;
		public 	Slider	drinkingWater_Slider;
		public 	Slider	sewage_Slider;

		/// <summary>
		/// The user interface manager.
		/// </summary>
		private UIManager uiManager;

		/// <summary>
		/// Player manager
		/// </summary>
		private GameManager gpm;


		///<summary>
		/// predefined states for submenus
		/// NONE: no submenu
		/// BUILD: show build submenu
		/// FINANCE: show finance submenu
		///</summary>
		private enum SubMenu {NONE, BUILD, FINANCE, STATISTICS};
		private SubMenu activeMenu = SubMenu.NONE;

		private enum BuildCursor {NONE, PIPE, FILTER};
		private BuildCursor activeCursor = BuildCursor.NONE;
		private bool mouseDown = false;

		private string spritePath = "Game/Sprites/HUD/";

		private BuildInfo buildInfo;


		/// <summary>
		/// Camera layer mask to save current mask
		/// to temporarly disable the screen
		/// </summary>
		private int lastLayerMask;

		void LogCyan(string _first, string _second)
		{
#if DEBUG
			string logstr = "No <color=cyan>";
			logstr += _first;
			logstr += "</color> assigned to:<color=cyan>";
			logstr += _second;
			logstr += "</color>";			
			Debug.Log (logstr);
#endif
		}
//------------------------------------------------------------------------------
		///<summary>
		///
		///</summary>
		void Awake()
		{
			Log.Debug(gameObject.name+" AWAKEN");

			//get managers
			uiManager = UIManager.Instance;
			gpm = GameManager.Instance;

			//TODO dynamic creation from file		
			if (mainMenu == null) LogCyan("mainMenu", gameObject.name);
			if (buildMenu == null) LogCyan("buildMenu", gameObject.name);
			if (txtBuildMenu == null) LogCyan("txtBuildMenu", gameObject.name);
			if (financeMenu == null) LogCyan("financeMenu", gameObject.name);
			if (txtFinanceMenu == null) LogCyan("txtFinanceMenu", gameObject.name);
			if (staticsMenu == null) LogCyan("staticsMenu", gameObject.name);
			if (txtStaticsMenu == null) LogCyan("txtStaticsMenu", gameObject.name);
			if (buildButton == null) LogCyan("buildButton", gameObject.name);
			if (financeButton == null) LogCyan("financeButton", gameObject.name);
			if (statisticsButton == null) LogCyan("statisticsButton", gameObject.name);
			if (cursorIcon_Pipe == null) LogCyan("cursorIcon_Pipe", gameObject.name);
			if (cursorIcon_Filter == null) LogCyan("cursorIcon_Filter", gameObject.name);
			if (btnMap == null) LogCyan("btnMap", gameObject.name);

			txtBuildMenu.Text = I18nManager.Instance.Get("POPUP_BUTTON", "004");
			txtFinanceMenu.Text = I18nManager.Instance.Get("POPUP_BUTTON", "005");
			txtStaticsMenu.Text = I18nManager.Instance.Get("POPUP_BUTTON", "006");

		}//Awake()

		/// <summary>
		/// Enable the HUD will register ui elements
		/// </summary>
		void OnEnable()
		{
			Log.Debug(gameObject.name+" Enabled");


			//add triggers?
			Log.Assert(buildInfoPanel, "Missing BuildInfoPanel in "+gameObject.name);
			buildInfo = buildInfoPanel.GetComponent<BuildInfo>();
			Log.Assert(buildInfo, 
			           "Missing buildInfo component from "
			           + buildInfoPanel.name+" in" + gameObject.name);
			
			//get financial model
			//financeModel = F0000_Finance.Instance;

			mainMenu.SetActive(true);	
			//register event handler			
			GameManager.Instance.RegisterEventHandler("SCREEN", ProcessScreenEvent);
			GameManager.Instance.RegisterEventHandler("HUD", ProcessHudEvent);
			GameManager.Instance.RegisterEventHandler("DIALOG", ProcessDialogEvent);
			GameManager.Instance.RegisterEventHandler("TIMELINE", ProcessTimelineEvent);
			GameManager.Instance.RegisterEventHandler("POPUP_DECISION", ProcessPopupEvent);

			GameObject obj = GameObject.Find("SimulationMenu/menu/HUD.002");
			if(obj!=null) 
			{
				//TODO Does not work with UIText - WHY??
				UIText numberOfPeople = obj.GetComponent<UIText>();
				numberOfPeople.Text = H0000_Population.Instance.NumberOfPeople.ToString(); 
				//WebPlayerDebugManager.addOutput("Text changed: " + numberOfPeople.Text, 3);
			}

			btnMap.SetActive(false);

		}//OnEnable()

		/// <summary>
		/// Disable the hud will remove registered ui elements
		/// </summary>
		void OnDisable()
		{
			Log.Debug(gameObject.name+" Disabled");
			
			GameManager.Instance.RemoveEventHandler("SCREEN", ProcessScreenEvent);
			GameManager.Instance.RemoveEventHandler("HUD", ProcessHudEvent);
			GameManager.Instance.RemoveEventHandler("DIALOG", ProcessDialogEvent);
			GameManager.Instance.RemoveEventHandler("TIMELINE", ProcessTimelineEvent);
			GameManager.Instance.RemoveEventHandler("POPUP_DECISION", ProcessPopupEvent);

		}//OnDisable()

		/// <summary>
		/// Start up
		/// </summary>
		void Start()
		{
			Log.Debug(gameObject.name + " Start");

		}//Start()

//------------------------------------------------------------------------------
		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void ProcessScreenEvent(string _eventId, string _param)
		{
			string eventIdP0006_balance = "P0006_balance";
			string eventIdH0000_numberOfPeople = "numberOfPeople";

			// // Log.GameTimes(eventType+ ": EVENT("+eventType+"): " + _eventId+","+_param);


			if (eventIdP0006_balance == _eventId) 
			{
				GameObject obj = GameObject.Find ("SimulationMenu/menu/HUD.003");
				// Log.Assert(obj, "Missing Finance.currentWaterRate in" + gameObject.name);

				//Log.Debug("FIRE : "+id);
				//menu = GameObject.Find("HUD/menu/HUD.003");
				if (obj != null) 
				{
					//Log.Assert(menu, "Missing menu in "+gameObject.name);
					// else 
					UIText currentBalance = obj.GetComponent<UIText> ();

					// Format must be done in the editor attributes
					//currentBalance.format = "{0:F2} €";
					currentBalance.Text = _param;
				}
			}			

			if (eventIdH0000_numberOfPeople == _eventId) 
			{
				GameObject obj = GameObject.Find ("SimulationMenu/menu/HUD.002");
				// Log.Assert(obj, "Missing Finance.eventIdH0000_numberOfPeople in" + gameObject.name);


				//Log.Debug("FIRE : "+id);
				//menu = GameObject.Find("HUD/menu/HUD.003");
				if (obj != null) 
				{
					//Log.Assert(menu, "Missing menu in "+gameObject.name);
					// else 
					UIText numberOfPeople = obj.GetComponent<UIText> ();
					numberOfPeople.Text = _param; 
					//WebPlayerDebugManager.addOutput ("Text changed: " + _param + ", " + numberOfPeople.Text, 3);
				}

			}	

		} 

		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void ProcessDialogEvent(string _eventId, string _param)
		{
				DialogManager.Instance.HUD (_eventId, _param);
		} 

		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void ProcessTimelineEvent(string _eventId, string _param)
		{

			if (_eventId == "SIM")
			{
				if (_param == "NewYear")
					financeYear.Text = GameManager.Instance.Get ("SIM").Time ().Year.ToString ();
				else if (_param == "Start")
					financeYear.Text = GameManager.Instance.Get ("SIM").Time ().Year.ToString ();
			}
		}

		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void ProcessHudEvent(string _eventId, string _param)
		{
			if (_eventId == "financeArrow")
				financeArrow.image.Sprite = int.Parse(_param);
		}

//------------------------------------------------------------------------------


//------------------------------------------------------------------------------
		/// <summary>
		/// Update used to receive mouse events.
		/// </summary>
		void Update()
		{
			if(activeCursor != BuildCursor.NONE)
			{
				btnMap.SetActive(true);

				if(activeCursor == BuildCursor.PIPE)
				{
					cursorIcon_Pipe.transform.position = UIManager.UICamera.ScreenToWorldPoint(Input.mousePosition);
				}
				else if(activeCursor == BuildCursor.FILTER)
				{
					cursorIcon_Filter.transform.position = UIManager.UICamera.ScreenToWorldPoint(Input.mousePosition);
				}
			}
			else
			{
				btnMap.SetActive(false);
			}
		}//Update()
//------------------------------------------------------------------------------
		///<summary>
		/// Toggle the build menu and start corresponding musics.		
		///</summary>
		//public void ToggleBuildMenu(UIButton button)
		public void ToggleBuildMenu(string button)
		{
			Log.Debug("showBuildMenu");
			if(activeCursor != BuildCursor.NONE)
				ClickOnButtonInsteadOfMap();
			
			if(activeMenu == SubMenu.BUILD)
			{
				buildMenu.SetActive(false);
				txtBuildMenu.FontColor = new Color32(136, 137, 137, 255);
				activeMenu = SubMenu.NONE;
				UIManager.UICamera.cullingMask = lastLayerMask;	

				GameManager.Instance.Event("TIMELINE", "SIM", "Unpause");

				//UIManager.ShowLayer("Player", true);
				uiManager.RemoveModal();
				return;
			}
			else if(activeMenu == SubMenu.FINANCE)
			{
				//buildButton.UIButton.LockSprite = false;
				//buildButton.UIButton.State = UIButtonState.RELEASE;
				financeMenu.SetActive(false);
				txtFinanceMenu.FontColor = new Color32(136, 137, 137, 255);
				financeButton.Up();
				UIManager.UICamera.cullingMask = lastLayerMask;
				//return;
			}
			else if(activeMenu == SubMenu.STATISTICS)
			{
				staticsMenu.SetActive(false);
				txtStaticsMenu.FontColor = new Color32(136, 137, 137, 255);
				statisticsButton.Up();
				UIManager.UICamera.cullingMask = lastLayerMask;
				//return;
			}

			//cityHallButton.SetActive(false);
			buildMenu.SetActive(true);
			txtBuildMenu.FontColor = new Color32(225, 225, 225, 255);
			activeMenu = SubMenu.BUILD;
			//TODO play music/sfx
			UIManager.Instance.ShowModal(gameObject);

			lastLayerMask = UIManager.UICamera.cullingMask;
			//UIManager.UICamera.cullingMask = 1 << LayerMask.NameToLayer("Menu");
			
			GameManager.Instance.Event("TIMELINE",
			                           "SIM", 
			                           "Pause");

			//UIManager.ShowLayer("Player", false);
		}//showBuildMenu()
//------------------------------------------------------------------------------
		///<summary>
		/// Toggle the Financial menu and play the corresponding sound/music.
		///</summary>
		public void ToggleFinanceMenu(string button)
		{
			if(activeCursor != BuildCursor.NONE)
				ClickOnButtonInsteadOfMap();
			
			if(activeMenu == SubMenu.FINANCE)
			{
				financeMenu.SetActive(false);
				txtFinanceMenu.FontColor = new Color32(136, 137, 137, 255);
				activeMenu = SubMenu.NONE;
				UIManager.UICamera.cullingMask = lastLayerMask;

				//TODO Decision Popup IN FinanceMenu

				{
					GameManager.Instance.Event("TIMELINE", 
					                           "SIM", 
					                           "Unpause");
				}

				uiManager.RemoveModal();
				return;
			}
			else if(activeMenu == SubMenu.BUILD)
			{
				buildMenu.SetActive(false);
				txtBuildMenu.FontColor = new Color32(136, 137, 137, 255);
				buildButton.Up();
				activeMenu = SubMenu.NONE;
				UIManager.UICamera.cullingMask = lastLayerMask;
				//return;
			}
			else if(activeMenu == SubMenu.STATISTICS)
			{
				staticsMenu.SetActive(false);
				txtStaticsMenu.FontColor = new Color32(136, 137, 137, 255);
				statisticsButton.Up();
				UIManager.UICamera.cullingMask = lastLayerMask;
				//return;
			}
			

			//cityHallButton.SetActive(false);
			financeMenu.SetActive(true);
			txtFinanceMenu.FontColor = new Color32(255, 255, 255, 255);
			activeMenu = SubMenu.FINANCE;
			UIManager.Instance.ShowModal(gameObject);
			//if (finance_Slider != null) finance_Slider.Value = (float) financeManager.ChargeAddition;

			
			lastLayerMask = UIManager.UICamera.cullingMask;
			//UIManager.UICamera.cullingMask = 1 << LayerMask.NameToLayer("Menu") | 1 << LayerMask.NameToLayer("Popup");
			//UIManager.UICamera.cullingMask = 2 << LayerMask.NameToLayer("Popup");
			
			GameManager.Instance.Event("TIMELINE", 
			                           "SIM", 
			                           "Pause");
			
			// UIManager.ShowLayer("Player", false);
		}//showFinanceMenu()

		private IEnumerator ToggleFinanceMenuCoroutine(string button)
		{
			while (popupDecision == 0)
			{
				yield return new WaitForSeconds(0.1f);
				//WebPlayerDebugManager.addOutput("waiting", 1);
			}
			//0: not decided, 1: yes, 2: no
			if (popupDecision == 1)
			{
				//WebPlayerDebugManager.addOutput("yes", 1);
				GameManager.Instance.Event("TIMELINE", 
				                           "SIM", 
				                           "Unpause");
			}
			else if (popupDecision == 2)
			{

				GameManager.Instance.Event("TIMELINE", 
				                           "SIM", 
				                           "Unpause");
			}	
		}
//------------------------------------------------------------------------------
		///<summary>
		/// Toggle the Statistics menu and play the corresponding sound/music.
		///</summary>
		public void ToggleStatisticsMenu(string button)
		{
			if(activeCursor != BuildCursor.NONE)
				ClickOnButtonInsteadOfMap();

			if(activeMenu == SubMenu.STATISTICS)
			{
				staticsMenu.SetActive(false);
				txtStaticsMenu.FontColor = new Color32(136, 137, 137, 255);
				activeMenu = SubMenu.NONE;
				UIManager.UICamera.cullingMask = lastLayerMask;
				
				//if(statisticsButton.UIButton.State == UIButtonState.ACTIVE_RELEASE)
				if(((UIToggleButton)statisticsButton.UIButton).Active)
					statisticsButton.Up();

				GameManager.Instance.Event("TIMELINE", 
				                           "SIM", 
				                           "Unpause");
				
				uiManager.RemoveModal();
				//UIManager.ShowLayer("Player", true);
				return;
			}
			else if(activeMenu == SubMenu.BUILD)
			{
				buildMenu.SetActive(false);
				txtBuildMenu.FontColor = new Color32(136, 137, 137, 255);
				buildButton.Up();
				UIManager.UICamera.cullingMask = lastLayerMask;
				//return;
			}
			else if(activeMenu == SubMenu.FINANCE)
			{
				financeMenu.SetActive(false);
				txtFinanceMenu.FontColor = new Color32(136, 137, 137, 255);
				financeButton.Up();
				UIManager.UICamera.cullingMask = lastLayerMask;
			}

			//cityHallButton.SetActive(false);
			staticsMenu.SetActive(true);
			txtStaticsMenu.FontColor = new Color32(225, 225, 225, 255);
			activeMenu = SubMenu.STATISTICS;
			//TODO play music/sfx
			
			/*
			lastLayerMask = UIManager.UICamera.cullingMask;
			UIManager.UICamera.cullingMask = 1 << LayerMask.NameToLayer("Menu");
			
			GameManager.Instance.Event("TIMELINE",
			                           "SIM", 
			                           "Pause");

			*/

			//cityHallButton.SetActive(false);
			staticsMenu.SetActive(true);
			txtStaticsMenu.FontColor = new Color32(255, 255, 255, 255);
			UIManager.Instance.ShowModal(gameObject);
			activeMenu = SubMenu.STATISTICS;
			
			lastLayerMask = UIManager.UICamera.cullingMask;
			//UIManager.UICamera.cullingMask = 1 << LayerMask.NameToLayer("Menu");
			
			GameManager.Instance.Event("TIMELINE", 
			                           "SIM", 
			                           "Pause");


			/*ScoreBoard scoreBoard = GameObject.Find ("ScoreBoard").GetComponent<ScoreBoard>();
			ScoreBoardChart scoreBoardChart = null;
			GameObject go = UnityEngine.GameObject.Find("ScoreBoardChart");
			if ((scoreBoardChart == null) && (go!=null))
				scoreBoardChart = go.GetComponent<ScoreBoardChart>();
			if (scoreBoardChart != null)
				scoreBoardChart.Evaluate(
					scoreBoard.socialHistory, 
				    scoreBoard.economyHistory, 
		            scoreBoard.ecologyHistory);

			WaterLossChart waterLossChart = null;
			go = UnityEngine.GameObject.Find("WaterLossChart");
			if ((waterLossChart == null) && (go!=null))
				waterLossChart = go.GetComponent<WaterLossChart>();
			if (waterLossChart != null)
				waterLossChart.Evaluate(
					financeManager.waterLossHistory);*/

			// UIManager.ShowLayer("Player", false);
			
		}//showFinanceMenu()
		//------------------------------------------------------------------------------


		/// <summary>
		/// Build a construction.
		/// This function is called when the user clicks
		/// on the "BUILD" button in the build selection panel.
		/// The function will change the icon to the selected
		/// consturction model and will hide the build panels.
		/// Parameter is the name of the button.
		/// </summary>
		public void BuildConstruction(string param)
		{
			activeMenu = SubMenu.NONE;
			buildMenu.SetActive(false);
			buildInfoPanel.SetActive(false);
			buildButton.Up();
			txtBuildMenu.FontColor = new Color32(136, 137, 137, 255);

			lastLayerMask = UIManager.UICamera.cullingMask;
			UIManager.UICamera.cullingMask =  1 << LayerMask.NameToLayer("Map") 
											| 1 << LayerMask.NameToLayer("Menu");
			
			string textureName ="";
			if(param == "btn_Build_1")
			{
				//textureName = "cursor_PipeSystem_50px";
				cursorIcon_Pipe.SetActive(true);
				cursorIcon_Pipe.transform.position = UIManager.UICamera.ScreenToWorldPoint(Input.mousePosition);
				activeCursor = BuildCursor.PIPE;
			}
			else if (param == "btn_Build_2")
			{
				//textureName = "cursor_SmallFilterPlant_50px";
				cursorIcon_Filter.SetActive(true);
				cursorIcon_Filter.transform.position = UIManager.UICamera.ScreenToWorldPoint(Input.mousePosition);
				activeCursor = BuildCursor.FILTER;
			}

			textureName = "Game/Sprites/HUD/symb_Cursor_Build"; //+ textureName;

			Log.Info("textureName:"+textureName);
			Vector2 hotSpot = Vector2.zero;
			Texture2D cursorTexture = Resources.Load<Texture2D>(textureName);
			Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
			mouseDown = true;
	
			//TODO remove this once Map draws on layer MAP only!
			//UIManager.ShowLayer("Default", true);

		}//BuildConstruction()
//------------------------------------------------------------------------------
		/// <summary>
		/// Open the Build information panel
		/// when the user click on the info icon.
		/// param holds the name of the button,
		/// which is used to determine the type
		/// of the construction item.
		/// </summary>
		public void OpenInfoPanel(string param)
		{
			//Log.Warning("OpenInfoPanel "+ param);
			

			if(param == "btn_Info_1")
				buildInfo.Type = ConstructionType.PIPE;
			else if(param == "btn_Info_2")
				buildInfo.Type = ConstructionType.SMALLFILTER;
			else if(param == "btn_Info_3")
				buildInfo.Type = ConstructionType.GREYWATERFILTER;
			else if(param == "btn_Info_4")
				buildInfo.Type = ConstructionType.BIOGASPLANT;
			else if(param == "btn_Info_5")
				buildInfo.Type = ConstructionType.TREATMENTPLANT;
			else
			{
				Log.Exception("Invalid button in "+
				              gameObject.name+".OpenInfoPanel:" + param);
			}

			buildInfoPanel.SetActive(true);
			
		}//OpenInfoPanel()
//------------------------------------------------------------------------------
		/// <summary>
		/// Close the info panel when the user clicks
		/// on the close button.
		/// </summary>
		public void CloseInfoPanel(string param)
		{
			//Log.Warning("CloseInfoPanel: "+ param);
			buildInfoPanel.SetActive(false);

		}//CloseInfoPanel

		/// <summary>
		/// Is called when player is in build-mode
		/// but doesn't click on map but on a button instead.
		/// </summary>
		private void ClickOnButtonInsteadOfMap()
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			activeCursor = BuildCursor.NONE;
			
			cursorIcon_Pipe.SetActive(false);
			cursorIcon_Filter.SetActive(false);

			UIManager.UICamera.cullingMask = lastLayerMask;
			
			GameManager.Instance.Event("TIMELINE", 
			                           "SIM", 
			                           "Unpause");
		}

		private void ClickOnMap()
		{
			if(activeCursor != BuildCursor.NONE)
			{
				Vector3 point = Input.mousePosition;
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

				cursorIcon_Pipe.SetActive(false);
				cursorIcon_Filter.SetActive(false);

				buildConfirmation = 0;
				//Save the changes
				GameManager.Instance.Event("POPUP_DECISION", 
				                           "007", 
				                           "show");
				UIManager.ShowLayer("Popup", true);
				StartCoroutine("BuildComfirmation", point);					
			}
		}

		private IEnumerator BuildComfirmation(Vector3 point)
		{
			while (buildConfirmation == 0)
			{
				yield return new WaitForSeconds(0.1f);
				//WebPlayerDebugManager.addOutput("waiting", 1);
			}
			//0: not decided, 1: yes, 2: no
			if (buildConfirmation == 1)
			{
				//WebPlayerDebugManager.addOutput("yes", 1);
				string buildItem = 
					(activeCursor  == BuildCursor.PIPE) ? "PIPE" : "FILTER";

				if (activeCursor == BuildCursor.FILTER)
				{
					GameManager.Instance.Event("SIMULATION",
					                           "CONSTRUCTION",
					                           "E0006");
				}
				else if (activeCursor == BuildCursor.PIPE)
				{
					GameManager.Instance.Event("SIMULATION",
					                           "CONSTRUCTION",
					                           "E0001");
				}
				Log.Info(
					string.Format(
					"BUILD {0} ON MAP AT POSITON: {1}", 
					buildItem, point));
				activeCursor = BuildCursor.NONE;

				//notify the world about building
				if(buildFunction != null)
					buildFunction(point, buildItem);
				
				cursorIcon_Pipe.SetActive(false);
				cursorIcon_Filter.SetActive(false);
				//UIManager.ShowLayer("Player", true);
				UIManager.UICamera.cullingMask = lastLayerMask;
				
				GameManager.Instance.Event("TIMELINE", 
				                           "SIM", 
				                           "Unpause");
				yield return true;
			}
			else if (buildConfirmation == 2)
			{
				//WebPlayerDebugManager.addOutput("no", 1);

				activeCursor = BuildCursor.NONE;
				cursorIcon_Pipe.SetActive(false);
				cursorIcon_Filter.SetActive(false);
				//UIManager.ShowLayer("Player", true);
				UIManager.UICamera.cullingMask = lastLayerMask;
				
				GameManager.Instance.Event("TIMELINE", 
				                           "SIM", 
				                           "Unpause");
				yield return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateFinanceSlider(float param)
		{

			
		}//UpdateFinanceSlider	

		/// <summary>
		/// WasteWaterSlider
		/// </summary>
		public void UpdateWasteWater(float param)
		{


			
		}//UpdateFinanceSlider	

		/// <summary>
		/// WasteWaterSlider
		/// </summary>
		public void UpdateDrinkingWater(float param)
		{


			
		}//UpdateFinanceSlider	

		/// <summary>
		/// Logic for process event from popup system
		/// </summary>
		void ProcessPopupEvent(string eventId, string data)
		{
			//WebPlayerDebugManager.addOutput("ProcessPopupEvent " + eventId + " " + data, 1);
			if(eventId == "006")
			{
				if(data == "yes")
				{
					popupDecision = 1;
				}
				else if(data == "no")
				{
					popupDecision = 2;
				}
			}

			if(eventId == "007")
			{
				if(data == "yes")
				{
					buildConfirmation = 1;
				}
				else if(data == "no")
				{
					buildConfirmation = 2;
				}
			}
		}//ProcessPopupEvent()

	}//class HUD

}//namespace
//------------------------------------------------------------------------------
