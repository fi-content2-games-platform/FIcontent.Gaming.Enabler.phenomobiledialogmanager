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
	public class LineExample : MonoBehaviour
	{
		private UIChartLine chart;
		private int time = 0;

		void Awake()
		{
			ChartLine c = gameObject.GetComponent<ChartLine>();
			// c.sortingOrder = 103;
			
			chart = c.UIChartLine;
			chart.chartType = UIChartLine.ChartType.LINE;

			chart.Title = "Total Water Loss"; //TODO: localization
			chart.UnitX = "Year"; //TODO: localization
			chart.UnitY = "L"; //TODO: localization

			chart.Width = 4;
			chart.Height = 2;
			chart.Thickness = 0.015f;
			chart.PointSize = 0.03f;

			chart.SetSize(1, 5);
			for (int j = 0; j < 5; j++)
			{
				float rnd = Random.Range(50.0f, 400.0f);
				chart.SetValue(0, j, rnd);
			}
			for (int i = 0; i < 5; i++)
			{
				chart.SetLabelX(i, string.Format("{0}", 2010 + i));
			}
			chart.SetColor(0, Color.blue);
			chart.Create();

			time = System.Environment.TickCount;
		}

		void Update()
		{
			if (time + 120 * 1000 < System.Environment.TickCount)
			{
				chart.RemoveValues();

				for (int j = 0; j < 5; j++)
				{
					float rnd = Random.Range(50.0f, 400.0f);
					chart.SetValue(0, j, rnd);
				}
				chart.Create(); //update chart
				
				time = System.Environment.TickCount;
			}
		}
	}
}
