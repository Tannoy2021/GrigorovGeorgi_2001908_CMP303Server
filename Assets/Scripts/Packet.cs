using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

    public enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        playerRotation,
        playerDisconnected,
        createBuffSpawner,
        buffSpawned,
        buffPickedUp,
        createFinishLine,
        finishCollected,
}
    public enum ClientPackets
    {
      userConnected = 1,
      playerMovement,

    }
    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        public Packet()
        {
            buffer = new List<byte>();
            readPos = 0;
        }
        public Packet(int _id)
        {
            buffer = new List<byte>();
            readPos = 0;
            Write(_id);
        }
        public Packet(byte[] _data)
        {
            buffer = new List<byte>();
            readPos = 0;
            SetBytes(_data);
        }
        #region Functions
        public void SetBytes(byte[] _data)
        {
             buffer.AddRange(_data);
            readableBuffer = buffer.ToArray();
        }
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
        }
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }
        public int Length()
        {
            return buffer.Count;
        }
        public int UnreadLength()
        {
            return Length() - readPos;
        }
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); 
                readableBuffer = null;
                readPos = 0; 
            }
            else
            {
                readPos -= 4;
            }
        }
        #endregion
        #region Write Data
        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        public void Write(Vector3 _value)
        {
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
        }
        public void Write(Quaternion _value)
        {
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
            Write(_value.w);
        }
        #endregion
        #region Read Data
        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] _value = buffer.GetRange(readPos, _length).ToArray();
                if (_moveReadPos)
                {

                    readPos += _length; 
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }
        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {

                int _value = BitConverter.ToInt32(readableBuffer, readPos);
                if (_moveReadPos)
                {

                    readPos += sizeof(int); 
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }
        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float _value = BitConverter.ToSingle(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += sizeof(int);
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }
        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos);
                if (_moveReadPos)
                {
 
                    readPos += sizeof(bool);
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }
    public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
