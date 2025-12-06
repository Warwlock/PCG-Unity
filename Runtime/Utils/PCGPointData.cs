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
        //void Resize(int newSize);
        //void Copy(int sourceIndex, int destIndex);
        System.Type GetDataType();
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
    }

    [Serializable]
    public class PCGPointData
    {
        private Dictionary<string, IAttributeBuffer> _attributes = new();
        //private Dictionary<string, Type> _attributeTypes = new();

        public int Count;// { get; private set; }
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
            for (int i = 0; i < Count; i++) buffer.Data[i] = defaultValue;

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


        public string[] GetAttributeNames()
        {
            return _attributes.Keys.ToArray();
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

            Debug.Log(stripAxis);


            return name.Split('.').First();
        }
    }
}
