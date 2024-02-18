using Newtonsoft.Json;
using System.Reflection;
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

        public T ReadNormalOrSL<T>()
        {
            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISaveLoad<>)))
            {
                MethodInfo readSL = GetType().GetMethod("ReadSL", BindingFlags.Public | BindingFlags.Instance)
                    .MakeGenericMethod(typeof(T));
                return (T)readSL.Invoke(this, null);
            }
            else
                return Read<T>();
        }

        public T ReadSL<T>() where T : class, ISaveLoad<T>
        {
            if (ReadUInt8() == SaveWriter.NULL) return null;
            else return T.Load(this);
        }

        public T Read<T>()
        {
            switch (typeof(T))
            {
                case Type o when o == typeof(object): return (T)ReadObject();
                case Type i8 when i8 == typeof(sbyte): return (T)(object)ReadInt8();
                case Type i8n when i8n == typeof(sbyte?): return (T)(object)ReadInt8Null();
                case Type ui8 when ui8 == typeof(byte): return (T)(object)ReadInt8();
                case Type ui8n when ui8n == typeof(byte?): return (T)(object)ReadInt8Null();
                case Type i16 when i16 == typeof(Int16): return (T)(object)ReadInt16();
                case Type i16n when i16n == typeof(Int16?): return (T)(object)ReadInt16Null();
                case Type ui16 when ui16 == typeof(UInt16): return (T)(object)ReadUInt16();
                case Type ui16n when ui16n == typeof(UInt16?): return (T)(object)ReadUInt16Null();
                case Type i32 when i32 == typeof(Int32): return (T)(object)ReadInt32();
                case Type i32n when i32n == typeof(Int32?): return (T)(object)ReadInt32Null();
                case Type ui32 when ui32 == typeof(UInt32): return (T)(object)ReadUInt32();
                case Type ui32n when ui32n == typeof(UInt32?): return (T)(object)ReadUInt32Null();
                case Type i64 when i64 == typeof(Int64): return (T)(object)ReadInt64();
                case Type i64n when i64n == typeof(Int64?): return (T)(object)ReadInt64Null();
                case Type ui64 when ui64 == typeof(UInt64): return (T)(object)ReadUInt64();
                case Type ui64n when ui64n == typeof(UInt64?): return (T)(object)ReadUInt64Null();
                case Type b when b == typeof(bool): return (T)(object)ReadBool();
                case Type bn when bn == typeof(bool?): return (T)(object)ReadBoolNull();
                case Type f when f == typeof(float): return (T)(object)ReadFloat();
                case Type fn when fn == typeof(float?): return (T)(object)ReadFloatNull();
                case Type d when d == typeof(double): return (T)(object)ReadDouble();
                case Type dn when dn == typeof(double?): return (T)(object)ReadDoubleNull();
                case Type s when s == typeof(string): return (T)(object)ReadString();
                case Type guid when guid == typeof(Guid): return (T)(object)ReadGuid();
                case Type guidn when guidn == typeof(Guid?): return (T)(object)ReadGuidNull();
                case Type dt when dt == typeof(DateTime): return (T)(object)ReadDateTime();
                case Type dtn when dtn == typeof(DateTime?): return (T)(object)ReadDateTimeNull();
                case Type kvp when kvp.IsGenericType && kvp.GetGenericTypeDefinition().IsAssignableFrom(typeof(KeyValuePair<,>)):
                    {
                        MethodInfo readKeyValuePair = GetType().GetMethod("ReadKeyValuePair", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(kvp.GetGenericArguments());
                        return (T)readKeyValuePair.Invoke(this, null);
                    }
                case Type a when a.IsArray:
                    {
                        MethodInfo readArray = GetType().GetMethod("ReadArray", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(typeof(T).GetElementType());
                        return (T)readArray.Invoke(this, null);
                    }
                case Type kvp when kvp.BaseType.IsGenericType && kvp.BaseType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)):
                    {
                        MethodInfo readList = GetType().GetMethod("ReadList", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(kvp.GetGenericArguments());
                        return (T)readList.Invoke(this, null);
                    }
                default:
                    throw new Exception($"Unexpected type: '{typeof(T)}'.");
            }
        }

        public object ReadObject()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
            {
                Type type = Type.GetType(ReadString());
                return JsonConvert.DeserializeObject(ReadString(), type);
            }
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadBytes(1)[0];
        }
        public sbyte? ReadInt8Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadInt8();
        }

        public byte ReadUInt8()
        {
            return ReadBytes(1)[0];
        }
        public byte? ReadUInt8Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadUInt8();
        }

        public Int16 ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }
        public Int16? ReadInt16Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadInt16();
        }

        public UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }
        public UInt16? ReadUInt16Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadUInt16();
        }

        public Int32 ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }
        public Int32? ReadInt32Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadInt32();
        }

        public UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }
        public UInt32? ReadUInt32Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadUInt32();
        }

        public Int64 ReadInt64()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }
        public Int64? ReadInt64Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadInt64();
        }

        public UInt64 ReadUInt64()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }
        public UInt64? ReadUInt64Null()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadUInt64();
        }

        public bool ReadBool()
        {
            return ReadUInt8() != 0;
        }
        public bool? ReadBoolNull()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadBool();
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }
        public float? ReadFloatNull()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadFloat();
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }
        public double? ReadDoubleNull()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadDouble();
        }

        public string ReadString()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
            {
                ushort length = ReadUInt16();
                return Encoding.UTF8.GetString(ReadBytes(length));
            }
        }

        public Guid ReadGuid()
        {
            return new Guid(ReadBytes(16));
        }
        public Guid? ReadGuidNull()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadGuid();
        }

        public DateTime ReadDateTime()
            => new DateTime(ReadInt64());
        public DateTime? ReadDateTimeNull()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadDateTime();
        }

        public KeyValuePair<A, B> ReadKeyValuePair<A, B>()
            => new KeyValuePair<A, B>(ReadNormalOrSL<A>(), ReadNormalOrSL<B>());
        public KeyValuePair<A, B>? ReadKeyValuePairNull<A, B>()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
                return ReadKeyValuePair<A, B>();
        }

        public T[] ReadArray<T>()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
            {
                T[] array = new T[ReadInt32()];
                Type[] interfaces = typeof(T).GetInterfaces();

                if (interfaces.Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISaveLoad<>)))
                {
                    MethodInfo readSL = GetType().GetMethod("ReadSL", BindingFlags.Public | BindingFlags.Instance)
                        .MakeGenericMethod(typeof(T));
                    for (int i = 0; i < array.Length; i++)
                        array[i] = (T)readSL.Invoke(this, null);
                }
                else
                    for (int i = 0; i < array.Length; i++)
                        array[i] = Read<T>();

                return array;
            }
        }
        public List<T> ReadList<T>()
        {
            if (ReadUInt8() == SaveWriter.NULL)
                return null;
            else
            {
                int count = ReadInt32();
                List<T> list = new List<T>();
                Type[] interfaces = typeof(T).GetInterfaces();

                if (interfaces.Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISaveLoad<>)))
                {
                    MethodInfo readSL = GetType().GetMethod("ReadSL", BindingFlags.Public | BindingFlags.Instance)
                        .MakeGenericMethod(typeof(T));
                    for (int i = 0; i < count; i++)
                        list.Add((T)readSL.Invoke(this, null));
                }
                else
                    for (int i = 0; i < count; i++)
                        list.Add(Read<T>());

                return list;
            }
        }

        public void Dispose()
        {
            Stream.Close();
            Stream.Dispose();
        }
    }
}