﻿// -------------------------------------------------------------------------
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
namespace Healthgateway.JobScheduler.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents the LegalAgreement Configuration.
    /// </summary>
    public class LegalAgreementConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LegalAgreementConfig"/> class.
        /// </summary>
        public LegalAgreementConfig()
        {
        }

        /// <summary>
        /// Gets or sets the Legal Document name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Legal Agreement code.
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Gets or sets the application setting key to look up the last processed date.
        /// </summary>
        public string LastCheckedKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email template to use for notifications.
        /// </summary>
        public string EmailTemplate { get; set; } = null!;

        /// <summary>
        /// Gets or sets the location to review the agreement relative to the host.
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets the contact email for the agreement.
        /// </summary>
        public string ContactEmail { get; set; } = null!;
    }
}
