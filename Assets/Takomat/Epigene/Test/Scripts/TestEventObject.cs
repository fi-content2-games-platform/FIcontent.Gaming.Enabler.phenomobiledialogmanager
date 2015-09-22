using UnityEngine;
using System.Collections;

using Epigene;
using Epigene.GAME;

public class TestEventObject : MonoBehaviour 
{
	public string eventType;
	public int eventCounter;

	// Use this for initialization
	void Start () 
	{
		eventCounter = 0;
	}

	void OnEnable()
	{
		GameManager.Instance.RegisterEventHandler(eventType, ProcessEvent, gameObject);
	}

	void OnDisable()
	{
		GameManager.Instance.RemoveEventHandler(eventType, ProcessEvent);
	}
	
	/// <summary>
	/// Test event handler will increment the event counter.
	/// </summary>	
	public void ProcessEvent(string id, string data)
	{
		Log.Debug("ProcessEvent "+gameObject.name);
		eventCounter++;
	}

	/// <summary>
	/// helper
	/// </summary>
	public void SetActive(bool flag)
	{
		gameObject.SetActive(flag);
	}
}
