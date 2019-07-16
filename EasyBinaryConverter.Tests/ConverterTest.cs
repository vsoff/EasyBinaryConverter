using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyBinaryConverter.Tests
{
    [TestClass]
    public class ConverterTest
    {
        [TestMethod]
        public void SerializeAndDeserializeTest()
        {
            // Настраиваем сервис.
            ConvertService convertService = new ConvertService();
            convertService.NewBuilder<People>()
                .Set(1, x => x.FirstName)
                .Set(2, x => x.LastName)
                .Set(3, x => x.BirdthDay)
                .Set(4, x => x.Height)
                .Set(5, x => x.Weight)
                .Set(6, x => x.FingersCount)
                .Set(7, x => x.Dog)
                .Build();

            convertService.NewBuilder<Dog>()
                .Set(1, x => x.Name)
                .Set(2, x => x.BirdthDay)
                .Build();

            // Создаём объект.
            People p = new People
            {
                FirstName = "Shoha",
                LastName = "Vitalievich",
                BirdthDay = DateTime.Now,
                Height = 180.5,
                Weight = 78.2,
                FingersCount = 20,
                Dog = new Dog
                {
                    Name = "Пёс",
                    BirdthDay = DateTime.Now.AddMinutes(-9999)
                }
            };

            // Сериализуем.
            byte[] bytes = convertService.Converter<People>().Serialize(p);

            // Десериализуем.
            People peopleFromBytes = convertService.Converter<People>().Deserialize(bytes);
        }
    }

    internal class People
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirdthDay { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public int? FingersCount { get; set; }
        public Dog Dog { get; set; }
    }

    internal class Dog
    {
        public string Name { get; set; }
        public DateTime BirdthDay { get; set; }
    }
}
