using EasyBinaryConverter.Converters;
using EasyBinaryConverter.Scenario;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyBinaryConverter
{
    public class ConvertService
    {
        private readonly ITypesBinaryConverter _typesBinaryConverter;

        /// <summary>
        /// Содержит в себе все построенные конвертеры.
        /// </summary>
        private readonly Dictionary<Type, IBytesConverter> _converters;

        public ConvertService(ITypesBinaryConverter typesBinaryConverter = null)
        {
            _typesBinaryConverter = typesBinaryConverter ?? new DefaultTypesBinaryConverter();
            _converters = new Dictionary<Type, IBytesConverter>();
        }

        /// <summary>
        /// Возвращает конвертер для типа.
        /// </summary>
        public BytesConverter<T> Converter<T>() where T : class, new()
        {
            if (!_converters.ContainsKey(typeof(T)))
                throw new Exception("Не найден конвертер данного типа");

            return (BytesConverter<T>)_converters[typeof(T)];
        }

        /// <summary>
        /// Возвращает билдер сценария конвертации для типа.
        /// </summary>
        public ConvertScenarioBuilder<T> NewBuilder<T>() where T : class, new() => new ConvertScenarioBuilder<T>(this);

        /// <summary>
        /// Добавляет новый сценарий конвертации типа.
        /// </summary>
        internal void SetConvertScenario<T>(ConvertScenario<T> scenario) where T : class, new()
        {
            _converters[typeof(T)] = new BytesConverter<T>(scenario, _typesBinaryConverter);
            _typesBinaryConverter.SetConverter(Converter<T>());
        }
    }
}
