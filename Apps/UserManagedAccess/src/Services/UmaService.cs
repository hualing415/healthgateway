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
namespace HealthGateway.UserManagedAccess.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>The UmaService class.</summary>
    public class UmaService : IUmaService
    {
        private ILogger<UmaService> logger;

        /// <summary>Initializes a new instance of the <see cref="UmaService"/> class.</summary>
        /// <param name="logger">The injected logger.</param>
        public UmaService(ILogger<UmaService> logger)
        {
            this.logger = logger;
        }

        /// <summary>Submit a request to share a resource.</summary>
        /// <param name="token">An access token.</param>
        /// <returns>Returns a true if ok response.</returns>
        public Task<bool> CreateSharable(string token)
        {
            // Call Delegates here
            this.logger.LogDebug($"{token}");
            return Task.FromResult<bool>(true);
        }
    }
}