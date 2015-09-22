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

namespace Epigene
{
	/// Software design issues:
	/// - This DeviceManager is designed to manage any kind of Device attached 
	///   to the game, e.g. gameserver, Oculus Rift
	/// - Why using this API, because it is good to have devices run in 
	///    an own thread. So that the main unity render thread has 
	///    not to bother about incoming data.
	/// - SHPFile reader might be also a device in the future
	/// 
	/// 
	/// General information on design template from MotionBuilder:
	/// 
	/// MotionBuilder runs two different threads
	/// 1.)  Real-time engine thread: tells your plugin 
	/// to query the hardware for data. 
	/// It calls FBDevice::DeviceTransportNotify(), 
	/// FBDevice::DeviceIONotify(), 
	/// and FBDevice::DeviceEvaluationNotify(). 
	/// Note that any functions called by the real-time engine, 
	/// must be optimized and cannot block (i.e. wait on a resource or another thread).
	/// COMPARABLE here to a device
	///	2.) Animation thread: tells your plugin to deliver animation data.
	/// This calls FBDevice::AnimationNodeNotify().
	/// COMPARABLE here to teh unity3d engine execution, render ... work

	//TODO : This Manager or one device shouldbe handled as a thread to allow
	//       parallel processing like AUtodesk Tools MoBu, Maya do.

	// Devices we need here : 
	// 1. http to send and receive data from the game server
	// 

	// Design code snippet for transfering scores and image:
	// using System;
/*
 *  google search : unity3D base64 encoding
 *  http://answers.unity3d.com/questions/40568/base64-encodedecoding.html

    using System.Text;
	Then you encode with (replace byte[] and string with var to make it js compatible):
		
		byte[] bytesToEncode = Encoding.UTF8.GetBytes (inputText);
	string encodedText = Convert.ToBase64String (bytesToEncode);
	And decode with (change byte[] and string to var again for js):
		
		byte[] decodedBytes = Convert.FromBase64String (encodedText);
	string decodedText = Encoding.UTF8.GetString (decodedBytes);
*/


//------------------------------------------------------------------------------
	/// <summary>
	/// Sound manager class to manage sound sfx
	/// </summary>
	public sealed class DeviceManager
	{

		private static readonly DeviceManager instance = new DeviceManager();
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static DeviceManager  Instance
		{
			get{ return instance;}
		}

		// Use this for initialization
		//void Start () 
		private DeviceManager()
		{
#if DEBUG
			Debug.Log("<color=cyan>DeviceManager</color> initialized.");
#endif				
			//TODO : configuration from 
			// Read in all necessary AudioClips fromthe Resource 
			// folder

		}//DeviceManager()

		/// <summary>
		/// The bgAudioIds mapped to Resource string to load from
		/// </summary>
		private Dictionary<string, string> bgResourceIdList;
		/// <summary>
		/// The bgAudioIds mapped to AudioClip
		/// </summary>
		public  Dictionary<string, AudioClip> bgClipList;	

		private Dictionary<string, string>   resourceIdList;
		/// <summary>
		/// Other Audio sound than bg
		/// </summary>
		private Dictionary<string, AudioClip> cliplist;	

		/// <summary>
		/// Releases unmanaged resources and performs 
		/// other cleanup operations 
		/// before the <see cref="DeviceManager"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~DeviceManager()
		{
		}
		
//------------------------------------------------------------------------------

		/*
		bool  Done();					//!< Device removal.
		bool  Reset();					//!< Reset function.
		bool  Stop();					//!< Device online routine.
		bool  Start();					//!< Device offline routine.


		public void Start() 
		{
		}

		public void Stop(GameObject gameObject) 
		{
		}
		*/

	}//class DeviceManager
}//namespace DeviceManager
