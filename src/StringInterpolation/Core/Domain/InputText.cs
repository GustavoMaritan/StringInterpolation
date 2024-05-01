using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace StringReplacement.Core.Domain
{
    public readonly struct InputText : IEquatable<InputText>
    {
        public InputText(string input)
        {
            InputValue = input;
        }

        public override string ToString()
        {
            return InputValue;
        }

        public int Length => InputValue.Length;
        public override bool Equals(object obj) => obj switch
        {
            null => IsNull,
            InputText input => EqualsImpl(in input),
            string sInput => EqualsImpl(new InputText(sInput)),
            _ => false
        };

        public bool Equals(InputText other) => EqualsImpl(in other);
        public static bool operator !=(InputText x, InputText y) => !x.EqualsImpl(in y);
        public static bool operator !=(string x, InputText y) => !y.EqualsImpl(new InputText(x));
        public static bool operator !=(InputText x, string y) => !x.EqualsImpl(new InputText(y));
        public static bool operator ==(InputText x, InputText y) => x.EqualsImpl(in y);
        public static bool operator ==(string x, InputText y) => y.EqualsImpl(new InputText(x));
        public static bool operator ==(InputText x, string y) => x.EqualsImpl(new InputText(y));

        public override int GetHashCode()
        {
            if (IsNull) return -1;
            if (IsEmpty) return 0;
            var len = TotalLength();
            if (len == 0) return 0;

            if (len <= 256)
            {
                Span<byte> span = stackalloc byte[len];
                var written = CopyTo(span);
                Debug.Assert(written == len);
                return GetHashCode(span);
            }
            else
            {
                var arr = ArrayPool<byte>.Shared.Rent(len);
                var span = new Span<byte>(arr, 0, len);
                var written = CopyTo(span);
                Debug.Assert(written == len);
                var result = GetHashCode(span);
                ArrayPool<byte>.Shared.Return(arr);
                return result;
            }
        }

        public static implicit operator InputText(string input)
        {
            if (input == null) return default;
            return new InputText(input);
        }

        public static implicit operator string(InputText key)
        {
            var len = key.TotalLength();
            var arr = ArrayPool<byte>.Shared.Rent(len);
            var written = key.CopyTo(arr);
            Debug.Assert(written == len, "length error");
            var result = Get(arr, len);
            ArrayPool<byte>.Shared.Return(arr);
            return result;

            static string Get(byte[] arr, int length)
            {
                if (length == -1) length = arr.Length;
                if (length == 0) return "";

                try
                {
                    return Encoding.UTF8.GetString(arr, 0, length);
                }
                catch
                {
                    return BitConverter.ToString(arr, 0, length);
                }
            }
        }

        internal static InputText Null { get; } = new InputText(null);

        internal static InputText Empty { get; } = new InputText(string.Empty);

        internal bool IsNull => InputValue == null;
        internal bool IsEmpty => string.IsNullOrWhiteSpace(InputValue);

        internal string InputValue { get; }
        private bool EqualsImpl(in InputText other)
        {
            if (IsNull)
            {
                return other.IsNull;
            }
            else if (other.IsNull)
            {
                return false;
            }
            if (IsEmpty)
            {
                return other.IsEmpty;
            }
            else if (other.IsEmpty)
            {
                return false;
            }

            if (InputValue is string valueString1 && other.InputValue is string valueString2)
                return valueString1 == valueString2;

            return false;
        }

        internal int TotalLength() =>
            InputValue switch
            {
                null => 0,
                string s when IsEmpty => Encoding.UTF8.GetByteCount(s),
                _ => 0,
            };

        internal int CopyTo(Span<byte> destination)
        {
            int written = 0;

            switch (InputValue)
            {
                case null:
                    break; // nothing to do
                case string s:
                    if (s.Length != 0)
                    {
#if NETCOREAPP
                        written += Encoding.UTF8.GetBytes(s, destination);
#else
                        unsafe
                        {
                            fixed (byte* bPtr = destination)
                            fixed (char* cPtr = s)
                            {
                                written += Encoding.UTF8.GetBytes(cPtr, s.Length, bPtr, destination.Length);
                            }
                        }
#endif
                    }

                    break;
                default:
                    break;
            }

            return written;
        }

        internal static int GetHashCode(ReadOnlySpan<byte> span)
        {
            unchecked
            {
                int len = span.Length;
                if (len == 0) return 0;

                int acc = 728271210;

                var span64 = MemoryMarshal.Cast<byte, long>(span);

                for (int i = 0; i < span64.Length; i++)
                {
                    var val = span64[i];
                    int valHash = (int)val ^ (int)(val >> 32);
                    acc = (acc << 5) + acc ^ valHash;
                }

                int spare = len % 8, offset = len - spare;
                while (spare-- != 0)
                {
                    acc = (acc << 5) + acc ^ span[offset++];
                }

                return acc;
            }
        }
    }
}