using StringInterpolation.Core.Domain;
using StringReplacement.Core.Domain;
using System.Text;

namespace StringInterpolation.Core
{
    public class TextReplacementStream : MemoryStream
    {
        private readonly Stream _responseStream;
        private readonly string _key;

        public TextReplacementStream(Stream responseStream, string key)
        {
            _responseStream = responseStream;
            _key = key;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InputText responseBody = Encoding.UTF8.GetString(buffer, offset, count);

            var keys = StorageValues.GetValue(_key);

            var result = TextReplacement.WithSpanToChar(responseBody, keys);

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
