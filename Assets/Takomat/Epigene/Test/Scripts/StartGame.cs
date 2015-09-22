using UnityEngine;
using System.Collections;

using Epigene.GAME;
using Epigene.UI;

namespace Epigene.UITest
{

	/// <summary>
	/// Start the game. Add each screens to the UIManager and set the first one to active.
	/// </summary>
	public class StartGame : MonoBehaviour 
	{

		public GameObject startText;
		public UIScreen screen1;
		public UIScreen screen2;
		public string lang;

		private I18nManager i18nManager;
		private GameManager gm;

		// Use this for initialization
		void Awake() 
		{
			Debug.Log("<color=cyan>START GAME</color>");

			if (startText == null) Debug.LogError("No <color=cyan>startText</color> assigned to:<color=cyan>"+gameObject.name+"</color>");
			if (screen1 == null) Debug.LogError("No <color=cyan>screen1</color> assigned to:<color=cyan>"+gameObject.name+"</color>");
			if (screen2 == null) Debug.LogError("No <color=cyan>screen2</color> assigned to:<color=cyan>"+gameObject.name+"</color>");

			UIManager ui = UIManager.Instance;
			UIManager.ScreenChanged = notifyScreenChanged;

			ui.AddScreen(screen1);
			ui.AddScreen(screen2);
			ui.ActivateScreen(screen1.name);

			startText.SetActive(false);

			//localisation
			i18nManager = I18nManager.Instance;
			i18nManager.SetLanguage(lang);

			//add an empty main game			
			GMGame game = new GMGame("TestGame");
			gm = GameManager.Instance;

			ACondition falseCond = new FalseCondition();
			ACondition trueCond = new TrueCondition();
			
			ACondition relTimeCond = new TimerCondition(TimerType.Absolute, 8, game.TimeSource);  //true after 1sec
			//this will only be fired, when both fireCondition is true at the same time
			ScriptTrigger<string> trigger3 = new ScriptTrigger<string>();
			trigger3.Value = "TestGame";
			trigger3.Function = TestScriptFunctionString;
			trigger3.Priority = 1;
			trigger3.Repeat = 1;							//only fire once
			trigger3.FireCondition += trueCond.Check;		//always true, but you can check sg extra here
			trigger3.FireCondition += relTimeCond.Check;	//should fire only when time reached
			trigger3.DeleteCondition = falseCond.Check;		//don't remove until repeating
			gm.Add (game);
			game.AddTrigger(trigger3);
			Debug.Log (" ----> AddTrigger 3");
			gm.Start(game.Name);			
		}//Start()

		/// <summary>
		/// Test script processing string value.
		/// </summary>
		/// <param name="value">Value as string</param>
		public void TestScriptFunctionString(string value)
		{
			Debug.Log (" ----> Test string value="+value);
			
		}//TestScriptFunctionString()

		// test function to notify us when screen changes
		public void notifyScreenChanged(UIScreen oldScreen, UIScreen newScreen)
		{
			string tmp = (oldScreen != null) ? oldScreen.Name : "NONE";
			Debug.Log("Swtich screen <color=cyan>"+tmp+" -> "+newScreen.Name+"</color>");
		}//notifyScreenChanged()

		/// <summary>
		/// Update called by every frame
		/// </summary>
		public void Update()
		{
			//update game state
			gm.Update();
			
		}//Update

	}//class StartGame

}//namespace
