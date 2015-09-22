using UnityEngine;
using System.Collections;

using Epigene;
using Epigene.GAME;

namespace Epigene.TEST
{

	/// <summary>
	/// Test manager for event handling
	/// </summary>
	public class TestEventHandler : MonoBehaviour 
	{

		public TestEventObject object1;	//TEST1 event
		public TestEventObject object2;	//TEST1 event
		public TestEventObject object3;	//TEST2 event
		public TestEventObject object4;	//TEST2 event

		private GameManager gm;

		void Awake()
		{
			//initialize
			gm = GameManager.Instance;
			Log.Assert(gm != null, "Can't create GameManager!");

			Log.Assert(object1, "Missing object1");
			Log.Assert(object2, "Missing object2");
			Log.Assert(object3, "Missing object3");
			Log.Assert(object4, "Missing object4");
		}

		//start the test
		void Start () 
		{
			//enable all for testing
			object1.SetActive(true);
			object2.SetActive(true);
			object3.SetActive(true);
			object4.SetActive(true);

			//TODO check gm event list count

			//send "TEST1" event when all active
			gm.Event("TEST1", "1/1");
			Log.Assert(object1.eventCounter == 1, "FAIL: 1/1 object1");
			Log.Assert(object2.eventCounter == 1, "FAIL: 1/1 object2");
			Log.Assert(object3.eventCounter == 0, "FAIL: 1/1 object3");
			Log.Assert(object4.eventCounter == 0, "FAIL: 1/1 object4");
			Log.Info("TEST 1/1 OK");

			//deactivate obj2 and send event again
			object2.SetActive(false);
			gm.Event("TEST1", "1/2");
			Log.Assert(object1.eventCounter == 2, "FAIL: 1/2 object1");
			Log.Assert(object2.eventCounter == 1, "FAIL: 1/2 object2");
			Log.Assert(object3.eventCounter == 0, "FAIL: 1/2 object3");
			Log.Assert(object4.eventCounter == 0, "FAIL: 1/2 object4");
			Log.Info("TEST 1/2 OK");

			//check TEST2
			gm.Event("TEST2", "2/1");
			Log.Assert(object1.eventCounter == 2, "FAIL: 2/1 object1");
			Log.Assert(object2.eventCounter == 1, "FAIL: 2/1 object2");
			Log.Assert(object3.eventCounter == 1, "FAIL: 2/1 object3");
			Log.Assert(object4.eventCounter == 1, "FAIL: 2/1 object4");
			Log.Info("TEST 2/1 OK");
			
			object3.SetActive(false);
			gm.Event("TEST2", "2/2");
			Log.Assert(object1.eventCounter == 2, "FAIL: 2/2 object1");
			Log.Assert(object2.eventCounter == 1, "FAIL: 2/2 object2");
			Log.Assert(object3.eventCounter == 1, "FAIL: 2/2 object3");
			Log.Assert(object4.eventCounter == 2, "FAIL: 2/2 object4");
			Log.Info("TEST 2/2 OK");

			Log.Info("ALL TEST OK");
			
		}


	}//class
}//namespace