#if !EPIGENE_UI_46
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
using System.Collections.Generic;

using Epigene.GAME;

namespace Epigene.UI
{
	public class ExternalImageLoader : MonoBehaviour
	{
		public void Load(string name, UIImage image)
		{
			LoadExternalCoroutine(name, image);
		}

		//hack to avoid memory leadk of loading external files into textures
		//keep one local texture for loading per image.
		private Texture2D loaderTexture = new Texture2D(4096,4096);

		private void LoadExternalCoroutine(string name, UIImage image)
		{
			string url = "file://" + GameManager.Instance.mainGame.ExternalResourcePath + "/" + name + ".jpg";
			Debug.Log(name);
			WWW www = new WWW(url);
			
			if (string.IsNullOrEmpty(www.error))
			{
 				www.LoadImageIntoTexture(loaderTexture);

				Sprite sprite = Sprite.Create(loaderTexture, 
					new Rect(0, 0, loaderTexture.width, loaderTexture.height),
					// new Rect(0, 0, www.texture.width, www.texture.height),  //->this eat up memory!!
					new Vector2(0.5f, 0.5f), 100.0f);

				Sprite[] sprites = new Sprite[1]; 
				sprites[0] = sprite;
				image.SetSprites(sprites);
				image.Sprite = 0;
				
				DestroyImmediate(www.texture);
				
			}
			else
			{
				Debug.LogWarning(www.error);

				image.SetSprites(UIManager.LoadSprite(name));
				image.Sprite = 0;
			}


			www.Dispose();
			www = null;
		}
	}
	///<summary>
	///Image class for a simple image with multiple sprites
	///</summary>
	public class UIImage : UIBaseObject
	{

		/// <summary>
		/// number of elements
		/// </summary>
		public int Count
		{
			get { return sprites.Length;}
		}

		/// <summary>
		/// Current state of UIButton, specify which sprites to use
		/// </summary>
		public int Sprite
		{
			get 
			{
				return this.sprite;
			}
			set 
			{
				//Log.Debug("spr:"+sprites+" l:"+lockSprite);
				if(render == null || sprites == null || lockSprite)
					return;

				//Log.Debug("spr length:"+sprites.Length);
				if( sprites.Length <= value)
					return;
				render.sprite = sprites[value];
				this.sprite = value;
			}
		}
		private int sprite;

		/// <summary>
		/// sprite sheets of this image
		/// </summary>
		private Sprite[] sprites;

		/// <summary>
		/// Lock a sprite image
		/// and don't allow the change it
		/// by uimanager or others
		/// </summary>
		public bool LockSprite
		{
			get{ return lockSprite; }
			set{ lockSprite = value;}
		}
		protected bool lockSprite;

		/// <summary>
		/// Every image needs a sprite renderer
		/// </summary>
		protected SpriteRenderer render;




		/// <summary>
		/// Number of blinks requried
		/// </summary>
		public int blinkNumber;
		/// <summary>
		/// speed of blinking
		/// </summary>
		public double blinkSpeed;

		/// <summary>
		/// UIImage Ctor from basic elements
		/// </summary>
		//public UIImage(GameObject gameObject, Sprite[] sprites, int state)
		public UIImage(GameObject gameObject, Sprite[] sprites)
		{
			//Log.Debug(UIIMage ctor"+gameObject.name);

			if(sprites != null)
			{
				this.render = gameObject.GetComponent<SpriteRenderer>();
				// if(this.render == null)
				// 	Debug.LogError("Button must have a SpriteRenderer! Button:"+gameObject.name);
			}

			this.gameObject = gameObject;
			this.sprites = sprites;
			this.type = UIType.Image;
			this.Sprite = 0;//state;
			this.visible = true;
			this.lockSprite = false;

		}//ctor()

		/// <summary>
		/// Constructor for create ui image from dicitionary.		
		/// </summary>
		public UIImage(Dictionary<string,object> dict)
		{
			if(!dict.ContainsKey("type")
				|| !dict.ContainsKey("id"))
			{
				Log.Error("Invalid dictionary for UIImage.");
			
			}

			this.type = UIType.Image;
			this.Sprite = 0;
			this.visible = true;
			this.lockSprite = false;

			//TODO this might go out from here
			//create a gameObject 
			GameObject gameObject = new GameObject();
			this.gameObject = gameObject;
			
			Parse(dict);

		}//ctor()


		public void Load(string name)
		{
			if (GameManager.Instance.mainGame.ExternalResourcePath.Length == 0)
			{
				this.sprites = UIManager.LoadSprite(name);
				Sprite = 0;
			}
 			else
 			{
				ExternalImageLoader loader = this.gameObject.GetComponent<ExternalImageLoader>();
				if (loader == null)
				{
					loader = this.gameObject.AddComponent<ExternalImageLoader>();
				}
				loader.Load(name, this);
			}
		}



		/// <summary>
		/// Parse the content of the dictionary
		/// and create a new gameobjet with this parameters
		/// </summary>
		protected virtual void Parse(Dictionary<string,object> dict)
		{
			//read image specific stuff
			if(dict.ContainsKey("sprite"))
			{

				string path = dict["sprite"].ToString();
				this.render = this.gameObject.AddComponent<SpriteRenderer>();
				this.sprites = UIManager.LoadSprite(path);

				if(dict.ContainsKey("spriteNumber"))
					Sprite = int.Parse(dict["spriteNumber"].ToString());
				else
					Sprite = 0;
			}

			//get base stuff
			base.Parse(dict);

		}//Parse()


		/// <summary>
		/// Get the current sprite
		/// </summary>
		public Sprite GetSprite()
		{
			//Log.Info("GetSprite: "+sprite);
			return (sprites != null) ? sprites[sprite] : null;
		}//GetSprite()
	
		public void SetSprites(Sprite[] sprites)
		{
			this.sprites = sprites;
		}

	}//class UISprite
	
}//namespace

#endif //!EPIGENE_UI_46
