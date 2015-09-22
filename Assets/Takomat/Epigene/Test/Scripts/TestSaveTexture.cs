using UnityEngine;
using System.Collections;


using Epigene.UI;

namespace TestSaveTexture
{
	public class TestSaveTexture : MonoBehaviour 
	{

		public GameObject obj;
		//Texture2D texture;
		public Camera camera;

		Camera virtualCamera;

		string layerName = "virtual";

		

		bool capture = true;

		int imageSize = 512;

		private GameObject targetObject;

		// Use this for initialization
		void OnEnable () 
		{

			string fileName = Application.persistentDataPath + "/testSavePNG2.png";

			//create a virtual camera as instance of the current camera
			Texture2D texture = UIManager.CreateTexture(obj, new Rect(0,0,56,56), 0.67f);
			UIManager.SaveTextureToFile(texture, fileName);

			
			Debug.Log("DONE:"+fileName);			
	

		}

		void OnDisable()
		{

			//Destroy(camera);
			//Destroy(targetObject);
			//camera = null;
		}

	}//class
}