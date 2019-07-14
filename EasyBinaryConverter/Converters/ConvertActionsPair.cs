using System;
using System.IO;

namespace EasyBinaryConverter.Converters
{
    public class ConvertActionsPair
    {
        public Action<BinaryWriter, object> Write { get; set; }
        public Func<BinaryReader, Type, object> Read { get; set; }

        public ConvertActionsPair(Action<BinaryWriter, object> write, Func<BinaryReader, Type, object> read)
        {
            Write = write;
            Read = read;
        }
    }
}
