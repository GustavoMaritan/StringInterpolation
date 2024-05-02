using StringInterpolation.Core.Domain;
using StringReplacement.Core.Domain;
using System.Text;

namespace StringInterpolation.Core
{
    public class TextReplacementStream : MemoryStream
    {
        private readonly Stream _responseStream;
        private readonly string _key;
        private readonly string _keyPattern;

        public TextReplacementStream(Stream responseStream, string key, string keyPattern = null)
        {
            _responseStream = responseStream;
            _key = key;
            _keyPattern = keyPattern;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InputText responseBody = Encoding.UTF8.GetString(buffer, offset, count);

            var keys = StorageValues.GetValue(_key);

            var result = TextReplacement.WithSpanToChar(responseBody, keys, _keyPattern);

            var resultBuffer = Encoding.UTF8.GetBytes(result);

            _responseStream.WriteAsync(resultBuffer, 0, resultBuffer.Length);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                _responseStream.Close();
                _responseStream.Dispose();
            }
            catch
            {
                base.Dispose(disposing);
            }
        }
    }
}
