﻿//-------------------------------------------------------------------------
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
// <auto-generated />
#pragma warning disable CS1591
using Microsoft.EntityFrameworkCore.Migrations;

namespace HealthGateway.Database.Migrations
{
    public partial class RenameEmailInviteDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvite_Email_EmailId",
                schema: "gateway",
                table: "EmailInvite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailInvite",
                schema: "gateway",
                table: "EmailInvite");

            migrationBuilder.RenameTable(
                name: "EmailInvite",
                schema: "gateway",
                newName: "MessagingVerification",
                newSchema: "gateway");

            migrationBuilder.RenameIndex(
                name: "IX_EmailInvite_EmailId",
                schema: "gateway",
                table: "MessagingVerification",
                newName: "IX_MessagingVerification_EmailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagingVerification",
                schema: "gateway",
                table: "MessagingVerification",
                column: "EmailInviteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessagingVerification_Email_EmailId",
                schema: "gateway",
                table: "MessagingVerification",
                column: "EmailId",
                principalSchema: "gateway",
                principalTable: "Email",
                principalColumn: "EmailId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessagingVerification_Email_EmailId",
                schema: "gateway",
                table: "MessagingVerification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagingVerification",
                schema: "gateway",
                table: "MessagingVerification");

            migrationBuilder.RenameTable(
                name: "MessagingVerification",
                schema: "gateway",
                newName: "EmailInvite",
                newSchema: "gateway");

            migrationBuilder.RenameIndex(
                name: "IX_MessagingVerification_EmailId",
                schema: "gateway",
                table: "EmailInvite",
                newName: "IX_EmailInvite_EmailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailInvite",
                schema: "gateway",
                table: "EmailInvite",
                column: "EmailInviteId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvite_Email_EmailId",
                schema: "gateway",
                table: "EmailInvite",
                column: "EmailId",
                principalSchema: "gateway",
                principalTable: "Email",
                principalColumn: "EmailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}