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
        /// Текущая версия конвертера.
        /// </summary>
        private int CurrentVersion;

        /// <summary>
        /// Сценарии по версиям.
        /// </summary>
        private readonly Dictionary<int, ConvertScenario<T>> _convertScenariosByVersion;

        internal BytesConverter(ITypesBinaryConverter typesBinaryConverter)
        {
            _typesBinaryConverter = typesBinaryConverter;

            CurrentVersion = 0;
            _convertScenariosByVersion = new Dictionary<int, ConvertScenario<T>>();
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
                // Записываем текущую версию.
                bw.Write(CurrentVersion);

                // Записываем все поля
                var steps = _convertScenariosByVersion[CurrentVersion].GetSteps();
                foreach (var step in steps)
                {
                    if (step.Value.IsNeedSkip)
                        continue;

                    object value = step.Value.Info.GetValue(obj);
                    _typesBinaryConverter.Write(bw, step.Key, value);
                }

                return ms.ToArray();
            }
        }

        private T Deserialize(BinaryReader br)
        {
            // Получаем версию объекта.
            int version = br.ReadInt32();

            if (!_convertScenariosByVersion.ContainsKey(version))
                throw new Exception($"Нет зарегистрированного сценария для версии {version} типа {typeof(T)}");

            T newObject = new T();

            // Вычитываем все поля
            var steps = _convertScenariosByVersion[version].GetSteps();
            foreach (var step in steps)
            {
                object value = _typesBinaryConverter.Read(br, step.Key);

                if (!step.Value.IsNeedSkip)
                    step.Value.Info.SetValue(newObject, value);
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

        internal void SetVersionConvertScenario(int version, ConvertScenario<T> scenario)
        {
            CurrentVersion = Math.Max(CurrentVersion, version);
            _convertScenariosByVersion[version] = scenario;
        }
    }
}
