using UnityEngine;
using System.Collections;

using Epigene.UI;

namespace Epigene
{
	public class PlayVideo : MonoBehaviour 
	{
		public string fileName = "Test/TestMovie";

		//public MovieTexture movie;

		UIVideo video;

		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		// Use this for initialization
		void Start () 
		{

			MovieTexture movie = LoadFromResource(fileName);
			//MovieTexture movie = LoadFromStream(fileName);
			transform.GetComponent<Renderer>().material.mainTexture = movie;
			

		    movie.Play();
		}

		public MovieTexture LoadFromResource(string fileName)
		{
			
		    MovieTexture movie = Resources.Load<MovieTexture>(fileName);
		    if(movie == null )
		    	Debug.LogError("No movie!");

		    return movie;
			
		    //transform.audio.clip = movie.audioClip;
		    //transform.audio.Play();			
		}


		public MovieTexture LoadFromStream(string fileName)
		{
			string url = "file:///"+Application.streamingAssetsPath +"/"+ fileName;
			Log.Debug("Load movie from:"+url);

			WWW stream = new WWW(url);
			//yield return stream;
		
			MovieTexture movie = stream.movie;
			Log.Assert(movie != null, "Missing movie!");

			return movie;
		}
#endif
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}