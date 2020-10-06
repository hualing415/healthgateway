//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace HealthGateway.Common.AccessManagement.Authorization.Keycloak.Client.Resource
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HealthGateway.Common.AccessManagement.Authorization.Keycloak.Representation;

    ///
    /// <summary>An entry point for managing permission tickets using the Protection API.</summary>
    ///
    public interface IProtectedResource
    {
        /// <summary>
        /// Creates a new Resource on the authorization server. See <see cref="Resource"/> class.
        /// </summary>
        /// <param name="resource">The Resource data.</param>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns>The Resource created.</returns>
        public Task<Resource> create(Resource resource, string token);

        /// <summary>
        /// Updates an existing Resource on the authorization server. See <see cref="Resource"/> class.
        /// </summary>
        /// <param name="resource">The Resource to be updated.</param>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns>True when updated.</returns>
        public Task<bool> update(Resource resource, string token);

        /// <summary>Deletes an existing user-managed Resource from the server.</summary>
        /// <param name="resourceId">The Resource identifier.</param>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns>True if the delete was successful.</returns>
        public Task<bool> delete(string resourceId, string token);

        /// <summary>
        /// Query the server for a resource given its id.
        /// </summary>
        /// <param name="resourceId">The Resource  ID to be found.</param>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns>The Resource found.</returns>
        public Task<Resource> findById(string resourceId, string token);

        /// <summary>
        /// Query the server for a Resource with a given Uri.
        /// This method queries the server for resources whose
        /// </summary>
        /// <param name="uri">The string url to be found.</param>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns>Returns a list of Resources that best matches the given Uri.</returns>
        public Task<List<Resource>> findByUri(string uri, string token);

        /// <summary>
        /// Query the server for a Resource with a given Uri.
        /// This method queries the server for resources whose
        /// </summary>
        /// <param name="uri">The string url to be found.</param>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns>Returns a list of Resources that best matches the given Uri.</returns>
        public Task<List<Resource>> findByMatchingUri(string uri, string token);

        /// <summary>Query the server for all resources.</summary>
        /// <param name="token"> A valid base64 access_token from authenticing the caller.</param>
        /// <returns> @return an array of strings with the resource ids</returns>
        public Task<string[]> findAll(string token);

    }
}