using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG
{
    // Interface allows us to store different types in one dictionary
    public interface IAttributeBuffer
    {
        void Init(int count);
        //void Resize(int newSize);
        //void Copy(int sourceIndex, int destIndex);
        System.Type GetDataType();
    }

    // The concrete implementation for specific types (Float, Vector3, Bool, etc)
    public class AttributeBuffer<T> : IAttributeBuffer
    {
        public T[] Data;

        public void Init(int count)
        {
            Data = new T[count];
        }

        /*public void Resize(int newSize)
        {
            // Logic to expand list or trim it
            while (Data.Count < newSize) Data.Add(default);
        }*/

        /*public void Copy(int sourceIndex, int destIndex)
        {
            if (sourceIndex < Data.Count && destIndex < Data.Count)
                Data[destIndex] = Data[sourceIndex];
        }*/

        public System.Type GetDataType() => typeof(T);
    }

    public class PCGPointData
    {
        private Dictionary<string, IAttributeBuffer> _attributes = new();

        public int Count { get; private set; }
        public string lastModifiedAttribute;

        public PCGPointData(int pointAmount)
        {
            Count = pointAmount;

            /*foreach (var attr in _attributes.Values)
            {
                attr.Init(Count);
            }*/
        }

        public void CreateAttribute<T>(string name, T defaultValue = default)
        {
            if (_attributes.ContainsKey(name)) return;

            var buffer = new AttributeBuffer<T>();
            // Fill existing points with default value
            for (int i = 0; i < Count; i++) buffer.Data[i] = defaultValue;

            _attributes.Add(name, buffer);
        }

        public T[] GetAttributeList<T>(string name)
        {
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
            if (_attributes.TryGetValue(name, out var buffer))
            {
                ((AttributeBuffer<T>)buffer).Data = value;
            }
            else
            {
                var buffer1 = new AttributeBuffer<T>();
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
