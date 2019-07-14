using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EasyBinaryConverter.Scenario
{
    public class ConvertScenarioBuilder<T> where T : class, new()
    {
        private readonly ConvertService _convertService;
        private readonly ConvertScenario<T> _scenario;

        public ConvertScenarioBuilder(ConvertService convertService)
        {
            _convertService = convertService;
            _scenario = new ConvertScenario<T>();
        }

        /// <summary>
        /// Добавляет поле для сериализации.
        /// </summary>
        public ConvertScenarioBuilder<T> Set<FieldType>(Expression<Func<T, FieldType>> expression)
        {
            var currentStep = expression.Body as MemberExpression;
            var prop = currentStep.Member as PropertyInfo;
            _scenario.AddStep(prop.PropertyType, prop);

            return this;
        }

        /// <summary>
        /// Помечает тип, как необходимый для пропуска.
        /// </summary>
        public ConvertScenarioBuilder<T> Skip(Type fieldType)
        {
            _scenario.AddStep(fieldType, null);

            return this;
        }

        /// <summary>
        /// Строит сценарий конвертации объекта для указанной версии.
        /// </summary>
        public void Build(int version) => _convertService.SetConvertScenario(version, _scenario);

        /// <summary>
        /// Строит кастомный сценарий конвертации объекта для указанной версии.
        /// </summary>
        public void BuildCustom(int version, Func<BinaryWriter, byte[]> write, Func<BinaryReader, T> read)
        {
            throw new NotImplementedException();
        }
    }
}
