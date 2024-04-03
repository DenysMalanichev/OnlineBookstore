using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Context;

public class DbInitializer
{
    public static void SeedData(ModelBuilder builder)
    {
        SeedGenres(builder);
    }

    private static void SeedGenres(ModelBuilder builder)
    {
        builder.Entity<Genre>().HasData(
            new Genre
            {
                Id = 1,
                Name = "Fantasy",
                Description = "A genre that features magical and supernatural elements that do not exist in the real" +
                              " world. Often set in imaginary worlds with enchanted creatures, wizards, and quests.",
            },
            new Genre
            {
                Id = 2,
                Name = "Science Fiction",
                Description = "Explores futuristic, imaginative, or technological advances and their impact on society " +
                              "or individuals, often set in space, on other planets, or in different dimensions.",
            },
            new Genre
            {
                Id = 3,
                Name = "Mystery",
                Description = "Involves suspenseful stories centered around a puzzling event, situation, or crime " +
                              "that is to be solved, often featuring a detective or amateur sleuth.",
            },
            new Genre
            {
                Id = 4,
                Name = "Romance",
                Description = "Focuses on relationships and love stories, ranging from contemporary settings to " +
                              "historical backdrops, with a primary emphasis on the emotional journey of the characters towards a romantic relationship.",
            },
            new Genre
            {
                Id = 5,
                Name = "Horror",
                Description = "Aims to evoke fear, dread, or horror in its readers. These stories often involve " +
                              "supernatural elements, monsters, or psychological thrills.",
            },
            new Genre
            {
                Id = 6,
                Name = "Self-Help",
                Description = "Designed to assist readers in improving aspects of their personal lives. These books " +
                              "often provide advice on personal growth, relationships, and professional success.",
            },
            new Genre
            {
                Id = 7,
                Name = "Historical Fiction",
                Description = "Set in the past, often during significant historical events. These novels can include " +
                              "real historical figures and locations but are enriched with fictionalized plots and characters.",
            },
            new Genre
            {
                Id = 8,
                Name = "Thriller",
                Description = "Characterized by fast pacing, tension, and excitement. Thrillers often involve a race " +
                              "against time, with heroes facing off against villains in high-stakes situations.",
            },
            new Genre
            {
                Id = 9,
                Name = "Biography/Autobiography",
                Description = "Narratives of a person's life. A biography is written by someone other than the subject," +
                              "while an autobiography is written by the subject themselves.",
            },
            new Genre
            {
                Id = 10,
                Name = "Young Adult (YA)",
                Description = "Targeted towards young adults, these stories often deal with themes and issues relevant " +
                              "to teenagers, ranging from first loves to facing personal or societal challenges.",
            }
        );
    }
}