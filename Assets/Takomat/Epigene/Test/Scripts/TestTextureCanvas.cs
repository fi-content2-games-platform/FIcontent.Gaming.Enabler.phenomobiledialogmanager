using UnityEngine;
using System.Collections;

using Epigene.UI;

public class TestTextureCanvas : MonoBehaviour 
{

	public GameObject sourceObject;

	private Texture2D texture;

	// Use this for initialization
	void Start () 
	{
		 UIManager ui = UIManager.Instance;

		if(sourceObject != null)
		{
			//sourceObject.SetActive(true);			
		}
	
	}
	
	void OnMouseDown()
	{
		texture = UIManager.CreateTexture(sourceObject, new Rect(0,0,150,150), 0.67f);
		//texture = Screenshot(new Rect(0,0,150,150));
		//texture = Screenshot(new Rect(0,0,1008,756));
		//if(texture == null)
		//	Debug.LogError("NO TEXTURE");

		byte[] imageBytes = texture.EncodeToPNG();
		Texture2D img = new Texture2D(150, 150);
		img.LoadImage(imageBytes);
		
		//texture = Resources.Load("test") as Texture2D;
		GetComponent<Renderer>().material.mainTexture = img;
		GetComponent<Renderer>().material.color = Color.white;
		GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
		//renderer.material.shader = Shader.Find("Transparent/Diffuse");

		//string fileName = Application.persistentDataPath + "/testSavePNG3.png";
		//UIManager.SaveTextureToFile(texture, fileName);
		//Debug.Log("Saved:"+fileName);
	}
			
	public static Texture2D Screenshot(Rect rect)
	{


		Camera camera = Camera.main;
		//Camera camera = 



		//create temp textures, one for the camera to render
		RenderTexture renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 16, RenderTextureFormat.ARGB32);
		//and one for the file
		Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
		//Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);



		//set renderTexture
		RenderTexture.active = renderTexture;
		RenderTexture oldTexture = Camera.main.targetTexture;
		camera.targetTexture = renderTexture;
		camera.Render();
		//read the content of the camera into the texture
		texture.ReadPixels(rect, 0, 0);
		//texture.Apply();
		//release the renders
		RenderTexture.active = null;
		//camera.targetTexture = null;
		camera.targetTexture = oldTexture;			

		return texture;

	}//Screenshot()

	// Update is called once per frame
	void Update () 
	{
		
	}
}
