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

using UnityEngine;
using System.Collections;

using Epigene.UI;

namespace Epigene.UI
{
	public class PieExample : MonoBehaviour
	{
		private UIChartPie chart;
		private int time = 0;

		void Awake()
		{
			Debug.Log("PieExample Awake");
			ChartPie c = gameObject.GetComponent<ChartPie>();
			chart = c.UIChartPie;
			c.sortingOrder = 103;

			// set default values
			chart.AddValues(new float[] { 110, 100, 10, 200, 200, 700, 70 } );
			chart.SetColor(0, Color.white, Color.black);
			chart.SetColor(1, Color.red, Color.yellow);
			chart.SetColor(2, new Color32(0, 40, 0, 255), new Color32(0, 255, 0, 255));
			chart.Create();

			time = System.Environment.TickCount;
		}

		void Update()
		{
			if (time + 1000 < System.Environment.TickCount)
			{
				// Debug.Log("update");
				chart.RemoveValues();
				time = System.Environment.TickCount;
				for (int i = 0; i < 7; i++)
				{
					float rnd = Random.Range(50.0f, 400.0f);
					chart.AddValue(rnd);
				}
				chart.Create(); //update chart
			}
		}
	}
}
