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

using Epigene.UI;
using Epigene.AUDIO;

namespace Epigene
{

	//TODO: 
	// - add onHover and onRelease
	// - resource name should be automatic from sprite

	[ExecuteInEditMode]
	public class Image : MonoBehaviour 
	{
		public string resourcePath;

		public UIImage image;

		/// <summary>
		/// Get the uiButton object
		/// </summary>
		public UIImage UIImage
		{
			get
			{
				if(image == null)
					Init();

				return image;
			}
		}
		//private UIImage image;

		/// <summary>
		/// Name of the button
		/// </summary>
		public string Name
		{
			get{ return gameObject.name; }
			set{ gameObject.name = value; }
		}

		// Use this for initialization
		void Awake()
		{
			//Log.Debug("Image Start in "+gameObject.name);
			
			UIManager uiManager = UIManager.Instance;
			image = (UIImage)uiManager.Add(UIType.Image, gameObject, resourcePath);

			//Log.Debug("image count="+image.Count);
	
		}//Start()

		/// <summary>
		/// Initializa the uiImage
		/// </summary>
		void Init()
		{
			image = (UIImage)UIManager.Instance.Add(
				UIType.Image, gameObject, resourcePath);
		}//Init()


	}//class Image

}//namespace