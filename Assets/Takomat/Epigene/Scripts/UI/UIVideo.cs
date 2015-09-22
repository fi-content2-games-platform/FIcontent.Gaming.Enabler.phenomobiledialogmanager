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

using Epigene;
using Epigene.GAME;

namespace Epigene.UI
{
#if UNITY_STANDALONE || UNITY_WEBPLAYER
	public class ExternalVideoLoader : MonoBehaviour
	{
		//flag to indicate if video was loaded already
		private bool loadedFlag = false;
		private string myname="";


		/// <summary>
		/// Start the screen
		/// </summary>
		void Start()
		{
			//WebPlayerDebugManager.addOutput("_______ ExternalVideoLoader Start url: ", 1);
			loadedFlag = false;
			myname="";
		}//Start()


		public void Load(string name, UIVideo video)
		{
			//we only create new video for the first load!
			if(((!loadedFlag)&&(name!=myname))||(name!=myname))
			{
				loadedFlag = true;
				myname = name;
				StartCoroutine(LoadExternalCoroutine(name, video));
			}
		}

		IEnumerator LoadExternalCoroutine(string name, UIVideo video)
		{
			//load only once
			string url = "file://" + GameManager.Instance.mainGame.ExternalResourcePath + "/" + name + ".ogv";
			// Debug.Log(">>>>>>> Load file:" + url);
			//WebPlayerDebugManager.addOutput("_______ LoadExternalCoroutine url: "+url, 1);

			//release resources for www
			DestroyImmediate(video.movie);
			DestroyImmediate(video.GameObject.GetComponent<Renderer>().material.mainTexture);
			video.movie = null;

			WWW www = new WWW(url);

			// while (!www.movie.isReadyToPlay);

			yield return www;
			
			if (string.IsNullOrEmpty(www.error))
			{
				video.movie = www.movie;
				video.GameObject.GetComponent<Renderer>().material.mainTexture = video.movie;
				video.GameObject.GetComponent<Renderer>().material.color = Color.white;
				video.movie.Play(); //hack: get the first frame
				video.movie.Stop();
			}
			else
			{
				Debug.LogWarning(www.error);
				MovieTexture t = Resources.Load<MovieTexture>(name);
				if (t != null)
				{
					video.movie = t;
					video.GameObject.GetComponent<Renderer>().material.mainTexture = video.movie;
				}
			}

			www.Dispose();
			www = null;
		}
	}
#endif

	public class UIVideo : UIBaseObject 
	{

#if UNITY_STANDALONE || UNITY_WEBPLAYER
		/// <summary>
		/// video texture to play
		/// </summary>	
		public MovieTexture movie;
#endif

		/// <summary>
		/// canvas obj to render the movie
		/// </summary>
		public GameObject Canvas
		{
			get {return gameObject;}
		}

		/// <summary>
		/// return true if the video 
		/// is loaded and ready to be play
		/// </summary>
		public bool IsReady
		{
			//TODO add audio
			get {
#if UNITY_STANDALONE || UNITY_WEBPLAYER		
				if(movie != null)
					return movie.isReadyToPlay; 
				else
					return false;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// return true if 
		/// the video is currently playing
		/// </summary>
		public bool IsPlaying
		{
			//TODO add audio
			get {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
				if (movie != null)
					return movie.isPlaying;
				else
					return false;
#else
				return false;
#endif		
			}
		}


		//private MeshRenderer renderer;

		/// <summary>
		/// default ctor
		/// </summary>
		public UIVideo(GameObject gameObject)
		{
			this.gameObject = gameObject;
			//this.gameObject.renderer.sortingOrder = 200;

		}//ctor

		/// <summary>
		/// ctor for create from dictionary
		/// </summary>
		public UIVideo(GameObject gameObject, Dictionary<string,object> dict)
		{
			WebPlayerDebugManager.addOutput("Create Video!", 1);
			if(!dict.ContainsKey("type")
				|| !dict.ContainsKey("id"))
			{
				Log.Error("Invalid dictionary for UIVideo.");
			
			}

			this.gameObject = gameObject;
			this.type = UIType.Video;			
			this.visible = true;

			//force to use sprite/default so sorting order will work
			gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");

			//force to have 100% color (not 50% transparency)
			gameObject.GetComponent<Renderer>().material.color = Color.white;

			Parse(dict);
			
			WebPlayerDebugManager.addOutput("Created Video!", 1);
		}//ctor()

		/// <summary>
		/// Parse the content of the dictionary
		/// and create a new gameobjet with this parameter
		/// </summary>
		public override void Parse(Dictionary<string,object> dict)
		{

			if(dict.ContainsKey("file"))
			{
				Load(dict["file"].ToString());
			}
			
			//get base stuff
			base.Parse(dict);
		}//Parse()

		/// <summary>
		/// Load a video from streaming assets
		/// </summary>
		public void Load(string name)
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			if (GameManager.Instance.mainGame.ExternalResourcePath.Length == 0
				|| !System.IO.File.Exists(GameManager.Instance.mainGame.ExternalResourcePath + "/" + name + ".ogv"))
			{
				MovieTexture t = Resources.Load<MovieTexture>(name);
				if (t != null)
				{
					movie = t;
					this.gameObject.GetComponent<Renderer>().material.mainTexture = movie;
	 			}
			}
 			else
 			{
				ExternalVideoLoader loader = this.gameObject.GetComponent<ExternalVideoLoader>();
				if (loader == null)
				{
					loader = this.gameObject.AddComponent<ExternalVideoLoader>();
				}
				loader.Load(name, this);
			}
			Log.GameTimes("_________________ video - load complete");
#else
			Log.Warning("MovieTexture is disabled");
#endif
		}//Load()

		/// <summary>
		/// Play the video
		/// </summary>
		public void Play()
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			Log.GameTimes("Play()");
			if(movie && movie.isReadyToPlay)
			{
				Log.GameTimes("ready:"+movie.isReadyToPlay);
				movie.Play();
			}
#endif
		}//Play()

		/// <summary>
		/// Stop the video
		/// </summary>
		public void Stop()
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			if(movie)
				movie.Stop();
#endif
		}//Stop()
		
		/// <summary>
		/// Pause the video
		/// </summary>
		public void Pause()
		{
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			if(movie)
				movie.Pause();
#endif
		}//Pause()


	}//class
}//namespace

#endif //!EPIGENE_UI_46
