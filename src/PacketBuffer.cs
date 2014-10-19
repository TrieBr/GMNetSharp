using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace GMNetSharp
{
    /*
     * Structure for building and reading packets
     * */
    public class PacketBuffer
    {
        //Data stream for the packet
        private MemoryStream data;

        //Packet with no data
        public PacketBuffer()
        {
            data = new MemoryStream(256);
        }
        //Packet with existing data
        public PacketBuffer(byte[] bytes, int index, int count)
        {
            data = new MemoryStream(bytes,index,count);
        }
        //Get the raw bytes in the packet
        public byte[] GetData()
        {
            return data.GetBuffer();
        }
        //Get the length of the bytes in the packet
        public int GetLength()
        {
            return (int)data.Position;
        }

        /*
         * Writing Operations
         * */
        //Write 8 bit integer (char/short byte)
        public void WriteInt8(sbyte val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write 8 bit Unsigned Byte to the buffer
        public void WriteUInt8(byte val)
        {
            data.WriteByte(val);
        }
        //Write 16 bit Integer
        public void WriteInt16(Int16 val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write 16 bit Unsigned integer
        public void WriteUInt16(UInt16 val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write 32 bit signed integer
        public void WriteInt32(Int32 val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write 32 bit unsigned integer
        public void WriteUInt32(UInt32 val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write 32 bit float
        public void WriteFloat(float val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write 64 bit float
        public void WriteDouble(double val)
        {
            Write(BitConverter.GetBytes(val));
        }
        //Write a string (Null terminated, or Not (length written))
        public void WriteString(String str, bool nullTerminated = true) {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str); 
            if (!nullTerminated)
                WriteInt16((Int16)bytes.Length);
            Write(bytes);
            if (nullTerminated)
                WriteUInt8(0);   //Null terminator
        }
        //Write byte array
        public void Write(byte[] buf)
        {
            data.Write(buf, 0, buf.Length);
        }

        /*
         *  Reading Operations
         * */
        //Read 8-bit integer
        public sbyte ReadInt8()
        {
            return (sbyte)Read(sizeof(sbyte))[0];
        }
        //Read 8-bit unsigned integer
        public byte ReadUInt8()
        {
            return Read(sizeof(byte))[0];
        }
        //Read 16-bit integer
        public Int16 ReadInt16()
        {
            return BitConverter.ToInt16(Read(sizeof(Int16)), 0);
        }
        //Read 16-bit unsigned integer
        public UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(Read(sizeof(UInt16)), 0);
        }
        //Read 32-bit integer
        public Int32 ReadInt32()
        {
            return BitConverter.ToInt32(Read(sizeof(Int32)), 0);
        }
        //Read 32-bit unsigned integer
        public UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(Read(sizeof(UInt32)),0);
        }
        //Read 32-bit Float
        public float ReadFloat()
        {
            return BitConverter.ToSingle(Read(sizeof(float)), 0);
        }
        //Read 64-bit Float
        public double ReadDouble()
        {
            return BitConverter.ToDouble(Read(sizeof(double)), 0);
        }
        //Read a String (null terminated or regular)
        public String ReadString(bool nullTerminated = true)
        {
            String str = "";
            if (!nullTerminated)
            {
                Int32 stringLength = ReadInt16();
                str = System.Text.Encoding.UTF8.GetString(Read(stringLength), 0, stringLength);
            }
            else
            {
                char c = '\0';
                do
                {
                    c = (char)data.ReadByte();
                    if (c != '\0') str += c;
                } while (c != '\0');
            }
            return str;
        }
        //Read Byte array
        public byte[] Read(int len)
        {
            byte[] buffer = new byte[len];
            data.Read(buffer, 0, len);
            return buffer;
        }
    }
}
