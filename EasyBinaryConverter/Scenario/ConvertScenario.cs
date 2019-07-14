using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EasyBinaryConverter.Scenario
{
    public class ConvertScenario<T>
    {
        private List<KeyValuePair<Type, Property>> _properties;

        public ConvertScenario()
        {
            _properties = new List<KeyValuePair<Type, Property>>();
        }

        public void AddStep(Type propertyType, PropertyInfo propertyInfo)
            => _properties.Add(new KeyValuePair<Type, Property>(propertyType, new Property(propertyInfo)));

        public KeyValuePair<Type, Property>[] GetSteps() => _properties.ToArray();

        public class Property
        {
            public PropertyInfo Info { get; set; }
            public bool IsNeedSkip => Info == null;

            public Property(PropertyInfo info)
            {
                Info = info;
            }
        }
    }
}
