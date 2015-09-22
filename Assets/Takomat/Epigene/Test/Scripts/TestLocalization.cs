using UnityEngine;
using System.Collections;

using Epigene.UI;

///<summary>
///	Test class to drive the I18nManager localization unit.
/// The test has 2 files, one without level and one with level sections.
///</summary>
public class TestLocalization : MonoBehaviour {

	private I18nManager i18;
	// Use this for initialization
	void Start () 
	{

		i18 = I18nManager.Instance;
		checkFileDb("Epigene/Config/localisation", "");
		Debug.Log("<color=green> - - - - - - - - - - - - </color>");
		checkFileDb("Epigene/Config/localisationLevel", "Menu");
	}

	///<summary>
	/// check the db file with section (level)
	///</summary>
	void checkFileDb(string fileName, string section)
	{
		i18.LoadDbFile(fileName);
		checkLangString("EN", section);
		checkLangString("DE", section);
	}//checkFileDb()

	///<summary>
	/// check the section with the given language
	///</summary>
	void checkLangString(string lang, string section)
	{
		i18.SetLanguage(lang);
		Debug.Log("1:"+i18.Get(section, "001"));
		Debug.Log("2:"+i18.Get(section, "002"));
	}//checkLangString()

}//class TestLocalization
