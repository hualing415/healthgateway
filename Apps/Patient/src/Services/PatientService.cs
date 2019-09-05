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
namespace HealthGateway.Service
{
    using System.Collections.Generic;
    using HealthGateway.Models;

    /// <summary>
    /// The Patient data service.
    /// </summary>
    public class PatientService : IPatientService
    {
        /// <summary>
        /// Gets the patient record.
        /// </summary>
        /// <param name="hdid">The patient id.</param>
        /// <returns>The patient model.</returns>
        public Patient GetPatient(string hdid)
        {
            return new Patient(hdid, string.Empty, string.Empty, string.Empty);
        }
    }
}