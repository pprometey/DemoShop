using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DemoShop.UI.Migrations
{
    public partial class SalesDateErrorFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "SalesInvoices",
                newName: "SalesDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalesDate",
                table: "SalesInvoices",
                newName: "PurchaseDate");
        }
    }
}
