using System;
using System.Configuration;
using System.Xml;

namespace AVS.CoreLib.Infrastructure.Config
{
    public abstract class XmlConfigNodeBase
    {
        protected string GetString(XmlNode node, string attrName, bool required = false)
        {
            var value = SetByXElement<string>(node, attrName, Convert.ToString);
            if(string.IsNullOrEmpty(value) && required)
                throw new ConfigurationErrorsException($"{attrName} is required", node);
            return value;
        }
        
        protected bool GetBool(XmlNode node, string attrName)
        {
            return SetByXElement<bool>(node, attrName, Convert.ToBoolean);
        }

        protected int GetInt(XmlNode node, string attrName)
        {
            return SetByXElement<Int32>(node, attrName, Convert.ToInt32);
        }

        protected double GetDouble(XmlNode node, string attrName)
        {
            return SetByXElement<double>(node, attrName, Convert.ToDouble);
        }

        protected double? GetDoubleOrNull(XmlNode node, string attrName)
        {
            var attr = node?.Attributes?[attrName];
            if (attr == null) return null;
            string attrVal = attr.Value;
            return Convert.ToDouble(attrVal);
        }

        protected T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node?.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            string attrVal = attr.Value;
            return converter(attrVal);
        }
    }
}