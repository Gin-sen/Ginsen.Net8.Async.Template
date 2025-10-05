using System.Collections.Generic;

namespace Ginsen.Net8.Async.Milestone.HttpClientGenerator.TypeScript
{
    public partial class ApiClient : ITextTemplate
    {
        /// <summary>
        /// Dtos.
        /// </summary>
        public List<RecordInfo> Dtos { get; }


        /// <summary>
        /// Endpoints.
        /// </summary>
        public List<EndpointInfo> Endpoints { get; }

        /// <summary>
        /// Creates an Instance of ApiClient.
        /// </summary>
        /// <param name="dtos">Dtos.</param>
        /// <param name="endpoints">Endpoints.</param>
        public ApiClient(List<RecordInfo> dtos, List<EndpointInfo> endpoints)
            => (Dtos, Endpoints) = (dtos, endpoints);

    }
}