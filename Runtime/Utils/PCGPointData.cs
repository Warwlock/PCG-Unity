using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG
{
    public interface IAttributeBuffer
    {
        void Init(int count);
        IAttributeBuffer Clone();
        System.Type GetDataType();
        object GetValue(int index);
    }

    public class AttributeBuffer<T> : IAttributeBuffer
    {
        public T[] Data;

        public void Init(int count)
        {
            Data = new T[count];
        }

        public System.Type GetDataType() => typeof(T);

        public IAttributeBuffer Clone()
        {
            var buffer = new AttributeBuffer<T>();
            buffer.Data = (T[])Data.Clone();
            return buffer;
        }

        public object GetValue(int index)
        {
            return Data[index];
        }
    }

    [Serializable]
    public class PCGPointData
    {
        private Dictionary<string, IAttributeBuffer> _attributes = new();

        public Dictionary<string, IAttributeBuffer> Attributes { get { return _attributes; } }
        //private Dictionary<string, Type> _attributeTypes = new();

        public int Count { get; private set; }
        public string lastModifiedAttribute;
        public bool requiresStripping = false;
        public int stripAxis = 0;

        public PCGPointData(int pointAmount)
        {
            Count = pointAmount;
        }

        public PCGPointData(PCGPointData copyData)
        {
            Count = copyData.Count;
            _attributes = copyData._attributes.ToDictionary(entry => entry.Key, entry => entry.Value.Clone());
            //_attributeTypes = copyData._attributeTypes.ToDictionary(entry => entry.Key, entry => entry.Value);
            lastModifiedAttribute = copyData.lastModifiedAttribute;
        }

        public void CreateAttribute<T>(string name, T defaultValue = default)
        {
            if (_attributes.ContainsKey(name)) return;

            var buffer = new AttributeBuffer<T>();
            buffer.Data = new T[Count];
            Array.Fill(buffer.Data, defaultValue);

            _attributes.Add(name, buffer);
        }

        public T[] GetAttributeList<T>(string name)
        {
            name = NameSeparator(name);

            if (_attributes.TryGetValue(name, out var buffer))
            {
                return ((AttributeBuffer<T>)buffer).Data;
            }
            return null;
        }

        public T GetAttribute<T>(string name, int pointIndex)
        {
            if (_attributes.TryGetValue(name, out var buffer))
            {
                return ((AttributeBuffer<T>)buffer).Data[pointIndex];
            }
            return default;
        }

        public void SetAttributeList<T>(string name, T[] value)
        {
            name = NameSeparator(name);

            if (_attributes.TryGetValue(name, out var buffer))
            {
                ((AttributeBuffer<T>)buffer).Data = value;
            }
            else
            {
                AttributeBuffer<T> buffer1 = new();
                buffer1.Data = value;
                _attributes.Add(name, buffer1);
            }
            lastModifiedAttribute = name;
        }

        public object GetAttributeObject(string name, int index)
        {
            if (_attributes.TryGetValue(name, out var buffer))
            {
                return buffer.GetValue(index);
            }
            return null;
        }


        public string[] GetAttributeNames()
        {
            return _attributes.Keys.ToArray();
        }

        public bool IsEmpty()
        {
            if (_attributes == null)
                return true;
            return _attributes.Count == 0;
        }

        public Type GetDataType(string name)
        {
            name = NameSeparator(name);

            if (_attributes.TryGetValue(name, out var buffer))
            {
                if (name.Contains("."))
                    return typeof(float);

                return buffer.GetDataType();
            }

            return null;
        }

        string NameSeparator(string name)
        {
            if (name == DefaultAttributes.LastModifiedAttribute)
                name = lastModifiedAttribute;

            if (name.Contains("."))
                requiresStripping = true;
            else
                requiresStripping = false;

            stripAxis = 0;

            if (!requiresStripping)
                return name;

            if (name.Split('.').Last()[0] == 'X')
                stripAxis = 1;
            else if (name.Split('.').Last()[0] == 'Y')
                stripAxis = 2;
            else if (name.Split('.').Last()[0] == 'Z')
                stripAxis = 3;
            else if (name.Split('.').Last()[0] == 'W')
                stripAxis = 4;
            else
                stripAxis = 0;

            if (name.Split('.').First() == DefaultAttributes.LastModifiedAttribute)
                name = lastModifiedAttribute;

            return name.Split('.').First();
        }
    }
}
