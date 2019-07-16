using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyBinaryConverter.Scenario
{
    public class ConvertScenario<T>
    {
        private Dictionary<int, Property> _propertiesByTag;

        public ConvertScenario()
        {
            _propertiesByTag = new Dictionary<int, Property>();
        }

        public void AddStep(PropertyInfo propertyInfo, int tag)
        {
            if (_propertiesByTag.ContainsKey(tag))
                throw new Exception($"Тег '{tag}' уже был добавлен ранее");

            _propertiesByTag.Add(tag, new Property(tag, propertyInfo));
        }

        public Property[] GetSteps() => _propertiesByTag.Select(x => x.Value).ToArray();
        public Property GetStep(int tag) => _propertiesByTag.ContainsKey(tag) ? _propertiesByTag[tag] : null;

        public class Property
        {
            public int Tag { get; set; }
            public PropertyInfo Info { get; set; }
            public Type Type => Info.PropertyType;
            public bool IsNeedSkip => Info == null;

            public Property(int tag, PropertyInfo info)
            {
                Tag = tag;
                Info = info;
            }
        }
    }
}
