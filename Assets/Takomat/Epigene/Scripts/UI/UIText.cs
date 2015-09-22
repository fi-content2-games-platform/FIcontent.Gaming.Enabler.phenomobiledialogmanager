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
using System.Text;
using System;

using Epigene;

namespace Epigene.UI
{

	//TODO
	// - add layer order for text rendering

	/// <summary>
	/// This class responsible to create a ui text,
	/// which can be localized and formatted by a format string.
	/// A text can be numeric or normal string. The format string follows the
	/// usage of string.formatter. Use {0} for no modification or {0:F} / {0:C}, etc 
	/// respectively for floating, currency or other formatting type.
	/// If localization is set to true, the localization ID will be
	/// set to the name of the game object.
	///
	/// This class also works in Editor mode, allow the user to
	/// see exaclty the same result as in the game mode,
	/// however localization needs to be initailized somewhere else.
	/// This class will not override or set default localization db, 
	/// language or sections. You have to make sure your code does it
	/// somewhere else.
	/// @see the UITest for further reference on how to use it.
	/// </summary>
	[ExecuteInEditMode]
	public class UIText : MonoBehaviour, UIObject 
	{
		/// <summary>
		/// Word wrap size.
		/// Word wrap is used if this value is greater then 0
		/// </summary>
		public int wrapSize = 0;

		/// <summary>
		/// maximum length to print out
		/// </summary>
		public int maxLength = 0;

		/// <summary>
		/// localized text id
		/// </summary>		
		public string textId;

		public Rect TextArea
		{
			get { 				
					return new Rect(0,0, 
							textMesh.GetComponent<Renderer>().bounds.size.x / gameObject.transform.localScale.x, 
							textMesh.GetComponent<Renderer>().bounds.size.y / gameObject.transform.localScale.y);
					
				}
		}

		/// <summary>
		/// public text values to be set in the editor
		/// </summary>		
		public string Text  
		{
			set
			{
				text = (wrapSize > 0) ? WordWrap(value, wrapSize) : value;

				//update textMesh
				string formattedText = "";
				if(isNumeric)
				{	
					try
					{
						if(text == "") text = "0";
						float f = 0;
						f = float.Parse(text);
						formattedText = string.Format(format, f);
					}
					catch
					{
						//deny formating errors for now
						Log.Debug("wrong numeric format in:"+gameObject.name);
					}
				}
				else
				{
					formattedText = string.Format(format, text);
				}
				if (textMesh)
				{
					textMesh.text = formattedText;
				}
			}
			get 
			{
				//Or should we return the localized, formatted text?
				return text;
			}
		}

		/// <summary>
		/// Font property
		/// </summary>
		public Font Font
		{
			get { return font;}
			set {
				font =  value;
				if(textMesh != null)
				{
					textMesh.font = font;

				}
				if(meshRenderer != null)
				{
					meshRenderer.material = font.material;
				}
			}
		}
		private Font font;

		/// <summary>
		/// size of the fonts
		/// </summary>
		public int FontSize
		{
			set { 
				size = value;

				if(textMesh != null)
				{
					textMesh.fontSize = size;
				}

			}
			get	{return size; }
		}
		private int size = 13;
		
		/// <summary>
		/// size of the line spacing
		/// </summary>
		public float LineSpacing
		{
			set { 
				lineSize = value;

				if(textMesh != null)
				{
					textMesh.lineSpacing = size;
				}
				
			}
			get	{return lineSize; }
		}
		private float lineSize = 1f;
		
		/// <summary>
		/// style of the fonts
		/// </summary>
		public FontStyle FontStyleDef
		{
			get	{return style;}
			set 
			{ 
				style = value;
				if(textMesh != null)
				{
					textMesh.fontStyle = style;
				}
			}
		}
		private FontStyle style = FontStyle.Normal;
		
		/// <summary>
		/// Colors for the fonts
		/// </summary>
		public Color32 FontColor
		{
			set 
			{ 
				color = value;
				if(textMesh != null)
				{
					textMesh.color =  color;
				}
			}
			get { return color;}
		}
		private Color32 color = new Color32(103,105,107, 255);
		
		/// <summary>
		/// Alpha channel of Colors for the fonts
		/// </summary>
		public float FontColorAlpha
		{
			set 
			{ 
				colorAlpha = value;
				if(textMesh != null)
				{
					textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, colorAlpha);
				}
			}
			get { return colorAlpha;}
		}
		private float colorAlpha = 1f;

