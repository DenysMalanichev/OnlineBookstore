using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineBookstore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedGenresMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "A genre that features magical and supernatural elements that do not exist in the real world. Often set in imaginary worlds with enchanted creatures, wizards, and quests.", "Fantasy" },
                    { 2, "Explores futuristic, imaginative, or technological advances and their impact on society or individuals, often set in space, on other planets, or in different dimensions.", "Science Fiction" },
                    { 3, "Involves suspenseful stories centered around a puzzling event, situation, or crime that is to be solved, often featuring a detective or amateur sleuth.", "Mystery" },
                    { 4, "Focuses on relationships and love stories, ranging from contemporary settings to historical backdrops, with a primary emphasis on the emotional journey of the characters towards a romantic relationship.", "Romance" },
                    { 5, "Aims to evoke fear, dread, or horror in its readers. These stories often involve supernatural elements, monsters, or psychological thrills.", "Horror" },
                    { 6, "Designed to assist readers in improving aspects of their personal lives. These books often provide advice on personal growth, relationships, and professional success.", "Self-Help" },
                    { 7, "Set in the past, often during significant historical events. These novels can include real historical figures and locations but are enriched with fictionalized plots and characters.", "Historical Fiction" },
                    { 8, "Characterized by fast pacing, tension, and excitement. Thrillers often involve a race against time, with heroes facing off against villains in high-stakes situations.", "Thriller" },
                    { 9, "Narratives of a person's life. A biography is written by someone other than the subject,while an autobiography is written by the subject themselves.", "Biography/Autobiography" },
                    { 10, "Targeted towards young adults, these stories often deal with themes and issues relevant to teenagers, ranging from first loves to facing personal or societal challenges.", "Young Adult (YA)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
