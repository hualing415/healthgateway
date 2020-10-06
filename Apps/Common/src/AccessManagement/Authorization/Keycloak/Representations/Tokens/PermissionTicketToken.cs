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
namespace HealthGateway.Common.AccessManagement.Authorization.Keycloak.Representation
{
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>A PermissionTicket Token for UMA 2.0</summary>
    public class PermissionTicketToken : JwtPayload
    {
        /// <summary> Get or sets the Permissions. </summary>
        public List<Permission>? Permissions { get; set; }

        /// <summary> Create new PermissionTicketToken class. </summary>
        /// <param name="permissions">A List of Permission objects.</param>
        public PermissionTicketToken(List<Permission> permissions)
        {
            this.Permissions = permissions;
        }

        /// <summary> Create new PermissionTicketToken class. </summary>
        public PermissionTicketToken()
        {
            this.Permissions = new List<Permission>();
        }
    }
}