		/// <summary>
		/// Anchor of text
		/// </summary>
		public TextAnchor Anchor
		{
			get { return anchor;}
			set 
			{
				anchor = value;
				if(textMesh != null)
				{
					textMesh.anchor = anchor;
				}
			}
		}
		private TextAnchor anchor = TextAnchor.MiddleCenter;

		/// <summary>
		/// Alignment of text
		/// </summary>
		public TextAlignment Alignment
		{
			get{ return alignment;}
			set
			{ 
				alignment = value;
				if(textMesh != null)
				{
					textMesh.alignment = alignment;
				}
			}
		}
		private TextAlignment alignment = TextAlignment.Center;

		/// <summary>
		/// local text of the ui without any formatting and localization
		/// </summary>
		public string text;
		/// <summary>
		/// Set to true if need localization
		/// localization will override the mesh text
		/// in every loop
		/// </summary>
		public bool isLocalized = true;
		/// <summary>
		/// set to true if you want to format the value as numeric
		/// </summary>
		public bool isNumeric = false;
		/// <summary>
		/// format of the text, use it with isNumaric
		/// </summary>
		public string format = "{0}";
		/// <summary>
		/// layer order of the text
		/// use the same orders as sprites
		/// </summary>
		public int SortingOrder
		{
			get{ return sortingOrder; }
			set {
				sortingOrder = value;
				if(meshRenderer != null)
				{
					meshRenderer.sortingOrder = sortingOrder;
				}
			}
		}
		//FIXME: this should be private, but prefabs lost this value if not public
		public int sortingOrder = 1;

		/// <summary>
		/// objects to render the text
		/// </summary>
		public TextMesh textMesh;
		/// <summary>
		/// the renderer for the text
		/// </summary>
		public MeshRenderer meshRenderer;

		private string defaultFont = "Epigene/Fonts/UniversLTStd";
		private Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);

		/// <summary>
		/// Get the type of the this item.
		/// </summary>
		public UIType GetType()
		{
			return UIType.Text;
		}//GetType()

		private static string newLine = "\n"; // Unix definition ! length is 1 for all platforms
		
		/// <summary>
		/// Set up the UIText with using TextMesh
		/// </summary>
		void Awake()
		{

			//Log.Debug("AWAKE:"+gameObject.name);
			
			if(font == null)
			{	
				//only load default if not yet assigned
				font = Resources.Load<Font>(defaultFont);
			}
			Log.Assert(font, "Missing font! Please assign a valid font!", gameObject);

			//check if we already has one, since it could run from Editor
			textMesh = gameObject.GetComponent<TextMesh>();
			if(textMesh == null)
			{
				//first time init, let's make them
				textMesh = gameObject.AddComponent<TextMesh>();			
				Log.Assert(textMesh, "Cannot add TextMesh!", gameObject);

				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				Log.Assert(meshRenderer, "Cannot find meshRenderer!", gameObject);
				meshRenderer.material = font.material;
				meshRenderer.castShadows  = false;
				meshRenderer.receiveShadows = false;

				int order = meshRenderer.sortingOrder;
				if (order != 0)
				{
					sortingOrder = meshRenderer.sortingOrder;
				}
				gameObject.transform.localScale = scale;
	
				//set up defaults
				textMesh.font = font;
				textMesh.anchor = anchor;
				textMesh.alignment = alignment;
				textMesh.fontSize = size;
				textMesh.lineSpacing = lineSize;
				textMesh.fontStyle = style;
				textMesh.color = color;
				textMesh.text = text;
			}


		}//Awake()

		/// <summary>
		/// Start will update the localization if required.
		/// </summary>
		public virtual void OnEnable()
		{
			//text = (wrapSize > 0) ? WordWrap(text, wrapSize) : text;

			//update text if localized
			if(isLocalized)
			{

				//Log.Info("upate localized");
				I18nManager i18n = I18nManager.Instance;
				if(textId == null || textId.Length == 0)
		 			textId = gameObject.name;

				if(Application.isEditor)
			 	{
			 		//update only if its valid key
			 		//so it wont throws error in editor
			 		if(i18n.Contains(textId))
			 			text = i18n.Get(textId);
			 		else
			 			Log.Debug("Not a valid textId:"+textId);
		  		}
		  		else
		  		{	
		  			//in normal mode we always want to use the id
		  			//it will show error if not valid
		  			text = i18n.Get(textId);	
		  		}

		  		Text = text;
			}

			meshRenderer = gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer)	
			{
				meshRenderer.sortingOrder = sortingOrder;			
			}

		}//OnEnable()


		/// <summary>
		/// This will update the text based on formatting and 
		/// localization settings.
		/// The localization will be updated only in Editor Mode
		/// to avoid unnecessary performance overhead.
		/// </summary>
		public virtual void Update () 
		{			

			//modify properties only in editor mode
			if(Application.isEditor)
			{

				//textId = gameObject.name;
			 	//if text localized, reload the text 
			 	//using the name of the object
			 	I18nManager i18n = I18nManager.Instance;
				if(isLocalized 
					&&  i18n != null && i18n.Contains(textId))
				{
					//Log.Debug("Valid key");
					
					text = (wrapSize > 0 ) 	? WordWrap(i18n.Get(textId), wrapSize) 
											: i18n.Get(textId);
					//text = i18n.Get(textId);
				}
				
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer)
				{
					meshRenderer.sortingOrder = sortingOrder;
					//Debug.Log("Set order: " + sortingOrder);
				}
			}

			//always update
			string formattedText = "";
			if(isNumeric)
			{	
				try
				{
					if(text == "") text = "0";
					float f = 0;
					f = float.Parse(text);
					formattedText = string.Format(format, f);
				}
				catch
				{
					//deny formating errors for now
					Log.Debug("wrong numeric format in:"+gameObject.name);
				}
			}
			else
			{
				formattedText = string.Format(format, text);
			}

			//limit the length if specifed
			if((maxLength > 0) && (maxLength <= formattedText.Length))
			{
				formattedText = formattedText.Substring(0, maxLength);
				formattedText += "...";
			}
			
			textMesh.text = formattedText;
		
		}//update()

