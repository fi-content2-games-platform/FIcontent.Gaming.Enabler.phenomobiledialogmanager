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
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace Epigene.Network
{

    /// <summary>
    /// Send HTTP data to network
    /// </summary>
    public class HTTP
    {
    	public int defaultTimeout = 2000; //10000;
    	public int sleepTime = 30;

    	private string serverUrl;
		private Thread receiverThread;

    	public HTTP(string url)
    	{
    		serverUrl = url;
    	}

		public byte[] Send(byte[] buffer, int size = 0)
		{
			if (size == 0)
				size = buffer.Length;

            WebRequest request = WebRequest.Create(serverUrl);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();

            // Display the status.
            Debug.Log(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Debug.Log(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();

		    byte[] bytes = new byte[responseFromServer.Length * sizeof(char)];
		    System.Buffer.BlockCopy(responseFromServer.ToCharArray(), 0, bytes, 0, bytes.Length);
		    return bytes;
		}
	
		public bool Receive(byte[] buffer, int size = 0, int timeout = -1, int offset = 0)
		{
			return false;
			// if (socket == null)
			// 	return false;

			// if (size == 0)
			// 	size = buffer.Length;
			// if (timeout == -1)
			// 	timeout = defaultTimeout;
	
	
			// int startTickCount = Environment.TickCount;
			// int received = 0;  // how many bytes is already received
			// do
			// {
			// 	if (Environment.TickCount > startTickCount + timeout)
			// 	{
			// 		// Debug.Log("Receive timeout");
			// 		// throw new Exception("Timeout.");
			// 		break;
			// 	}

			// 	try
			// 	{
			// 		int s = socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
			// 		received += s;
			// 		if (s == 0 && received > 0) //TODO check with 64k+ packets, why 10923 bytes??
			// 		{
			// 			Debug.Log("received: " + received);
			// 			break;
			// 		}
			// 	}
			// 	catch (SocketException ex)
			// 	{
			// 		if (ex.SocketErrorCode == SocketError.WouldBlock ||
			// 			ex.SocketErrorCode == SocketError.IOPending ||
			// 			ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
			// 		{
			// 			// socket buffer is probably empty, wait and try again
			// 			Thread.Sleep(sleepTime);
			// 		}
			// 		else
			// 			throw ex;  // any serious error occurr
			// 	}
			// } while (received < size);
			
			// return (received > 0);
		}


		// Socket socket = tcpClient.Client;
		// byte[] buffer = new byte[12];  // length of the text "Hello world!"
		// try
		// { // receive data with timeout 10s
		//   MyClass.Receive(socket, buffer, 0, buffer.Length, 10000);
		//   string str = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
		// }
		// catch (Exception ex) { /* ... */ }




	}//class
}//namespace