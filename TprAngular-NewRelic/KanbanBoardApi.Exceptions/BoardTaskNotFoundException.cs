using System;
using System.Runtime.Serialization;

namespace KanbanBoardApi.Exceptions
{
    [Serializable]
    public class BoardTaskNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BoardTaskNotFoundException()
        {
        }

        public BoardTaskNotFoundException(string message) : base(message)
        {
        }

        public BoardTaskNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BoardTaskNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}