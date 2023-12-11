using System.Text;

namespace aspnetapp
{
    public class SaveReader : IDisposable
    {
        public Stream Stream;

        public long Position { get => Stream.Position; set => Stream.Position = value; }

        public long Length => Stream.Length;

        public long BytesLeft => Stream.Length - Position;

        public SaveReader(byte[] _bytes)
        {
            Stream = new MemoryStream(_bytes);
            if (!Stream.CanRead)
                throw new Exception("Can't read from stream");
            Position = 0;
        }

        public SaveReader(Stream _stream)
        {
            Stream = _stream;
            if (!Stream.CanRead)
                throw new Exception("Can't read from stream");
            Position = 0;
        }

        public SaveReader(string _path)
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException($"File \"{_path}\" doesn't exist", _path);

            Stream = new FileStream(_path, FileMode.Open, FileAccess.Read);
            if (!Stream.CanRead)
                throw new Exception("Can't read from stream");
            Position = 0;
        }

        public void Reset() => Stream.Position = 0;

        public byte[] ReadBytes(int count)
        {
            if (BytesLeft < count)
                throw new EndOfStreamException("Reached end of stream");

            byte[] bytes = new byte[count];
            Stream.Read(bytes, 0, count);
            return bytes;
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadBytes(1)[0];
        }

        public byte ReadUInt8()
        {
            return ReadBytes(1)[0];
        }

        public Int16 ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        public UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }

        public Int32 ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        public UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }
        public UInt32? ReadUInt32Nullable()
        {
            UInt32 value = BitConverter.ToUInt32(ReadBytes(4), 0);
            return value == UInt32.MaxValue ? null : value;
        }

        public Int64 ReadInt64()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        public UInt64 ReadUInt64()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }
        public UInt64? ReadUInt64Nullable()
        {
            UInt64 value = BitConverter.ToUInt64(ReadBytes(8), 0);
            return value == UInt64.MaxValue ? null : value;
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public string ReadString()
        {
            short length = ReadInt16();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }
        public string? ReadStringNullable()
        {
            short length = ReadInt16();
            if (length == short.MinValue)
                return null;
            else
                return Encoding.UTF8.GetString(ReadBytes(length));
        }

        public void Dispose()
        {
            Stream.Close();
            Stream.Dispose();
        }
    }
}