//----------------------------------------------------------------------  3P ----------

		/// The following methods are modified 3p code
		/// 3p : search for "automatic word wrap algorithm C#"
		/// This 3p code is licensed under the CPOL
		/// http://www.codeproject.com/info/cpol10.aspx
		/// 
		/// So it is free of use and modification, as we did, are allowed.


		/// <summary>
		/// Word wraps the given text to fit within the specified width.
		/// </summary>
		/// <param name="text">Text to be word wrapped</param>
		/// <param name="width">Width, in characters, to which the text
		/// should be word wrapped</param>
		/// <returns>The modified text</returns>
		public static string WordWrap(string text, int width)
		{
			int pos, next;
			StringBuilder sb = new StringBuilder();
			
			// Lucidity check
			if (width < 1)
				return text;

			//Log.GameTimes ("Newline length : " + newLine.Length);
			// Parse each line of text
			for (pos = 0; pos < text.Length; pos = next)
			{
				// Find end of line
				//int eol = text.IndexOf(Environment.NewLine, pos);
				int eol = text.IndexOf(newLine, pos);
				if (eol == -1)
					next = eol = text.Length;
				else
				{
					//next = eol + Environment.NewLine.Length;
					// newLine.Length is fix set to 1, because on Windows it is 2 and on Mac 1
					next = eol + 1; //newLine.Length;
				} 

				
				// Copy this line of text, breaking into smaller lines as needed
				if (eol > pos)
				{
					do
					{
						int len = eol - pos;
						if (len > width)
							len = BreakLine(text, pos, width);
						sb.Append(text, pos, len);
						sb.Append(newLine);
						
						// Trim whitespace following break
						pos += len;
						while (pos < eol && Char.IsWhiteSpace(text[pos]))
							pos++;
					} while (eol > pos);
				}
				else 
				{
					sb.Append(newLine); // Empty line
					//// Log.GameTimes("sb String empty Line");
				} 
					
			}
			//// Log.GameTimes("sb String "+ sb.ToString());
			return sb.ToString();
		}//WordWrap

		/// <summary>
		/// Locates position to break the given line so as to avoid
		/// breaking words.
		/// </summary>
		/// <param name="text">String that contains line of text</param>
		/// <param name="pos">Index where line of text starts</param>
		/// <param name="max">Maximum line length</param>
		/// <returns>The modified line length</returns>
		private static int BreakLine(string text, int pos, int max)
		{
			// Find last whitespace in line
			int i = max;
			while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
				i--;
			
			// If no whitespace found, break at maximum length
			if (i < 0)
				return max;
			
			// Find start of whitespace
			while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
				i--;
			
			// Return length of text before whitespace
			return i + 1;

		}//BrakeLine

