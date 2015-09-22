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

namespace Epigene
{
	/// <summary>
	/// resource manager
	/// </summary>
	public sealed class ResourceManager
	{
		private static readonly ResourceManager instance = new ResourceManager();
		             
		public double glbOriginX = 4455331.424; // current values from Wohlsborn
		public double glbOriginY = 5655189.415; // current values from Wohlsborn

		public Dictionary<string, List<Resource>> resources;	
		public Dictionary<string, List<ResourceView>> resourceViews;	
		// public  List<Resource>     resources;
		// public  List<ResourceView> resourcesView;	
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ResourceManager  Instance
		{
			get{ return instance;}
		}

		// Use this for initialization
		//void Start () 
		private ResourceManager()
		{
			Log.Debug("<color=cyan>ResourceManager</color> initialized.");
			// resources = new List<Resource>();
			// resourcesView = new List<ResourceView>();
			resources = new Dictionary<string, List<Resource>>();
			resourceViews = new Dictionary<string, List<ResourceView>>();
		}//GameManager()


		/// <summary>
		/// Releases unmanaged resources and performs 
		/// other cleanup operations 
		/// before the <see cref="GameManager"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~ResourceManager()
		{
		}

	}//class ResourceManager
}//namespace ResourceManager
