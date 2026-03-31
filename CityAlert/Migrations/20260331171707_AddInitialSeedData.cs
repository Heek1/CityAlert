using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityAlert.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DistrictId", "EndDate", "IsActive", "Severity", "StartDate", "Title" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2025, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), "system_bot", "У зв'язку з аварією на вул. Галицькій, водопостачання буде відсутнє до 20:00. Працює аварійна бригада.", 1, new DateTime(2025, 1, 11, 20, 0, 0, 0, DateTimeKind.Unspecified), true, 2, new DateTime(2025, 1, 10, 8, 30, 0, 0, DateTimeKind.Unspecified), "Прорив магістрального водопроводу" },
                    { 2, 3, new DateTime(2025, 1, 11, 12, 0, 0, 0, DateTimeKind.Unspecified), "admin_moderator", "Через проведення дорожніх робіт автобус тимчасово курсуватиме через вул. Тролейбусну.", 2, new DateTime(2025, 1, 15, 23, 0, 0, 0, DateTimeKind.Unspecified), true, 1, new DateTime(2025, 1, 12, 6, 0, 0, 0, DateTimeKind.Unspecified), "Зміна маршруту автобуса №22" },
                    { 3, 0, new DateTime(2025, 1, 12, 9, 0, 0, 0, DateTimeKind.Unspecified), "energy_service", "Ремонтні роботи на підстанції. Світла не буде у приватному секторі.", 3, new DateTime(2025, 1, 13, 17, 0, 0, 0, DateTimeKind.Unspecified), true, 0, new DateTime(2025, 1, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), "Планове відключення світла" },
                    { 4, 1, new DateTime(2025, 1, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), "local_community", "Запрошуємо всіх мешканців на ярмарок у підтримку ЗСУ.", 5, new DateTime(2025, 1, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 0, new DateTime(2025, 1, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), "Благодійний ярмарок" },
                    { 5, 3, new DateTime(2025, 1, 14, 15, 0, 0, 0, DateTimeKind.Unspecified), "traffic_police", "Аварія біля ТЦ. Рух ускладнено.", 4, new DateTime(2025, 1, 14, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 1, new DateTime(2025, 1, 14, 15, 10, 0, 0, DateTimeKind.Unspecified), "ДТП на перехресті" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
