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
namespace HealthGateway.UserManagedAccess.Delegates
{
    /// <summary>The Uma Delegate interface.</summary>
    public interface IUmaDelegate
    {
        /// <summary>Submit a request to share a resource.</summary>
        /// <param name="token">The base65 access token.</param>
        /// <returns>True if created.</returns>
        public bool CreateSharable(string token);
    }
}