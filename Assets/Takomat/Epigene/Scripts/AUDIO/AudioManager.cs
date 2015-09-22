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
//------------------------------------------------------------------------------

using Epigene;
using Epigene.UI;


namespace Epigene.AUDIO
{
	/// Software design issues:
	/// - This AudioManager is designed to maanage the Sound.
	///   Different approaches can be taken. we have chosen,
	///   that you can play a Sound directly with the AudioManager
	///   and with an Event system set up.
	/// - As the base Audio API of  FMOD. The AudioManager
	///   will be very similar.
	/// 
	/// - This manger can load files from a resource folder, in this case
	///   each id equals with the name of the audio file.
	///   Or Load only files which are in the localization file 
	///   and use the id from the localization db. (like AUDIO.XXX)
	/// - The manger use two AudioSource components. 
	///   One for the background music (audioBackground), which plays one at a time 
	///   and looped,
	///   and another one (audioSfx) for sound effects which can play more sounds
	///   simultaneously and not looped.

//------------------------------------------------------------------------------


	/// <summary>
	/// Sound manager class to manage sound sfx
	/// </summary>
	public sealed class AudioManager
	{

		
		/// <summary>
		/// Audio types: BACKGROUND is the type always played in the background.
		/// SFX are Button Click Sounds, an explosion sound ...
		/// </summary>
		public enum Type {BACKGROUND, SFX};
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static AudioManager  Instance
		{
			get{ return instance;}
		}
		private static readonly AudioManager instance = new AudioManager();

		/// <summary>
		/// mute all audio
		/// </summary>
		public bool Mute
		{
			get { return audioObject.activeInHierarchy; }
			set { audioObject.SetActive(value);	}
		}
		
		/// <summary>
		/// mute background music
		/// </summary>
		public bool MuteMusic
		{
			get { return audioBackground.mute; }
			set { if(audioBackground != null ) audioBackground.mute = value; }
		}

		/// <summary>
		/// mute all sound effects
		/// </summary>
		public bool MuteSfx
		{
			get { return audioSfx.mute; }
			set { audioSfx.mute = value; }
		}

		/// <summary>
		/// set the volume for the music
		/// </summary>
		public float backgroundVolume
		{
			get { return audioBackground.volume; }
			set { audioBackground.volume = value; }
		}

		/// <summary>
		/// set the volume for the sound effects
		/// </summary>
		public float SfxVolume
		{
			get { return audioSfx.volume; }
			set { audioSfx.volume = value; }
		}

		/// <summary>
		/// Other Audio sound than bg
		/// </summary>
		private Dictionary<string, AudioClip> clipList;

		/// <summary>
		/// GameObject to hold all audio Sources
		/// This way we can cache all audios
		/// for selected language and we can play them parallel.
		/// </summary>
		private GameObject audioObject;

		/// <summary>
		/// Audio source for background music
		/// </summary>		
		private AudioSource audioBackground;

		/// <summary>
		/// Audio source for mixed effects
		/// </summary>
		private AudioSource audioSfx;

		// Use this for initialization
		//void Start () 
		AudioManager()
		{

			Log.Debug("AudioManager initialized.");

			//Create the list for clips			
			clipList = new Dictionary<string, AudioClip>();

			//initialize the audio list
			// InitAudio();

		}//AudioManager()

		/// <summary>
		/// Initialize the Audio object.
		/// This will create required audio sources,
		/// if they are not yet exists.
		/// </summary>
		public void InitAudio()
		{
			
			clipList.Clear();

			// //create game object for audio sources
			// //it will keep all audio organized
			// //and independent from game objects
			audioObject = GameObject.Find("Audio");
			if(audioObject == null)
			{
					audioObject = new GameObject();
					audioObject.name = "Audio";
					//background music audio source
					audioBackground = audioObject.AddComponent<AudioSource>();
					audioBackground.loop = true;
					audioBackground.playOnAwake = false;
					audioBackground.volume = 1.0f;
					//sfx audio source
					audioSfx = audioObject.AddComponent<AudioSource>();
					audioSfx.loop = false;
					audioSfx.playOnAwake = false;
					audioSfx.volume = 1.0f;
		
			}
			else
			{
				AudioSource[] list = audioObject.GetComponents<AudioSource>();
				Log.Assert((list.Length == 2), "Audio object must have exactly 2 AudioSources!");

				audioBackground = list[0];
				audioSfx = list[1];
			}

		}//InitAudio()

