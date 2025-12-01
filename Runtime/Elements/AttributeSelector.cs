using System;
using UnityEngine;

namespace PCG
{
    [Serializable]
    public class AttributeSelector
    {
        public string attributeString;
        [SerializeField] DefaultAttributes.DefaultAttributesEnum _defaultAttributes;

        public DefaultAttributes.DefaultAttributesEnum DefaultAttributes
        {
            get { return  _defaultAttributes; }
            private set
            {
                _defaultAttributes = value;
                attributeString = value.ToString();
            }
        }
    }
}
