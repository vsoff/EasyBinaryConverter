using System;
using System.Collections.Generic;
using System.IO;

namespace EasyBinaryConverter.Converters
{
    public class DefaultTypesBinaryConverter : ITypesBinaryConverter
    {
        /// <summary>
        /// Содержит в себе методы для обработки простых типов.
        /// </summary>
        private readonly Dictionary<Type, ConvertActionsPair> _defaultTypeActions;

        /// <summary>
        /// Содержит в себе конвертеры.
        /// </summary>
        private readonly Dictionary<Type, IBytesConverter> _converters;

        public DefaultTypesBinaryConverter()
        {
            _converters = new Dictionary<Type, IBytesConverter>();


            var actions = new Dictionary<Type, ConvertActionsPair>();
            actions[typeof(int)] = new ConvertActionsPair(
                (bw, obj) => bw.Write((int)obj), (br, type) => br.ReadInt32());
            actions[typeof(bool)] = new ConvertActionsPair(
                (bw, obj) => bw.Write((bool)obj), (br, type) => br.ReadBoolean());
            actions[typeof(string)] = new ConvertActionsPair(
                (bw, obj) => bw.Write((string)obj), (br, type) => br.ReadString());
            actions[typeof(double)] = new ConvertActionsPair(
                (bw, obj) => bw.Write((double)obj), (br, type) => br.ReadDouble());
            actions[typeof(DateTime)] = new ConvertActionsPair(
                (bw, obj) => bw.Write(((DateTime)obj).Ticks), (br, type) => DateTime.MinValue.AddTicks(br.ReadInt64()));
            actions[typeof(Nullable<>)] = new ConvertActionsPair(WriteNullable, ReadNullable);

            _defaultTypeActions = actions;
        }

        private void WriteNullable(BinaryWriter writer, object obj)
        {
            bool isNull = obj == null;
            writer.Write(isNull);
            if (!isNull)
                Write(writer, obj.GetType(), obj);
        }

        private object ReadNullable(BinaryReader reader, Type type)
        {
            bool isNull = reader.ReadBoolean();
            if (isNull)
                return null;

            return Read(reader, type.GetGenericArguments()[0]);
        }

        public void SetConverter<T>(BytesConverter<T> converter) where T : class, new() => _converters[typeof(T)] = converter;

        public void Write(BinaryWriter writer, Type type, object obj)
        {
            Type targetType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (_defaultTypeActions.ContainsKey(targetType))
            {
                _defaultTypeActions[targetType].Write(writer, obj);
            }
            else
            {
                if (type.IsGenericType)
                    throw new Exception($"Невозможно обработать generic тип {targetType}. Generic типы должны быть добавлены в список стандартных обработчиков");

                if (!_converters.ContainsKey(type))
                    throw new Exception($"Для типа {type} не найдено конвертера");

                byte[] bytes = _converters[type].SerializeObject(obj);
                writer.Write(bytes);
            }
        }

        public object Read(BinaryReader reader, Type type)
        {
            Type targetType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (_defaultTypeActions.ContainsKey(targetType))
            {
                return _defaultTypeActions[targetType].Read(reader, type);
            }
            else
            {
                if (type.IsGenericType)
                    throw new Exception($"Невозможно обработать generic тип {targetType}. Generic типы должны быть добавлены в список стандартных обработчиков");

                if (!_converters.ContainsKey(type))
                    throw new Exception($"Для типа {type} не найдено конвертера");

                return _converters[type].DeserializeObject(reader);
            }
        }
    }
}
