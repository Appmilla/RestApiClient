using System.Collections.Generic;
using Appmilla.RestApiClient.Interfaces;

namespace Appmilla.RestApiClient
{
    public class PostResponseModel : IEntity
    {
        public IList<string> Errors { get; set; }
    }
}
