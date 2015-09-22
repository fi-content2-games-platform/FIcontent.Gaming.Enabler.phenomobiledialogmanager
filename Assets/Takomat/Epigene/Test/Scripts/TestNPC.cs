using UnityEngine;
using System.Collections;
using Epigene;
using Epigene.UI;
using Epigene.MODEL;

public class TestNPC : MonoBehaviour
{
	private UIManager ui = UIManager.Instance;

	public GameObject npc1;


	private UICharacter char1;

	private SpeechBubble bubble;

	// Use this for initialization
	void Start () 
	{

		char1 = (UICharacter)ui.Add(UIType.Character, npc1, "Sprites/NPC/CoordLady");
		//char1 = (UICharacter)ui.Add(UIType.Character, npc1, "Sprites/NPC/Rentner");
	
		char1.Emotion = EmotionType.POSITIVE;

	}
	
	// Update is called once per frame
	void Update () {

		 //if (Input.GetMouseButton(0))
			//Log.Debug("down");

		//if (ui != null)
			//ui.Update();
	}

	void OnGUI()
	{
		

		if (GUI.Button(new Rect(10, 10, 100, 30), "Next"))
		{			
			NextEmotion();
		}

	}

	void NextEmotion()
	{
		switch(char1.Emotion)
		{

			case EmotionType.NEUTRAL:
			char1.Emotion = EmotionType.POSITIVE;
			break;

			case EmotionType.POSITIVE:
			char1.Emotion = EmotionType.NEGATIVE;
			break;
			
			case EmotionType.NEGATIVE:
			char1.Emotion = EmotionType.NEUTRAL;			
			break;
				
		}

		//enable bubble again
		if(bubble)
			bubble.gameObject.SetActive(true);
	}

	/// <summary>
	/// test for close bubble on click
	/// </summary>
	void BubbleClose(SpeechBubble obj)
	{

		Log.Info("Close clicked on:" + obj.Id);

		//keep reference
		bubble = obj;
		obj.gameObject.SetActive(false);

	}

	/// <summary>
	/// test function to handle click on arrow
	/// </summary>
	void BubbleArrow(SpeechBubble obj)
	{
		Log.Info("Arrow clicked in:"+obj.Id);
		//nothing to do  here
	}
	
}
