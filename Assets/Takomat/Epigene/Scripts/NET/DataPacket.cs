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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace Epigene.Network
{
   
    /// <summary>
    /// This class represnets a data packet.
    /// The data contains two parts, the header and the data.
    /// The header holds information about the type of message, while the data contains the actual data of the message.
    /// </summary>
    class DataPacket
    {
        public bool useHeader = false;
    	public enum DataFormat { BINARY = 0, JSON, XML };

		/// <summary>
		/// This class represnets a binary data packet header.
		/// id represents (E)pigene (T)ransfer (t)akomat (P)rotocol 
		/// </summary>
        public class Header
        {
            private int id = 0x45547450; //"ETtP"
            
            public short command;
            public int sampleCounter;
            public int datagramCounter;
            public int crc;
            public int timecode;
            public DataFormat dataFormat = DataFormat.JSON;

            public void Deserialize(byte[] data)
            {
                int i = 0;
                
                id = (int)data[i++] << 24;
                id += (int)data[i++] << 16;
                id += (int)data[i++] << 8;
                id += (int)data[i++];
                
                command = (short)((int)data[i++] << 8);
                command += (short)data[i++];

                sampleCounter = (int)data[i++] << 24;
                sampleCounter += (int)data[i++] << 16;
                sampleCounter += (int)data[i++] << 8;
                sampleCounter += (int)data[i++];

                datagramCounter = (int)data[i++] << 24;
                datagramCounter += (int)data[i++] << 16;
                datagramCounter += (int)data[i++] << 8;
                datagramCounter += (int)data[i++];

                crc = (int)data[i++] << 24;
                crc += (int)data[i++] << 16;
                crc += (int)data[i++] << 8;
                crc += (int)data[i++];

                timecode = (int)data[i++] << 24;
                timecode += (int)data[i++] << 16;
                timecode += (int)data[i++] << 8;
                timecode += (int)data[i++];

                dataFormat = (DataFormat)data[i++];
                //we dont care about the next 9 reserved bytes
                
                //TODO check CRC, timecode
                
            }

            public byte[] Serialize()
            {
                byte[] result = new byte[32];
                int i = 0;

                //TODO calculate CRC and timecode here

                result[i++] = (byte)(id >> 24);
                result[i++] = (byte)(id >> 16);
                result[i++] = (byte)(id >> 8);
                result[i++] = (byte) id;

                result[i++] = (byte)(command >> 8);
                result[i++] = (byte)command;

                result[i++] = (byte)(sampleCounter >> 24);
                result[i++] = (byte)(sampleCounter >> 16);
                result[i++] = (byte)(sampleCounter >> 8);
                result[i++] = (byte)sampleCounter;

                result[i++] = (byte)(datagramCounter >> 24);
                result[i++] = (byte)(datagramCounter >> 16);
                result[i++] = (byte)(datagramCounter >> 8);
                result[i++] = (byte)datagramCounter;

                result[i++] = (byte)(crc >> 24);
                result[i++] = (byte)(crc >> 16);
                result[i++] = (byte)(crc >> 8);
                result[i++] = (byte)crc;

                result[i++] = (byte)(timecode >> 24);
                result[i++] = (byte)(timecode >> 16);
                result[i++] = (byte)(timecode >> 8);
                result[i++] = (byte)timecode;
                
                result[i++] = (byte)dataFormat;
                
                //9 bytes reserved
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;
                result[i++] = 0;

                return result;
            }
        }

        public void Process(byte[] readData)
        {
            //TODO
        }


        /// <summary>
        /// Enum for identify message types
        /// </summary>
        public enum MessageType
        {
            Invalid         = 0,
            CustomData1     = 1,
            CustomData2     = 2
        }

        private FormatterBase packet_data;
        public Header packet_header;

        /// <summary>
        /// Parses the payload depends on the current network mode.
        /// </summary>
        // protected abstract double[] Parse(BinaryReader br);
        
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name='readData'>
        /// Create the packed from this byte array.
        /// </param>
        public DataPacket(byte[] readData)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(readData))) 
            {
                //msg = null;
                int command = GetCommand(br);
                
                Debug.Log("command: " + command);

            }
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DataPacket()
        {

            if(useHeader)
            {
            	packet_header = new Header();
            	packet_header.dataFormat = DataFormat.JSON; //TODO

            	if (packet_header.dataFormat == DataFormat.JSON)
            	{
            		packet_data = new JSONFormatter();
            	}
            }
            else
            {
                //we use JSON without header
                packet_data = new JSONFormatter();
            }
        }

		public void SetValue(string key, string value)
		{
			packet_data.SetValue(key, value);
		}
		
		public string GetValue(string key)
		{
			return packet_data.GetValue(key);
		}

      
        public byte[] Serialize(List<string> keys = null)
        {

    		List<byte> ret = new List<byte>();
            if(useHeader)
            {
    		  ret.AddRange(packet_header.Serialize());
            }

        	string json = packet_data.Serialize(keys);
			ret.AddRange(System.Text.Encoding.UTF8.GetBytes(json));
        	
            return ret.ToArray();
        }

        /// <summary>
        /// Parses the header.
        /// </summary>
        private int GetCommand(BinaryReader br)
        {
            br.ReadBytes(4); //TODO: seek
            return ConvertMessageType(br.ReadBytes(2));
        }
        
        /// <summary>
        /// Converts the type of the message from string to int.
        /// </summary>
        /// <returns>
        /// The message type as integer.
        /// </returns>
        /// <param name='incomingByteArray'>
        /// Incoming byte array.
        /// </param>
        protected int ConvertMessageType(byte[] incomingByteArray)
        {
            int id = (int)MessageType.Invalid;
            if (incomingByteArray.Count() == 2)
            {
                id = (incomingByteArray[0] - 0x30) * 10;
                id += (incomingByteArray[1] - 0x30);
            }
            return id;         
        }

        /// <summary>
        /// Since the binary reader is small endian, and the data from the packet is big endian we need to convert the data
        /// This is done here, and simply puts the reverse data into a temp buffer and the memorystream and binaryreader make an integer of the data
        /// </summary>
        /// <param name="incomingByteArray"></param>
        /// <returns></returns>
        protected int Convert32BitInt(byte[] incomingByteArray)
        {
            byte[] tempByteArray = new byte[4];
         
            //Log.Debug("incomByteArray.Count="+incomingByteArray.Count());
         
            if (incomingByteArray.Count() >= 4)
            {
                tempByteArray[0] = incomingByteArray[3];
                tempByteArray[1] = incomingByteArray[2];
                tempByteArray[2] = incomingByteArray[1];
                tempByteArray[3] = incomingByteArray[0];
            }
            else
            {
                Log.Error("invalid data size:" + incomingByteArray.Count());
            }
     
            return BitConverter.ToInt32(tempByteArray, 0);

        }

        /// <summary>
        /// Since the binary reader is small endian, and the data from the packet is big endian we need to convert the data
        /// This is done here, and simply puts the reverse data into a temp buffer and the memorystream and binaryreader make an float of the data
        /// </summary>
        /// <param name="incomingByteArray"></param>
        /// <returns></returns>
        protected double Convert32BitFloat(byte[] incomingByteArray)
        {
            byte[] tempByteArray = new byte[4];
         
            if (incomingByteArray.Count() >= 4)
            {
                tempByteArray[0] = incomingByteArray[3];
                tempByteArray[1] = incomingByteArray[2];
                tempByteArray[2] = incomingByteArray[1];
                tempByteArray[3] = incomingByteArray[0];
            } 
            else 
            {
                Log.Debug("invalid Float data size:" + incomingByteArray.Count());
            }
         
            return BitConverter.ToSingle (tempByteArray, 0);
        }
        
    }//class
}//namespace