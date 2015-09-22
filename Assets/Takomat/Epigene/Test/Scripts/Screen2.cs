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
	public class Screen2 : MonoBehaviour 
	{

		// static ui definitions
		public GameObject bgScreen;
		public GameObject btnBye;

		/// <summary>
		/// The user interface manager.
		/// </summary>
		private UIManager uiManager;

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


			if (bgScreen == null) Debug.LogError("No bgScreen assigned to:"+gameObject.name);
			if (btnBye == null) Debug.LogError("No btnBye assigned to:"+gameObject.name);
			//use static creation for now
			uiManager = UIManager.Instance;
			//uiManager.AddButton(btnBye, "Sprites/btnBye", nextScreen, null, null);
			UIButton b = (UIButton)uiManager.Add(UIType.Button, btnBye, "Sprites/btnBye");
			b.Register(UIEvent.Click, nextScreen);


		}//Awake()

		// Update is called once per frame
		void Update () 
		{
			uiManager.Update();
		
		}//Update()

		/// <summary>
		/// go to next screen
		/// </summary>
		public void nextScreen(UIButton button)
		{
			uiManager.ActivateScreen("Screen1");
		}//nextScreen()


	}//class Screen1
}//namespace