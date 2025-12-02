using System;
using System.Collections.Generic;
using System.Linq;
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

        public int Count;// { get; private set; }
        public string lastModifiedAttribute;

        public PCGPointData(int pointAmount)
        {
            Count = pointAmount;
        }

        public PCGPointData(PCGPointData copyData)
        {
            Count = copyData.Count;
            _attributes = copyData._attributes.ToDictionary(entry => entry.Key, entry => entry.Value.Clone());
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
            if (name == DefaultAttributes.LastModifiedAttribute)
                name = lastModifiedAttribute;
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
            if (name == DefaultAttributes.LastModifiedAttribute)
                name = lastModifiedAttribute;
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


        public void SetAttribute<T>(string name, int pointIndex, T value)
        {
            if (_attributes.TryGetValue(name, out var buffer))
            {
                ((AttributeBuffer<T>)buffer).Data[pointIndex] = value;
            }
            lastModifiedAttribute = name;
        }

        public string[] GetAttributeNames()
        {
            return _attributes.Keys.ToArray();
        }

    }
}
