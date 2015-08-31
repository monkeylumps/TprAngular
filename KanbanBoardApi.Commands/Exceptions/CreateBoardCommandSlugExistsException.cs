using System;
using System.Runtime.Serialization;

namespace KanbanBoardApi.Commands.Exceptions
{
    [Serializable]
    public class CreateBoardCommandSlugExistsException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CreateBoardCommandSlugExistsException()
        {
        }

        public CreateBoardCommandSlugExistsException(string message) : base(message)
        {
        }

        public CreateBoardCommandSlugExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CreateBoardCommandSlugExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}