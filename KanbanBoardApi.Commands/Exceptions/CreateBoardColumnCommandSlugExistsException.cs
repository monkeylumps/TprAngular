using System;
using System.Runtime.Serialization;

namespace KanbanBoardApi.Commands.Exceptions
{
    [Serializable]
    public class CreateBoardColumnCommandSlugExistsException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CreateBoardColumnCommandSlugExistsException()
        {
        }

        public CreateBoardColumnCommandSlugExistsException(string message) : base(message)
        {
        }

        public CreateBoardColumnCommandSlugExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CreateBoardColumnCommandSlugExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}