using System;
using System.Runtime.Serialization;

namespace KanbanBoardApi.Exceptions
{
    [Serializable]
    public class BoardNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BoardNotFoundException()
        {
        }

        public BoardNotFoundException(string message) : base(message)
        {
        }

        public BoardNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BoardNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}