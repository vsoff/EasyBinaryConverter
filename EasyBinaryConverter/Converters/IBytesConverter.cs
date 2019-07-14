using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyBinaryConverter.Converters
{
    public interface IBytesConverter
    {
        byte[] SerializeObject(object obj);
        object DeserializeObject(BinaryReader binaryReader);
    }
}
