using UnityEngine;
using System.Collections;
using Epigene;
using Epigene.UI;

public class TestButton : MonoBehaviour
{
	private UIManager ui = UIManager.Instance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (ui != null)
			ui.Update();
	}

	void OnClick(string name)
	{
	}

	void OnRelease(string name)
	{
	}

	void OnOver(string name)
	{
	}
}