//---------------------------------------------------------------------- End 3P ----------
		/// <summary>
		/// Parse the content of the dictionary
		/// and create a new gameobjet with this parameters
		/// </summary>
		public virtual void Parse(Dictionary<string,object> dict)
		{

			//base.Parse(dict);

			//GameObject gameObject = new GameObject();
			//this.gameObject = gameObject;
			//and set the id
			//this.gameObject.name = dict["id"].ToString();

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

				string rot = dict["rotation"].ToString();
				float rx = System.Convert.ToSingle(rot.Split(',')[0]);
				float ry = System.Convert.ToSingle(rot.Split(',')[1]);

				//Log.Debug("Rotation:"+rot+":"+rx+","+ry);
				this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(rx, ry, 0));
				//this.gameObject.name = dict["position"].ToString();
			}
			if(dict.ContainsKey("scale"))
			{

				string sc = dict["scale"].ToString();
				float sx = System.Convert.ToSingle(sc.Split(',')[0]);
				float sy = System.Convert.ToSingle(sc.Split(',')[1]);

				this.gameObject.transform.localScale = new Vector3(sx, sy, 0);
			}
			if(dict.ContainsKey("order"))
			{
				SortingOrder = System.Convert.ToInt32(dict["order"].ToString());				
			}
			if(dict.ContainsKey("layer"))
			{
				string layer = dict["layer"].ToString();
				this.gameObject.layer = LayerMask.NameToLayer(layer);
			}
			if(dict.ContainsKey("fontSize"))
			{
				FontSize = Convert.ToInt32(dict["fontSize"].ToString());
			}
			if(dict.ContainsKey("spacing"))
			{
				string value = dict["spacing"].ToString();
				LineSpacing = float.Parse(value);
			}
			if(dict.ContainsKey("fontStyle"))
			{
				//TODO implement possibility to disply UIText in Bold or Italic
				string fontStyleDef = dict["fontStyle"].ToString();
				if(fontStyleDef == "bold")
					FontStyleDef = FontStyle.Bold;
				else if(fontStyleDef == "italic")
					FontStyleDef = FontStyle.Italic;
				else if(fontStyleDef == "bold and italic")
					FontStyleDef = FontStyle.BoldAndItalic;
			}
			if(dict.ContainsKey("color"))
			{
				string color = dict["color"].ToString();
				byte red = System.Convert.ToByte(color.Split(',')[0]);
				byte green = System.Convert.ToByte(color.Split(',')[1]);
				byte blue = System.Convert.ToByte(color.Split(',')[2]);
				byte alpha = System.Convert.ToByte(color.Split(',')[3]);
				
				this.color = new Color32(red, green, blue, alpha);
			}
			if(dict.ContainsKey("localized"))
			{
				string value = dict["localized"].ToString();
				isLocalized = (value == "true" ) ? true : false;
				textId = dict["id"].ToString();
			}
			if(dict.ContainsKey("font"))
			{
				font = Resources.Load<Font>(dict["font"].ToString());
				Log.Debug("font:"+font);
			}
			if(dict.ContainsKey("text"))
			{
				Text = dict["text"].ToString();
			}

			if(dict.ContainsKey("anchor"))
			{
				string value = dict["anchor"].ToString();
				if(value == "middle center")
					Anchor = TextAnchor.MiddleCenter;
				else if(value == "middle left")
					Anchor = TextAnchor.MiddleLeft;
				else if(value == "middle right")
					Anchor = TextAnchor.MiddleRight;
				else if(value == "upper left")
					Anchor = TextAnchor.UpperLeft;
				else if(value == "upper center")
					Anchor = TextAnchor.UpperCenter;
				else if(value == "upper right")
					Anchor = TextAnchor.UpperRight;
				else if(value == "lower left")
					Anchor = TextAnchor.UpperLeft;
				else if(value == "lower center")
					Anchor = TextAnchor.UpperCenter;
				else if(value == "lower right")
					Anchor = TextAnchor.UpperRight;
				
			}
			if(dict.ContainsKey("alignment"))
			{
				string value = dict["alignment"].ToString();
				if(value == "left")
					Alignment = TextAlignment.Left;
				else if(value == "center")
					Alignment = TextAlignment.Center;
				else if(value == "right")
					Alignment = TextAlignment.Right;
			}

			if(dict.ContainsKey("wrapsize"))
			{
				string value = dict["wrapsize"].ToString();
				this.wrapSize = Convert.ToInt32(value);
			}

			if(dict.ContainsKey("maxlength"))
			{
				string value = dict["maxlength"].ToString();
				this.maxLength = Convert.ToInt32(value);
			}
			if(dict.ContainsKey("format"))
			{
				string value = dict["format"].ToString();
				this.format = value;
			}
			if(dict.ContainsKey("numeric"))
			{
				string value = dict["numeric"].ToString();
				this.isNumeric = (value == "true" ) ? true : false;
			}




		}//Parse()
	
	}//class UIText

}//namespace Epigene.UI 

#endif //!EPIGENE_UI_46
