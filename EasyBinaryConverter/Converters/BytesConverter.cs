using EasyBinaryConverter.Scenario;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyBinaryConverter.Converters
{
    public class BytesConverter<T> : IBytesConverter where T : class, new()
    {
        private readonly ITypesBinaryConverter _typesBinaryConverter;

        /// <summary>
        /// Сценарии по версиям.
        /// </summary>
        private readonly ConvertScenario<T> _scenario;

        internal BytesConverter(ConvertScenario<T> scenario, ITypesBinaryConverter typesBinaryConverter)
        {
            _typesBinaryConverter = typesBinaryConverter;

            _scenario = scenario;
        }

        public byte[] SerializeObject(object obj)
        {
            Type objType = obj.GetType();

            if (!objType.Equals(typeof(T)))
                throw new Exception($"Тип {objType} не соответствует ожиданию {typeof(T)}");

            return Serialize((T)obj);
        }
        public object DeserializeObject(BinaryReader binaryReader) => Deserialize(binaryReader);

        public byte[] Serialize(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                // Записываем все поля
                var steps = _scenario.GetSteps();
                foreach (var step in steps)
                {
                    if (step.IsNeedSkip)
                        continue;

                    // Записываем тег.
                    bw.Write(step.Tag);

                    // Записываем значение.
                    object value = step.Info.GetValue(obj);
                    _typesBinaryConverter.Write(bw, step.Type, value);
                }

                return ms.ToArray();
            }
        }

        private T Deserialize(BinaryReader br)
        {
            Stream stream = br.BaseStream;
            T newObject = new T();

            // Вычитываем все поля.
            while (stream.Position != stream.Length)
            {
                int tag = br.ReadInt32();
                var step = _scenario.GetStep(tag);
                object value = _typesBinaryConverter.Read(br, step.Type);
                step.Info.SetValue(newObject, value);
            }

            return newObject;
        }

        public T Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            using (BinaryReader br = new BinaryReader(ms))
            {
                return Deserialize(br);
            }
        }
    }
}
