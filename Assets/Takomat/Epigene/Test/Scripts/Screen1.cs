using UnityEngine;
using System.Collections;

using Epigene.UI;
using Epigene.GAME;

namespace Epigene.UITest
{


	/// <summary>
	/// Game Screen1. This class responsible to create and manage this screen.
	/// Disabling this script will disable any ui processing on this screen.
	/// </summary>
	public class Screen1 : MonoBehaviour 
	{

		// static ui definitions
		public GameObject bgScreen;
		public GameObject btnMir;

		/// <summary>
		/// The user interface manager.
		/// </summary>
		private UIManager uiManager;

		/// <summary>
		/// Player manager
		/// </summary>
		private GameManager gpm;


		/// <summary>
		/// Destroy this object
		/// </summary>
		void OnDisable() 
		{
        	Debug.Log("<color=red>"+gameObject.name+" disabled.</color>");
    	}//OnDestroy()

		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake()
		{
			Debug.Log("<color=cyan>"+gameObject.name+"</color> AWAKEN");

			//get managers
			uiManager = UIManager.Instance;
			gpm = GameManager.Instance;

			//TODO dynamic creation from file
			//uiManager.CreateFromFile("Config/screen");
			if (btnMir == null) Debug.LogError("No <color=cyan>btnMir</color> assigned to:<color=cyan>"+gameObject.name+"</color>");
			if (bgScreen == null) Debug.LogError("No <color=cyan>bgScreen</color> assigned to:<color=cyan>"+gameObject.name+"</color>");
			
			//use static creation for now
			//uiManager.AddButton(btnMir, "Sprites/btn_erzaehls_mir", nextScreen, null /*testOnRelease*/, null /*testOnOver*/);
			UIButton button = (UIButton)uiManager.Add(UIType.Button, btnMir, "Sprites/btn_erzaehls_mir");
			button.Register(UIEvent.Click, nextScreen);


		}//Awake()

		// Update is called once per frame
		void Update () 
		{
			//uiManager.Update();
			uiManager.Update();
		
		}//Update()

		/// <summary>
		/// go to next screen
		/// </summary>
		public void nextScreen(UIButton button)
		{
			Debug.Log("Test click :"+button.Name);

			uiManager.ActivateScreen("Screen2");


		}//nextScreen()

		/// <summary>
		///Test function to process release even from buttons
		/// </summary>
		public void testOnRelease(UIButton button)
		{
			Debug.Log("Test release: "+button.Name);
		}//testOnRelease()

		/// <summary>
		/// test function to process over state from buttons
		/// </summary>
		public void testOnOver(UIButton button)
		{
			Debug.Log("Test over :"+button.Name);
		}//testOnOver()

	}//class Screen1

}//namesapce