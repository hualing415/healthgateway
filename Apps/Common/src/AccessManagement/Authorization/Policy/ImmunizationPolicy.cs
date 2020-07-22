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
namespace HealthGateway.Common.AccessManagement.Authorization.Policy
{
    /// <summary>
    /// The set of claims to access Immunization data.
    /// </summary>
    public static class ImmunizationPolicy
    {
        /// <summary>
        /// Policy which allows the reading of the identified Immunization.
        /// </summary>
        public const string Read = "ImmunizationRead";

        /// <summary>
        /// Policy which allows writing of the identified Immunization.
        /// </summary>
        public const string Write = "ImmunizationWrite";
    }
}