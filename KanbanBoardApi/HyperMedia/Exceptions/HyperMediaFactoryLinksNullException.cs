using System;
using System.Runtime.Serialization;

namespace KanbanBoardApi.HyperMedia.Exceptions
{
    [Serializable]
    public class HyperMediaFactoryLinksNullException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public HyperMediaFactoryLinksNullException()
        {
        }

        public HyperMediaFactoryLinksNullException(string message) : base(message)
        {
        }

        public HyperMediaFactoryLinksNullException(string message, Exception inner) : base(message, inner)
        {
        }

        protected HyperMediaFactoryLinksNullException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}