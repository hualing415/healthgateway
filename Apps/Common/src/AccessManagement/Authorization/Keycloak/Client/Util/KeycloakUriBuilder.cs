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
namespace HealthGateway.Common.AccessManagement.Authorization.Keycloak.Client.Util
{
    using System;

    using HealthGateway.Common.AccessManagement.Authorization.Keycloak.Client.Configuration;

    /// <summary>Helper class to build a URL from the well-known URL constant templates for Keycloak.</summary>
    public static class KeycloakUriBuilder
    {
        private const string REALM_NAME = "{realm-name}";
        private static string build(IKeycloakConfiguration configuration, string template)
        {
            string realm = configuration.Realm;
            string url = new string(configuration.AuthServerUrl);
            int idx = url.LastIndexOf("/");
            if (idx.Equals(url.Length - 1))
            {
                url = url.Substring(0, idx);
            }
            url += template;
            url.Replace(REALM_NAME, realm);

            return url;
        }

        /// <summary>Build up the keycloak server url from the template provided and using the configuration settings.</summary>
        /// <param name="configuration">The <cref name="IKeycloakConfiguration"/>.</param>
        /// <param name="template">The template of which '{realm-name}' will be substituted for the configured realm name.</param>
        /// <returns>a Uri for he given template url</returns>
        public static Uri buildUri(IKeycloakConfiguration configuration, string template)
        {
            return new Uri(build(configuration, template));
        }
    }
}