using System;
using System.IO;
using System.Runtime.Serialization;

namespace DotLiquid.ViewEngine.Exceptions
{
    [Serializable]
    public class SaasCompileException : Exception
    {
        protected SaasCompileException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            SassLine = info.GetString("SassLine");
        }

        public override string Message
        {
            get
            {
                return base.Message + "\n\r" + this.ToString();
            }
        }

        public string SassLine
        {
            get;
            private set;
        }

        public override string ToString()
        {

            return String.Format("Line: {0}\n\rCompiler error: {1}", SassLine, _innerException != null ? _innerException.ToString() : "");
        }

        private Exception _innerException;

        public SaasCompileException(string filename, string sass, Exception innerException) : base("Failed to compile sass file \"" + filename + "\"")
        {
            _innerException = innerException;
            if (innerException.Message.StartsWith("stdin"))
            {
                var lineNumber = Int32.Parse(innerException.Message.Split(':')[1]);
                this.SassLine = ReadLine(sass, lineNumber);
            }
        }

        private static string ReadLine(string text, int lineNumber)
        {
            var reader = new StringReader(text);

            string line;
            int currentLineNumber = 0;

            do
            {
                currentLineNumber += 1;
                line = reader.ReadLine();
            }
            while (line != null && currentLineNumber < lineNumber);

            return (currentLineNumber == lineNumber) ? line : string.Empty;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("SassLine", SassLine);
        }
    }


}
