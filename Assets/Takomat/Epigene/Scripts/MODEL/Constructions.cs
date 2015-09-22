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
using System;
using Epigene;
using Epigene.UI;
//------------------------------------------------------------------------------
namespace Epigene.MODEL
{
	public abstract class Constructions : Resource
	{
		public class Data
		{
			static I18nManager i18n;
	
			public double value;
			public string label;
			public string format;
			public string icon;

			public string text
			{
				get { return string.Format(format, value); }
			}

			static Data()
			{
				i18n = I18nManager.Instance;
			}

			public Data(string name)
			{
			 	label = i18n.Get("Constructions", name + "Label");
			 	format = i18n.Get("Constructions", name + "Format");
			 	icon = i18n.Get("Constructions", name + "Icon");
			}
		}

		public DateTime GroundBreakingDate
		{
			get {return groundBreakingDate;}
			set {groundBreakingDate = value;}
		}		
		private DateTime groundBreakingDate	= new DateTime(1970,1,1);
		
		public string Name
		{
			get {return name;}
			set {name = value; }
		}		
		private string name	= "";
		
		public bool isOutOfOrder   = false;
		public bool isDismanteled  = false;
		public bool underConstruction = false;

		public Data Price { get {return price;} }
		public Data PaymentByInstallments { get { return paymentByInstallments; } }
		public Data Amortization { get { return amortization; } }
		public Data DemolationCost { get { return demolationCost; } }
		public Data FixedCost { get { return fixedCost; } }
		public Data RestDebt  { get { return restDebt; } set {RestDebt = value; } }
		public Data RemediationCost { get { return remediationCost; } }
		public Data Co2emission { get { return co2emission; } }
		public Data NuclearWaste { get { return nuclearWaste; } }
		public Data GrossPower { get { return grossPower; } }
		public Data Environmental { get { return environmental; } }
		public Data WorkplaceEffect { get { return workplaceEffect; } }
		public Data Technology { get { return technology; } }
		public Data FBXPath { get { return fbxPath; } }
	
		protected Data price = new Data("price");
		protected Data paymentByInstallments = new Data("paymentByInstallments");
		protected Data amortization = new Data("amortization");
		protected Data demolationCost = new Data("demolationCost");
		protected Data fixedCost = new Data("fixedCost");
		protected Data restDebt = new Data("restDebt");
		protected Data remediationCost = new Data("remediationCost");
		protected Data co2emission = new Data("co2emission");
		protected Data nuclearWaste = new Data("nuclearWaste");
		protected Data grossPower = new Data("grossPower");
		protected Data environmental = new Data("environmental");
		protected Data workplaceEffect = new Data("workplaceEffect");
		protected Data technology = new Data("technology");
		protected Data fbxPath = new Data("fbxPath");


		// GIS attributes
		public double AttributeC3
		{
			get {return attributeC3;}
			set {attributeC3 = value;}
		}
		protected double attributeC3 = 0.0;

		//??		
		public double buildingTime;  // unit in months						
		// public string buildingTimeUnit         = "a"; // a=annum
		// public double acceptance				  = 1;  // 1-4 view in stars for technic acceptance						


	}//class Constructions

}//namespace
//------------------------------------------------------------------------------
