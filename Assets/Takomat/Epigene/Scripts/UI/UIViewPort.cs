//------------------------------------------------------------------------------
// Copyright (c) 2014-2015 takomat GmbH and/or its licensors.
// All Rights Reserved.

// The coded instructions, statements, computer programs, and/or related material
// (collectively the "Data") in these files contain unpublished information
// proprietary to takomat GmbH and/or its licensors, which is protected by
// German federal copyright law and by international treaties.

// The Data may not be disclosed or distributed to third parties, in whole or in
// part, without the prior written consent of takoamt GmbH ("takomat").

// THE DATA IS PROVIDED "AS IS" AND WITHOUT WARRANTY.
// ALL WARRANTIES ARE EXPRESSLY EXCLUDED AND DISCLAIMED. TAKOMAT MAKES NO
// WARRANTY OF ANY KIND WITH RESPECT TO THE DATA, EXPRESS, IMPLIED OR ARISING
// BY CUSTOM OR TRADE USAGE, AND DISCLAIMS ANY IMPLIED WARRANTIES OF TITLE,
// NON-INFRINGEMENT, MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE OR USE.
// WITHOUT LIMITING THE FOREGOING, TAKOMAT DOES NOT WARRANT THAT THE OPERATION
// OF THE DATA WILL gameengine_dialogsmanagerBE UNINTERRUPTED OR ERROR FREE.

// IN NO EVENT SHALL TAKOMAT, ITS AFFILIATES, LICENSORS BE LIABLE FOR ANY LOSSES,
// DAMAGES OR EXPENSES OF ANY KIND (INCLUDING WITHOUT LIMITATION PUNITIVE OR
// MULTIPLE DAMAGES OR OTHER SPECIAL, DIRECT, INDIRECT, EXEMPLARY, INCIDENTAL,
// LOSS OF PROFITS, REVENUE OR DATA, COST OF COVER OR CONSEQUENTIAL LOSSES
// OR DAMAGES OF ANY KIND), HOWEVER CAUSED, AND REGARDLESS
// OF THE THEORY OF LIABILITY, WHETHER DERIVED FROM CONTRACT, TORT
// (INCLUDING, BUT NOT LIMITED TO, NEGLIGENCE), OR OTHERWISE,
// ARISING OUT OF OR RELATING TO THE DATA OR ITS USE OR ANY OTHER PERFORMANCE,
// WHETHER OR NOT TAKOMAT HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH LOSS
// OR DAMAGE.
//------------------------------------------------------------------------------
// This class is part of the epigene(TM) Software Framework.
// All license issues, as above described, have to be negotiated with the
// takomat GmbH, Cologne.
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

using Epigene;

namespace Epigene.UI
{

	/// <summary>
	/// This class responsible for managing a view port.
	/// It shows a portion of an image, wihthin it's viewport boundary, 
	/// using a "virtual" camera.
	/// This camera can be moved whithin the image and create 
	/// different effects such as scrolling, zooming, rortating.	
	/// </summary>
	[ExecuteInEditMode]
	public class UIViewPort : MonoBehaviour 
	{

		public GameObject targetImage;


		//private Quad quad;
		private GameObject quadObject;	//this is the viewport area or "canvas" where we render the picture from the camera
		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;
		private Mesh quadMesh;
		//private Sprite targetImage;  //this is what we want to see (look at with the camera)
		private Camera camera;			//virtual camera
		private float screenRatio;

		private Material material;		//material to connect the camera and view port
//		private DirectionalLight light;
		private string defaultSprite = "Epigene/Test/Sprites/speechBubbleEnd";

		///
		private GameObject cameraObj;
		private RenderTexture viewportTexture;


		/// <summary>
		/// Create a mesh with given width and height
		/// </summary>
	    Mesh CreateMesh(string name, float width, float height)
	    {
		    Mesh m = new Mesh();
		    m.name = name;
		    m.vertices = new Vector3[] 
		    {
			    new Vector3(-width, -height, 0.01f),
			    new Vector3(width, -height, 0.01f),
			    new Vector3(width, height, 0.01f),
			    new Vector3(-width, height, 0.01f)
		    };

		    m.uv = new Vector2[] 
		    {
			    new Vector2 (0, 0),
			    new Vector2 (0, 1),
			    new Vector2(1, 1),
			    new Vector2 (1, 0)
		    };

		    m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		    m.RecalculateNormals();
		     
		    return m;
	    }

