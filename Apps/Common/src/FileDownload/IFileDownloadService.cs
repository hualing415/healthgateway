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
namespace HealthGateway.Common.FileDownload
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The file download service.
    /// </summary>
    public interface IFileDownloadService
    {
        /// <summary>
        /// Service to download a file specified by the supplied URL.
        /// </summary>
        /// <parameter>url</parameter>
        /// <parameter>targetFolder</parameter>
        /// <parameter>isRelativePath</parameter>
        /// <returns>The DownloadedFile.</returns>
        Task<DownloadedFile> GetFileFromUrl(Uri url, string targetFolder, bool isRelativePath);
    }
}