		/// <summary>
		/// Load all audio resource file from a folder.
		/// The name of the audio files are used as the key.		
		/// </summary>
		public void LoadResourcesFromFolder(string resourcePath)
		{
			Log.Debug("AudioManager loading resources " + resourcePath);
			
			AudioClip[] clips = Resources.LoadAll<AudioClip>(resourcePath);
			if(clips == null && clips.Length == 0)
			{
				throw new System.ArgumentNullException("Cannot load audio: "+resourcePath);
			}

			foreach(AudioClip c in clips)
			{
				Log.Debug("Loaded audio: <color=yellow>"+c.name+"</color>");
				Log.Assert(!clipList.ContainsKey(c.name), "Duplicated audio:<color=cyan>"+c.name+"</color>");
				clipList.Add(c.name, c);
			}
			Log.Debug("Loaded <color=yellow>"+resourcePath+"</color> audio files:<color=yellow>"+clips.Length+"</color>");

		}//LoadResourcesFromFolder()

		/// <summary>
		/// Load resources based on localization db using the given section
		/// and resource path.
		/// Only files listed in the section will be loaded,
		/// where the key will be used as id for the audio files. 
		/// </summary>
		public void LoadResources(string section, string resourcePath)
		{

			I18nManager i18n = I18nManager.Instance;
			List<string> keys = i18n.GetKeys(section);			

			foreach(string id in keys)
			{
				string name = i18n.Get(id);
				string fullname = resourcePath + "/" + name;

				AudioClip clip = Resources.Load<AudioClip>(fullname);
				Log.Assert(clip, "Missing audio file:<color=yellow>"+fullname+"</color>");
				Log.Debug("load audio:<color=yellow>"+id+"</color> file:<color=yellow>"+name+"</color>");
				//create audiosource, add to game object
				Log.Assert(!clipList.ContainsKey(id), "Duplicated audio:<color=cyan>"+id+"</color>");
				clipList.Add(id, clip);
			}

			// Log.Debug("Loaded <color=yellow>"+section+"</color> audio files:<color=yellow>"+keys.Count+"</color>");

		}//LoadResources()

		/// <summary>
		/// Play a sound effect by id
		/// </summary>
		public void PlaySound(string id, float volume = 1.0f)
		{
			Play(Type.SFX, id, volume);
		}//PlaySound()

		/// <summary>
		/// Play a background music by id
		/// </summary>
		public void PlayMusic(string id, float volume = 1.0f)
		{
			Play(Type.BACKGROUND, id, volume);
		}//PlayMusic()		
		
		/// <summary>
		/// Play a sound with specified name.
		/// Based on Audio parameter it can play background music,
		/// which will be played one at a time and looped,
		/// or sound effects, which played simultaneously and not looped.		
		/// </summary>
		private void Play(Type type, string id, float volume = 1.0f)
		{

			Log.Assert(clipList.ContainsKey(id), "No audio found with id:"+id);

			switch(type)
			{
				case Type.BACKGROUND:					
					audioBackground.Stop();
					audioBackground.clip = clipList[id];
					audioBackground.Play();
					audioBackground.loop = true;
					break;

				case Type.SFX:
					if(!MuteSfx)
						audioSfx.PlayOneShot(clipList[id],  volume);
					break;

			}

			Log.Debug("Play audio:<color=yellow>"+ id + "</color> ("+clipList[id].name+")");
			
		}//Play()

		/// <summary>
		/// Stop all audio
		/// </summary>
		public void Stop()
		{
			audioBackground.Stop();
			audioSfx.Stop();
		}

		/// <summary>
		/// Stop the current background music,
		/// but leave the sounds.
		/// </summary>
		public void StopMusic()
		{
			Stop(Type.BACKGROUND);
		}

		/// <summary>
		/// Stop all sounds, but leave the music.
		/// </summary>
		public void StopSound()
		{
			Stop(Type.SFX);
		}//StopSound()

		/// <summary>
		/// Stop the type of audio
		/// </summary>
		private void Stop(Type type)
		{
			if(type == Type.BACKGROUND)
				audioBackground.Stop();
			else
				audioSfx.Stop();
		}//Stop()


		/// <summary>
		/// Clear all the audio from list
		/// </summary>
		public void Clear()
		{
			clipList.Clear();
			audioObject = null;
			audioBackground = null;
			audioSfx = null;
		}

///--------------------------------------------------------------------
		///TODO remove later..
		public void Test(GameObject gameObject)		
		{

			Log.Debug("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
			//TODO we just get one resource AudioClip here, that is strange. Must be about 34 
			// in the curren TWISTBueBe Resource/Audio folder

			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			string bgId = "Audio/T0009_BGSFX_NaturUmgebung";
			AudioClip ac = Resources.Load(bgId) as AudioClip;
			audioSource.clip = ac; 

			Log.Debug("<color=cyan>AudioClip name "+ac.name+"</color> initialized.");

			audioSource.Play();
		}
///--------------------------------------------------------------------

	}//class AudioManager
}//namespace 
