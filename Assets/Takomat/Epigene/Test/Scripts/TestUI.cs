using UnityEngine;
using System.Collections;

using Epigene;
using Epigene.UI;



[ExecuteInEditMode]
public class TestUI : MonoBehaviour 
{

	public string lang = "EN";

	private I18nManager i18;

	public UIText uiText1;

	float val1 = 10.0f;
	// Use this for initialization
	void Awake () 
	{

		Log.Level = Log.LogLevel.INFO;
		i18 = I18nManager.Instance;
		i18.LoadDbFile("Epigene/Test/Config/localisation");
		i18.SetLanguage(lang);
		
		
	}

	// Update is called once per frame
	void Update () 
	{
	
		//ui.Update();
		val1 += 0.0001f;
		uiText1.Text = val1.ToString();

		if(Application.isEditor)
		{
			I18nManager.Instance.SetLanguage(lang);
		}

	}

}//class TestUI
