﻿//-------------------------------------------------------------------------
// Copyright © 2020 Province of British Columbia
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
// <auto-generated />
#pragma warning disable CS1591
namespace HealthGateway.Database.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class LegalAgreements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LegalAgreementTypeCode",
                schema: "gateway",
                columns: table => new
                {
                    LegalAgreementCode = table.Column<string>(maxLength: 10, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: false),
                    xmin = table.Column<uint>(type: "xid", nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalAgreementTypeCode", x => x.LegalAgreementCode);
                });

            migrationBuilder.CreateTable(
                name: "LegalAgreement",
                schema: "gateway",
                columns: table => new
                {
                    LegalAgreementsId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: false),
                    xmin = table.Column<uint>(type: "xid", nullable: false),
                    LegalAgreementCode = table.Column<string>(maxLength: 10, nullable: false),
                    LegalText = table.Column<string>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalAgreement", x => x.LegalAgreementsId);
                    table.ForeignKey(
                        name: "FK_LegalAgreement_LegalAgreementTypeCode_LegalAgreementCode",
                        column: x => x.LegalAgreementCode,
                        principalSchema: "gateway",
                        principalTable: "LegalAgreementTypeCode",
                        principalColumn: "LegalAgreementCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "gateway",
                table: "LegalAgreementTypeCode",
                columns: new[] { "LegalAgreementCode", "CreatedBy", "CreatedDateTime", "Description", "UpdatedBy", "UpdatedDateTime" },
                values: new object[] { "ToS", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Terms of Service", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                schema: "gateway",
                table: "LegalAgreement",
                columns: new[] { "LegalAgreementsId", "CreatedBy", "CreatedDateTime", "EffectiveDate", "LegalAgreementCode", "LegalText", "UpdatedBy", "UpdatedDateTime" },
                values: new object[] { new Guid("f5acf1de-2f5f-431e-955d-a837d5854182"), "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "ToS", @"<p><strong>HealthGateway Terms of Service</strong></p>
<p>
    Use of this service is governed by the following terms and conditions. Please read these terms and conditions
    carefully, as by using this website you will be deemed to have agreed to them. If you do not agree with these terms
    and conditions, do not use this service.
</p>
<p>
    The Health Gateway provides BC residents with access to their health information empowering patients and their
    families to manage their health care. In accessing your health information through this service, you acknowledge
    that the information within does not represent a comprehensive record of your health care in BC. No personal health
    information will be stored within the Health Gateway application. Each time you login, your health information will
    be fetched from information systems within BC and purged upon logout. If you choose to share your health information
    accessed through the website with a family member or caregiver, you are responsible for all the actions they take
    with respect to the use of your information.
</p>
<p>
    This service is not intended to provide you with medical advice nor replace the care provided by qualified health
    care professionals. If you have questions or concerns about your health, please contact your care provider.
</p>
<p>
    The personal information you provide (Name and Email) will be used for the purpose of connecting your Health Gateway
    account to your BC Services Card account under the authority of section 33(a) of the Freedom of Information and
    Protection of Privacy Act. This will be done through the BC Services Identity Assurance Service. Once your identity
    is verified using your BC Services Card, you will be able to view your health records from various health
    information systems in one place. Health Gateway’s collection of your personal information is under the authority of
    section 26(c) of the Freedom of Information and Protection of Privacy Act.
</p>
<p>
    If you have any questions about our collection or use of personal information, please direct your inquiries to the
    Health Gateway team:
</p>
<p>
    <i
        ><div>Nino Samson</div>
        <div>Product Owner, Health Gateway</div>
        <div>Telephone: 778-974-2712</div>
        <div>Email: nino.samson@gov.bc.ca</div>
    </i>
</p>

<p><strong>Limitation of Liabilities</strong></p>
<p>
    Under no circumstances will the Government of British Columbia be liable to any person or business entity for any
    direct, indirect, special, incidental, consequential, or other damages based on any use of this website or any other
    website to which this site is linked, including, without limitation, any lost profits, business interruption, or
    loss of programs or information, even if the Government of British Columbia has been specifically advised of the
    possibility of such damages.
</p>", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_LegalAgreement_LegalAgreementCode",
                schema: "gateway",
                table: "LegalAgreement",
                column: "LegalAgreementCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegalAgreement",
                schema: "gateway");

            migrationBuilder.DropTable(
                name: "LegalAgreementTypeCode",
                schema: "gateway");
        }
    }
}