using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToSixRoleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Roles SET Name = 'Sales' WHERE Name = 'Sales Manager';");
            migrationBuilder.Sql("UPDATE Roles SET Name = 'Warehouse' WHERE Name = 'Warehouse Clerk';");
            
            migrationBuilder.Sql("UPDATE Roles SET Permissions = '[]';");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Purchasing')
                BEGIN
                    INSERT INTO Roles (Name, Permissions) VALUES ('Purchasing', '[]');
                END
                
                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Finance')
                BEGIN
                    INSERT INTO Roles (Name, Permissions) VALUES ('Finance', '[]');
                END
                
                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'HR')
                BEGIN
                    INSERT INTO Roles (Name, Permissions) VALUES ('HR', '[]');
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
