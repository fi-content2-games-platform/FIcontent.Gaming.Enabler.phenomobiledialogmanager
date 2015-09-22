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

using Epigene.UI;

public class SortingOrder46 : MonoBehaviour
{
	private struct Item
	{
		public UIBaseObject obj;
		public int sortingOrder;
	}

	private List<Item> uiItems = new List<Item>();

	public void AddUIObject(UIBaseObject obj, int order = -1)
	{
		Item item = new Item();
		item.obj = obj;
		item.sortingOrder = (order == -1) ? uiItems.Count : order;

		uiItems.Add(item);
		UpdateOrder(uiItems.Count - 1, item.sortingOrder);
	}

	public void SetSoringOrder(UIBaseObject obj, int order)
	{
		// Debug.Log(string.Format("<color=yellow>SetSoringOrder {0}->{1}</color>", obj, order));

		// for (int i = 0; i < uiItems.Count; i++)
		// {
		// 	Debug.Log(string.Format("<color=red>{0}->{1} ({2})</color>", uiItems[i].obj.GameObject.name, uiItems[i].sortingOrder, i));
		// }

		//search item
		for (int i = 0; i < uiItems.Count; i++)
		// foreach (Item item in uiItems)
		{
			if (uiItems[i].obj == obj)
			{
				UpdateOrder(i, order);
				return;
			}
		}

		AddUIObject(obj, order);
		//search postion by order, move in list, update order Item.Move(oldIndex, newindex)
	}

	private void UpdateOrder(int oldIndex, int newOrder)
	{
		int oldOrder = uiItems[oldIndex].sortingOrder;
		// Debug.Log(string.Format("<color=green>UpdateOrder index: {0}  oldOrder: {1} newOrder: {2}</color>", index, oldOrder, newOrder));

		// string s = "****** \n Before:\n";
		// for (int i = 0; i < uiItems.Count; i++)
		// {
		// 	s += string.Format("{0} {1}\n", uiItems[i].obj.GameObject.name, uiItems[i].sortingOrder);
		// }

		int newIndex = -1;
		for (int i = 0; i < uiItems.Count; i++)
		{
			if (uiItems[i].sortingOrder > newOrder)
			{
				newIndex = i;
				break;
			}
		}

		if (newIndex != -1 && oldIndex != newIndex)
		{
			Item item = uiItems[oldIndex];
			item.obj.GameObject.transform.SetSiblingIndex(newIndex);
			uiItems.RemoveAt(oldIndex);
			uiItems.Insert(newIndex, item);
		}

		// s += "After:\n";
		// for (int i = 0; i < uiItems.Count; i++)
		// {
		// 	s += string.Format("{0} {1}\n", uiItems[i].obj.GameObject.name, uiItems[i].sortingOrder);
		// }
		// Debug.Log(s);
	}
}

