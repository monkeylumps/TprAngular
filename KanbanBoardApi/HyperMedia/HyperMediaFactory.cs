using System.Collections.Generic;
using System.Linq;
using KanbanBoardApi.Dto;
using KanbanBoardApi.Exceptions;
using KanbanBoardApi.HyperMedia.States;

namespace KanbanBoardApi.HyperMedia
{
    public class HyperMediaFactory : IHyperMediaFactory
    {
        private readonly IEnumerable<IHyperMediaState> hyperMediaStates;

        public HyperMediaFactory(IEnumerable<IHyperMediaState> hyperMediaStates)
        {
            this.hyperMediaStates = hyperMediaStates;
        }

        public void Apply(object obj)
        {
            foreach (var hyperMediaState in hyperMediaStates.Where(hyperMediaState => hyperMediaState.IsAppliable(obj)))
            {
                hyperMediaState.Apply(obj);
            }
        }

        public string GetLink(IHyperMediaItem obj, string linkType)
        {
            if (obj.Links == null)
            {
                throw new HyperMediaFactoryLinksNullException();
            }

            var link = obj.Links.FirstOrDefault(x => x.Rel == linkType);

            if (link == null)
            {
                throw new HyperMediaFactoryLinksNotFoundException();
            }

            return link.Href;
        }
    }
}