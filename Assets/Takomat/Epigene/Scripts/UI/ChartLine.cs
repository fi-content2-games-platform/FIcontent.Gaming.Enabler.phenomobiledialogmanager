using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


using Epigene.UI;

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

namespace Epigene
{
	public class ChartLine : MonoBehaviour
	{
		public int sortingOrder = 0;
		public ScrollArea scrollArea;
		protected UIType type = UIType.ChartLine;

		/// <summary>
		/// Get the uiChartLine object
		/// </summary>
		public UIChartLine UIChartLine
		{
			get { return uiChartLine; }
		}
		private UIChartLine uiChartLine;

		/// <summary>
		/// Name of the chart
		/// </summary>
		public string Name
		{
			get{ return gameObject.name; }
			set{ gameObject.name = value; }
		}

		void Awake()
		{
			if (Application.isPlaying)
			{
				// Debug.Log(gameObject.name+ " <color=red>Init</color>");
				// Debug.Log("transform:" + gameObject.transform);

				uiChartLine = (UIChartLine)UIManager.Instance.Add(type, gameObject, "");
				uiChartLine.SortingOrder = sortingOrder;
				uiChartLine.scrollArea = scrollArea;
			}
		}
	}//class Chart

	#if UNITY_EDITOR


	[CustomEditor(typeof(ChartLine))]
	public class ChartLineEditor : Editor
	{
	    void OnSceneGUI()
	    {
	        ChartLine myTarget = (ChartLine)target;
	 
	        Vector3 pos = myTarget.transform.position;
	        Vector3 scale = myTarget.transform.localScale;

			Vector3[] verts  = new Vector3[] { 
							new Vector3(pos.x - scale.x, pos.y - scale.y, 0), 
							new Vector3(pos.x - scale.x, pos.y + scale.y, 0), 
							new Vector3(pos.x + scale.x, pos.y + scale.y, 0), 
							new Vector3(pos.x + scale.x, pos.y - scale.y, 0)
			};
			Handles.DrawSolidRectangleWithOutline(verts, new Color(1,1,1,0.2f), Color.white);

			string label;
			label = "LINE chart";

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 20;
			Handles.Label(myTarget.transform.position, label, style);
		}
	}// class ChartEditor
	#endif

}//namespace

