using Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(ProjectContext))]
    [Migration("20260716090000_SeedPlatesAndAddDistanceRange")]
    public partial class SeedPlatesAndAddDistanceRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistanceRange",
                table: "PumpOperations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.Sql(
                """
                SET IDENTITY_INSERT Plates ON;

                MERGE INTO Plates AS target
                USING (VALUES
                    (1, N'II210NI', N'52მ'),
                    (2, N'PB107PB', N'42მ'),
                    (3, N'BB685FF', N'42მ'),
                    (4, N'VQ632QV', N'42მ'),
                    (5, N'CC178LC', N'<60მ'),
                    (6, N'WW832GW', N'<60მ'),
                    (7, N'OV247VO', N'47მ'),
                    (8, N'CG368CG', N'48მ'),
                    (9, N'ᲰᲘᲓ. ᲒᲐᲛᲐᲜᲐᲬᲘᲚᲔᲑᲔᲚᲘ', N'18მ')
                ) AS source (Id, Number, Type)
                ON target.Id = source.Id
                WHEN MATCHED THEN
                    UPDATE SET Number = source.Number, [Type] = source.Type
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT (Id, Number, [Type])
                    VALUES (source.Id, source.Number, source.Type);

                SET IDENTITY_INSERT Plates OFF;

                DELETE checker
                FROM PlateCheckerEmails AS checker
                WHERE checker.PlateId BETWEEN 1 AND 9
                  AND LOWER(LTRIM(RTRIM(checker.Email))) <> 'gaga.lortkipanidze@cement.ge';

                INSERT INTO PlateCheckerEmails (PlateId, Email)
                SELECT plate.Id, 'gaga.lortkipanidze@cement.ge'
                FROM Plates AS plate
                WHERE plate.Id BETWEEN 1 AND 9
                  AND NOT EXISTS (
                      SELECT 1
                      FROM PlateCheckerEmails AS checker
                      WHERE checker.PlateId = plate.Id
                        AND LOWER(LTRIM(RTRIM(checker.Email))) = 'gaga.lortkipanidze@cement.ge'
                  );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM PlateCheckerEmails
                WHERE PlateId BETWEEN 1 AND 9
                  AND LOWER(LTRIM(RTRIM(Email))) = 'gaga.lortkipanidze@cement.ge';

                DELETE FROM Plates WHERE Id IN (8, 9);

                UPDATE Plates
                SET [Type] = ''
                WHERE Id BETWEEN 1 AND 7;
                """);

            migrationBuilder.DropColumn(
                name: "DistanceRange",
                table: "PumpOperations");
        }
    }
}
