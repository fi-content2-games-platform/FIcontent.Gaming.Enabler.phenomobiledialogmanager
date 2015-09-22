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

namespace Epigene.UI
{

	/// <summary>
	/// Inteface for all ui objects.
	/// This way we can gourp them into one list
	/// TODO: move interface functions here
	/// </summary>
	public interface UIObject 
	{
		UIType	GetType();

	}//class UIObject

	/// <summary>
	/// Abstract base class which contains
	/// generic functions and properties.
	/// </summary>
	public abstract class UIBaseObject : UIObject
	{

		/// <summary>
		///  Parent GameObject which this button belongs
		/// </summary>
		public GameObject GameObject
		{
			get { return gameObject;}
			set { gameObject = value; }

		}
		protected GameObject gameObject;

		/// <summary>
		/// reference name of the button
		/// </summary>
		public string Name
		{
			get {return gameObject.name;}
			set {gameObject.name = value;}
		}

		/// <summary>
		/// type of UI element
		/// </summary>		
		public UIType Type
		{
			get { return type;}
		}		
		protected UIType type;

		/// <summary>
		/// flag for visibility
		/// </summary>
		public bool Visible
		{
			get { return visible; }
			set 
			{ 
				visible = value;
				if(gameObject.GetComponent<Renderer>())
					gameObject.GetComponent<Renderer>().enabled = visible; 
				Collider2D c = gameObject.GetComponent<Collider2D>();
				if(c != null)
					c.enabled = visible;
			}
		}
		protected bool visible;

		/// <summary>
		/// Get/Set layer order
		/// </summary>
		public int SortingOrder
		{
			get 
			{
				return (gameObject.GetComponent<Renderer>() != null) ? gameObject.GetComponent<Renderer>().sortingOrder : 0;
			}
			set 
			{
				if(gameObject.GetComponent<Renderer>() != null)
				{
					gameObject.GetComponent<Renderer>().sortingOrder = value;
				}

			}
		}

		/// <summary>
		/// Get the type of the this item.
		/// </summary>
		public virtual UIType GetType()
		{
			return type;
		}//GetType()

		/// <summary>
		/// Parse the content of the dictionary
		/// and create a new gameobjet with this parameters
		/// </summary>
		public virtual void Parse(Dictionary<string,object> dict)
		{

			//and set the id
			if (dict.ContainsKey ("id")) {
				this.gameObject.name = dict ["id"].ToString ();
			} else {
				Log.Error("No ID defined in this dictionary");
				return;
			}

			if(dict.ContainsKey("position"))
			{

				string pos = dict["position"].ToString();
				float px = System.Convert.ToSingle(pos.Split(',')[0]);
				float py = System.Convert.ToSingle(pos.Split(',')[1]);

				//Log.Debug("Position:"+px+","+py);
				this.gameObject.transform.position = new Vector3(px, py, 0);
				//this.gameObject.name = dict["position"].ToString();
			}
			if(dict.ContainsKey("rotation"))
			{

				string[] rot = dict["rotation"].ToString().Split(',');
				float rx = System.Convert.ToSingle(rot[0]);
				float ry = System.Convert.ToSingle(rot[1]);
				float rz = rot.Length > 2 ? System.Convert.ToSingle(rot[2]) : 0.0f;

				//Log.Debug("Rotation:"+rot+":"+rx+","+ry);
				this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(rx, ry, rz));
				//this.gameObject.name = dict["position"].ToString();
			}
			if(dict.ContainsKey("scale"))
			{

				string sc = dict["scale"].ToString();
				float sx = System.Convert.ToSingle(sc.Split(',')[0]);
				float sy = System.Convert.ToSingle(sc.Split(',')[1]);

				this.gameObject.transform.localScale = new Vector3(sx, sy, 0);
				//this.gameObject.name = dict["position"].ToString();
			}
			if(dict.ContainsKey("order"))
			{
				int order = System.Convert.ToInt32(dict["order"].ToString());
				SortingOrder = order;

			}
			if(dict.ContainsKey("layer"))
			{
				string layer = dict["layer"].ToString();
				this.gameObject.layer = LayerMask.NameToLayer(layer);
			}

			if(dict.ContainsKey("collider"))
			{
				// Log.Debug("Add collider");
				string colliderName = dict["collider"].ToString();
				if(colliderName == "circle")
					gameObject.AddComponent<CircleCollider2D>();// as Collider;
				else if(colliderName == "box")
					gameObject.AddComponent<BoxCollider2D>();
				else if(colliderName == "polygon")
					gameObject.AddComponent<PolygonCollider2D>();
				else
					Log.Error("Invalid collider:"+ colliderName +" in:"+Name);
			}

			if(dict.ContainsKey("tooltip"))
			{
				Log.Debug("Add tooltip");

				Tooltip t = gameObject.AddComponent<Tooltip>();
				if(t)
				{
					t.id = dict["tooltip"].ToString();
				}
				
				Collider2D c = gameObject.GetComponent<Collider2D>();
				if(c == null)
				{
					//default is a box collider
					gameObject.AddComponent<BoxCollider2D>();
				}
			}


		}//Parse()

	}//class UIBaseObject

}//namespace

#endif //!EPIGENE_UI_46

