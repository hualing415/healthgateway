// -------------------------------------------------------------------------
//  Copyright © 2019 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------
namespace HealthGateway.WebClient.Services
{
    using System;
    using HealthGateway.Common.Constants;
    using HealthGateway.Common.Delegates;
    using HealthGateway.Common.ErrorHandling;
    using HealthGateway.Common.Models;
    using HealthGateway.Database.Constants;
    using HealthGateway.Database.Delegates;
    using HealthGateway.Database.Models;
    using HealthGateway.Database.Wrapper;
    using HealthGateway.WebClient.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    public class DependentService : IDependentService
    {
        private readonly ILogger logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPatientDelegate patientDelegate;
        private readonly IDependentDelegate dependentDelegate;
        private readonly IConfigurationService configurationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependentService"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="httpAccessor">The injected http context accessor provider.</param>
        /// <param name="patientDelegate">The injected patient registry provider.</param>
        /// <param name="dependentDelegate">The dedendent delegate to interact with the DB.</param>
        /// <param name="configuration">The configuration service.</param>
        public DependentService(
            ILogger<DependentService> logger,
            IHttpContextAccessor httpAccessor,
            IPatientDelegate patientDelegate,
            IDependentDelegate dependentDelegate,
            IConfigurationService configuration)
        {
            this.logger = logger;
            this.httpContextAccessor = httpAccessor;
            this.patientDelegate = patientDelegate;
            this.dependentDelegate = dependentDelegate;
            this.configurationService = configuration;
        }

        /// <inheritdoc />
        public RequestResult<DependentModel> CreateDependent(DependentModel dependentModel)
        {
            this.logger.LogDebug("Getting dependent hdid...");
            string jwtString = this.httpContextAccessor.HttpContext.Request.Headers["Authorization"][0];

            // (1) Retrieve the hdid
            dependentModel.HdId = this.patientDelegate.GetPatientHdId(dependentModel.PHN, jwtString).ResourcePayload!;
            if (string.IsNullOrEmpty(dependentModel.HdId))
            {
                return new RequestResult<DependentModel>()
                {
                    ResultStatus = ResultType.Error,
                    ResultError = new RequestResultError() { ResultMessage = "Communication Exception when trying to retrieve the HdId", ErrorCode = ErrorTranslator.ServiceError(ErrorType.CommunicationExternal, ServiceType.Patient) },
                };
            }

            this.logger.LogTrace($"Dependent hdid: {dependentModel.HdId}");
            this.logger.LogDebug("Getting dependent details...");
            RequestResult<Patient> patientResult = this.patientDelegate.GetPatient(dependentModel.HdId, jwtString);
            if (patientResult.ResourcePayload == null)
            {
                return new RequestResult<DependentModel>()
                {
                    ResultStatus = ResultType.Error,
                    ResultError = new RequestResultError() { ResultMessage = "Communication Exception when trying to retrieve the Dependent", ErrorCode = ErrorTranslator.ServiceError(ErrorType.CommunicationExternal, ServiceType.Patient) },
                };
            }
            else
            {
                // Verify dependent's details entered by user
                if (!patientResult.ResourcePayload.LastName.Equals(dependentModel.LastName, StringComparison.OrdinalIgnoreCase)
                    || !patientResult.ResourcePayload.FirstName.Equals(dependentModel.FirstName, StringComparison.OrdinalIgnoreCase)
                    || patientResult.ResourcePayload.Birthdate.Year != dependentModel.DateOfBirth.Year
                    || patientResult.ResourcePayload.Birthdate.Month != dependentModel.DateOfBirth.Month
                    || patientResult.ResourcePayload.Birthdate.Day != dependentModel.DateOfBirth.Day
                    // Todo: add the check for Gender.
                    )
                {
                    return new RequestResult<DependentModel>()
                    {
                        ResultStatus = ResultType.Error,
                        ResultError = new RequestResultError() { ResultMessage = "Information of the Dependent enterned don't match.", ErrorCode = ErrorTranslator.ServiceError(ErrorType.InvalidState, ServiceType.Patient) },
                    };
                }
            }

            // (2) Inserts Dependent to database
            Dependent dependent = dependentModel.ToDbModel();

            DBResult<Dependent> dbDependent = this.dependentDelegate.InsertDependent(dependent);
            RequestResult<DependentModel> result = new RequestResult<DependentModel>()
            {
                ResourcePayload = DependentModel.CreateFromDbModel(dbDependent.Payload),
                ResultStatus = dbDependent.Status == DBStatusCode.Created ? ResultType.Success : ResultType.Error,
                ResultError = dbDependent.Status == DBStatusCode.Read ? null : new RequestResultError() { ResultMessage = dbDependent.Message, ErrorCode = ErrorTranslator.ServiceError(ErrorType.CommunicationInternal, ServiceType.Database) },
            };
            return result;
        }
    }
}