		/// <summary>
		/// Create required components
		/// </summary>
		void Awake()
		{


			screenRatio = gameObject.transform.localScale.x / gameObject.transform.localScale.y;

			//create render Texture
			viewportTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
			Log.Assert(viewportTexture, "Cannot create viewportTexture!");

			//create quad for view port
			//we use this as the screen for the virtual camera
			//by attaching the render texture
			quadObject = GameObject.Find(gameObject.name+"/Screen");
			if(quadObject == null)
			{
				quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
				quadObject.transform.parent = gameObject.transform;
				quadObject.transform.localScale = gameObject.transform.localScale;
				quadObject.name = "Screen";

				if(Application.isEditor)
					quadObject.GetComponent<Renderer>().sharedMaterial.mainTexture = viewportTexture;
				else
					quadObject.GetComponent<Renderer>().material.mainTexture = viewportTexture;
				
			}

			//create virutal camera which render into viewportTexture
			cameraObj = GameObject.Find(gameObject.name+".Camera");
			if(cameraObj == null)
			{
				Log.Info("Create camera");
				cameraObj = new GameObject();
				cameraObj.name = gameObject.name+".Camera";
				//cameraObj.transform.parent = gameObject.transform;
				
				cameraObj.AddComponent<Camera>();
				//camera = cameraObj.AddComponent<Camera>();
			}
			camera = cameraObj.GetComponent<Camera>();

			camera.orthographic = true;				
			camera.orthographicSize = Camera.main.orthographicSize;
			camera.nearClipPlane = 0;
			camera.farClipPlane = 0.01f;

			//Rect r = new Rect(0,0,screenRatio, 1);
			//camera.rect =r;
			camera.aspect = screenRatio;
			camera.targetTexture = viewportTexture;
			

			MeshFilter mf = (MeshFilter) quadObject.GetComponent<MeshFilter>();

			Vector2[] uv = new Vector2[4];
			uv[0] = new Vector2(0, 0);
			uv[1] = new Vector2(screenRatio, 1);
			uv[2] = new Vector2(screenRatio, 0f); //x/y, 0
			uv[3] = new Vector2(0f, 1f);

			//if(Application.isEditor)
				mf.sharedMesh.uv = uv;
			//else
			//	mf.mesh.uv = uv;



		}

		// Use this for initialization
		void Start () 
		{
		
		}
		
		// Update is called once per frame
		void Update () 
		{

			//Update size of the object/screen and camera if size changed
			//quadObject.transform.localScale = gameObject.transform.localScale;
			screenRatio = gameObject.transform.localScale.x / gameObject.transform.localScale.y;

			if(camera != null)
			{
				Rect r = new Rect(0,0,screenRatio, 1);
				camera.rect = r;
				camera.aspect = screenRatio;

				//camera.orthographicSize = Camera.mainCamera.orthographicSize;

			}
			//cameraObj.transform.parent = targetImage.transform;
			//reposition camera to targetimage
			//
			//
			if(Application.isEditor)
				quadObject.GetComponent<Renderer>().sharedMaterial.mainTexture = viewportTexture;
			else
				quadObject.GetComponent<Renderer>().material.mainTexture = viewportTexture;



			MeshFilter mf = (MeshFilter) quadObject.GetComponent<MeshFilter>();

			Vector2[] uv = new Vector2[4];
			uv[0] = new Vector2(0, 0);
			uv[1] = new Vector2(screenRatio, 1);
			uv[2] = new Vector2(screenRatio, 0f); //x/y, 0
			uv[3] = new Vector2(0f, 1f);

			//if(Application.isEditor)
			mf.sharedMesh.uv = uv;
		}
	}//class UISCrollArea

}//namepsace