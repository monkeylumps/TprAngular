using System;
using System.Runtime.Serialization;

namespace KanbanBoardApi.Exceptions
{
    [Serializable]
    public class BoardColumnNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public BoardColumnNotFoundException()
        {
        }

        public BoardColumnNotFoundException(string message) : base(message)
        {
        }

        public BoardColumnNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BoardColumnNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}