using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace aspnetapp
{
    public class SaveWriter : IDisposable
    {
        public const byte NULL = 127;

        private Stream stream;

        public long Position { get => stream.Position; set => stream.Position = value; }

        public long Length => stream.Length;

        public SaveWriter(byte[] _bytes)
        {
            stream = new MemoryStream(_bytes);
            if (!stream.CanWrite)
                throw new Exception("Can't write to stream");
            Position = 0;
        }

        public SaveWriter(Stream _stream)
        {
            stream = _stream;
            if (!stream.CanWrite)
                throw new Exception("Can't write to stream");
            Position = 0;
        }

        public SaveWriter(string _path, bool clear = true)
        {
            if (!File.Exists(_path) || clear)
                File.WriteAllBytes(_path, new byte[] { });

            stream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write);
            if (!stream.CanWrite)
                throw new Exception("Can't write to stream");
            Position = 0;
        }

        public void Reset() => stream.Position = 0;

        public void WriteBytes(byte[] bytes) => WriteBytes(bytes, 0, bytes.Length);
        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            stream.Write(bytes, offset, count);
        }

        public void WriteNormalOrSL<T>(T value)
        {
            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISaveLoad<>)))
            {
                MethodInfo writeSL = GetType().GetMethod("WriteSL", BindingFlags.Public | BindingFlags.Instance)
                    .MakeGenericMethod(typeof(T));
                writeSL.Invoke(this, new object[] { value });
            }
            else
                Write(value);
        }

        public void WriteSL<T>(ISaveLoad<T> value) where T : class, ISaveLoad<T>
        {
            if (value is null) WriteUInt8(NULL);
            else { WriteUInt8(0); value.Save(this); }
        }
        public void Write<T>(T value)
        {
            switch (value)
            {
                case null:
                    WriteUInt8(NULL);
                    break;
                case sbyte i8:
                    WriteInt8(i8);
                    break;
                case byte ui8:
                    WriteUInt8(ui8);
                    break;
                case Int16 i16:
                    WriteInt16(i16);
                    break;
                case UInt16 ui16:
                    WriteUInt16(ui16);
                    break;
                case Int32 i32:
                    WriteInt32(i32);
                    break;
                case UInt32 ui32:
                    WriteUInt32(ui32);
                    break;
                case Int64 i64:
                    WriteInt64(i64);
                    break;
                case UInt64 ui64:
                    WriteUInt64(ui64);
                    break;
                case bool b:
                    WriteBool(b);
                    break;
                case float f:
                    WriteFloat(f);
                    break;
                case double d:
                    WriteDouble(d);
                    break;
                case string s:
                    WriteString(s);
                    break;
                case Guid guid:
                    WriteGuid(guid);
                    break;
                case DateTime dt:
                    WriteDateTime(dt);
                    break;
                default:
                    {
                        if (typeof(T) == typeof(object))
                            WriteObject(value);
                        else if (typeof(T).IsArray)
                        {
                            MethodInfo saveArray = GetType().GetMethod("WriteArray", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(typeof(T).GetElementType());
                            saveArray.Invoke(this, new object[] { value });
                        }
                        else if (typeof(T).BaseType.IsGenericType && typeof(T).BaseType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                        {
                            MethodInfo writeList = GetType().GetMethod("WriteList", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(typeof(T).GetGenericArguments());
                            writeList.Invoke(this, new object[] { value });
                        }
                        else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition().IsAssignableFrom(typeof(KeyValuePair<,>)))
                        {
                            MethodInfo writeKeyValuePair = GetType().GetMethod("WriteKeyValuePair", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(typeof(T).GetGenericArguments());
                            writeKeyValuePair.Invoke(this, new object[] { value });
                        }
                        else
                            throw new Exception($"Unexpected type: '{typeof(T)}'.");
                    }
                    break;
            }
        }

        public void WriteObject(object value)
        {
            if (value == null)
                WriteUInt8(NULL);
            else
            {
                WriteUInt8(0);
                string type = value.GetType().FullName;
                if (type == null)
                    throw new Exception($"Type.FullName is null for type: \"{value.GetType()}\"");
                WriteString(type);
                WriteString(JsonConvert.SerializeObject(value));
            }
        }

        public void WriteInt8(sbyte value)
        {
            WriteBytes(new byte[] { (byte)value });
        }
        public void WriteInt8Null(sbyte? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0, (byte)value.Value });
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteUInt8(byte value)
        {
            WriteBytes(new byte[] { value });
        }
        public void WriteUInt8Null(byte? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0, value.Value });
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteInt16(Int16 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteInt16Null(Int16? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteUInt16(UInt16 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteUInt16Null(UInt16? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteInt32(Int32 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteInt32Null(Int32? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteUInt32(UInt32 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteUInt32Null(UInt32? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteInt64(Int64 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteInt64Null(Int64? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteUInt64(UInt64 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteUInt64Null(UInt64? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteBool(bool value)
        {
            WriteUInt8((byte)(value ? 1 : 0));
        }
        public void WriteBoolNull(bool? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(new byte[] { (byte)(value.Value ? 1 : 0) }).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteFloat(float value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteFloatNull(float? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteDouble(double value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteDoubleNull(double? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteString(string value)
        {
            if (value == null)
                WriteUInt8(NULL);
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                if (bytes.Length > ushort.MaxValue)
                    throw new Exception($"String too long ({bytes.Length}) max {ushort.MaxValue}, string: \"{value}\"");

                WriteUInt8(0);
                WriteUInt16((ushort)bytes.Length);
                WriteBytes(bytes);
            }
        }

        public void WriteGuid(Guid value)
        {
            WriteBytes(value.ToByteArray());
        }
        public void WriteGuidNull(Guid? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(value.Value.ToByteArray()).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteDateTime(DateTime value)
        {
            WriteBytes(BitConverter.GetBytes(value.Ticks));
        }
        public void WriteDateTimeNull(DateTime? value)
        {
            if (value.HasValue)
                WriteBytes(new byte[] { 0 }.Concat(BitConverter.GetBytes(value.Value.Ticks)).ToArray());
            else
                WriteBytes(new byte[] { NULL });
        }

        public void WriteKeyValuePair<A, B>(KeyValuePair<A, B> value)
        {
            WriteNormalOrSL(value.Key);
            WriteNormalOrSL(value.Value);
        }
        public void WriteKeyValuePairNull<A, B>(KeyValuePair<A, B>? value)
        {
            if (value.HasValue)
            {
                WriteUInt8(0);
                WriteKeyValuePair(value.Value);
            }
            else
                WriteUInt8(NULL);
        }

        public void WriteArray<T>(T[] value)
        {
            if (value == null)
                WriteUInt8(NULL);
            else
            {
                WriteUInt8(0);
                WriteInt32(value.Length);
                Type[] interfaces = typeof(T).GetInterfaces();

                if (interfaces.Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISaveLoad<>)))
                {
                    MethodInfo writeSL = GetType().GetMethod("WriteSL", BindingFlags.Public |BindingFlags.Instance).MakeGenericMethod(typeof(T));
                    for (int i = 0; i < value.Length; i++)
                        writeSL.Invoke(this, new object[] { value[i] });
                }
                else
                    for (int i = 0; i < value.Length; i++)
                        Write(value[i]);
            }
        }
        public void WriteList<T>(List<T> value)
        {
            if (value == null)
                WriteUInt8(NULL);
            else
            {
                WriteUInt8(0);
                WriteInt32(value.Count);
                Type[] interfaces = typeof(T).GetInterfaces();

                if (interfaces.Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ISaveLoad<>)))
                {
                    MethodInfo writeSL = GetType().GetMethod("WriteSL", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(typeof(T));

                    for (int i = 0; i < value.Count; i++)
                        writeSL.Invoke(this, new object[] { value[i] });
                }
                else
                    for (int i = 0; i < value.Count; i++)
                        Write(value[i]);
            }
        }

        public void Flush() => stream.Flush();

        public void Dispose()
        {
            stream.Close();
            stream.Dispose();
        }
    }
}