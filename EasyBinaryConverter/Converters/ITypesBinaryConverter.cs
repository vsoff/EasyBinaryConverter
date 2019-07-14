using System;
using System.IO;
using System.Text;

namespace EasyBinaryConverter.Converters
{
    public interface ITypesBinaryConverter
    {
        void SetConverter<T>(BytesConverter<T> converter) where T : class, new();
        void Write(BinaryWriter writer, Type type, object obj);
        object Read(BinaryReader reader, Type type);
    }
}
