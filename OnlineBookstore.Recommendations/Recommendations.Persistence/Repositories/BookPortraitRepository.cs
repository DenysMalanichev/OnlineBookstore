using MongoDB.Bson;
using MongoDB.Driver;
using Recommendations.Abstractions.Entities;
using Recommendations.Abstractions.Repositories;

namespace Recommendations.Persistence.Repositories;
public class BookPortraitRepository : IBookPortraitRepository
{
    private readonly IMongoCollection<BookPortrait> _bookPortraits;

    private readonly IReadOnlyDictionary<string, double> weights = new Dictionary<string, double>
        {
            { "language", 0.4 },
            { "genre", 0.2 },
            { "author", 0.1 },
            { "rating", 0.2 },
            { "popularity", 0.05 },
            { "type", 0.05 }
        };

    public BookPortraitRepository(MongoDbContext dbContext)
    {
        _bookPortraits = dbContext.BookPortraits;
    }

    public async Task<IList<int>> GetRecommendedBooksIdsAsync(UserPortrait userPortrait, int pageNumber = 1, int pageSize = 10)
    {
        // Get max popularity value for normalization
        var maxPopularity = _bookPortraits.AsQueryable()
            .Max(b => b.PurchaseNumber);

        // Build the aggregation pipeline using the fluent API
        var pipeline = _bookPortraits.Aggregate()
            // Stage 1: Match books the user hasn't purchased yet
            .Match(book => !userPortrait.PurchasedBooks.Contains(book.BookId))

            // Stage 2: Project to include normalized values
            .AppendStage(new BsonDocumentPipelineStageDefinition<BookPortrait, BsonDocument>(
                new BsonDocument("$project", new BsonDocument
                {
                    { "BookId", 1 },
                    { "Language", 1 },
                    { "GenreIds", 1 },
                    { "AuthorId", 1 },
                    { "Rating", 1 },
                    { "PurchaseNumber", 1 },
                    { "IsPaperback", 1 },
                    { "normalizedValues", new BsonDocument
                        {
                            { "language", new BsonDocument("$cond", new BsonArray
                                {
                                    new BsonDocument("$in", new BsonArray { "$Language", new BsonArray(userPortrait.PreferedLanguages) }),
                                    1,
                                    0
                                })
                            },
                            { "genre", new BsonDocument("$let", new BsonDocument
                                {
                                    { "vars", new BsonDocument
                                        {
                                            { "matchCount", new BsonDocument("$size", new BsonDocument("$setIntersection",
                                                new BsonArray { "$GenreIds", new BsonArray(userPortrait.PreferedGenreIds) }))
                                            }
                                        }
                                    },
                                    { "in", new BsonDocument("$cond", new BsonArray
                                        {
                                            new BsonDocument("$gt", new BsonArray { "$$matchCount", 0 }),
                                            new BsonDocument("$divide", new BsonArray { "$$matchCount", new BsonDocument("$size", "$GenreIds") }),
                                            0
                                        })
                                    }
                                })
                            },
                            { "author", new BsonDocument("$cond", new BsonArray
                                {
                                    new BsonDocument("$in", new BsonArray { "$AuthorId", new BsonArray(userPortrait.PreferedAuthoreIds) }),
                                    1,
                                    0
                                })
                            },
                            { "rating", new BsonDocument("$divide", new BsonArray { "$Rating", 5 }) },
                            { "popularity", new BsonDocument("$divide", new BsonArray { "$PurchaseNumber", maxPopularity }) },
                            { "type", new BsonDocument("$cond", new BsonArray
                                {
                                    new BsonDocument("$eq", new BsonArray { "$IsPaperback", userPortrait.IsPaperbackPrefered }),
                                    1,
                                    0
                                })
                            }
                        }
                    }
                })
            ))

            // Stage 3: Apply weights to normalized values
            .AppendStage(new BsonDocumentPipelineStageDefinition<BsonDocument, BsonDocument>(
                new BsonDocument("$project", new BsonDocument
                {
                    { "BookId", 1 },
                    { "Language", 1 },
                    { "GenreIds", 1 },
                    { "AuthorId", 1 },
                    { "Rating", 1 },
                    { "PurchaseNumber", 1 },
                    { "IsPaperback", 1 },
                    { "normalizedValues", 1 },
                    { "weightedValues", new BsonDocument
                        {
                            { "language", new BsonDocument("$multiply", new BsonArray { "$normalizedValues.language", weights["language"] }) },
                            { "genre", new BsonDocument("$multiply", new BsonArray { "$normalizedValues.genre", weights["genre"] }) },
                            { "author", new BsonDocument("$multiply", new BsonArray { "$normalizedValues.author", weights["author"] }) },
                            { "rating", new BsonDocument("$multiply", new BsonArray { "$normalizedValues.rating", weights["rating"] }) },
                            { "popularity", new BsonDocument("$multiply", new BsonArray { "$normalizedValues.popularity", weights["popularity"] }) },
                            { "type", new BsonDocument("$multiply", new BsonArray { "$normalizedValues.type", weights["type"] }) }
                        }
                    }
                })
            ))

            // Stage 4: Calculate distances to ideal and anti-ideal solutions
            .AppendStage(new BsonDocumentPipelineStageDefinition<BsonDocument, BsonDocument>(
                new BsonDocument("$project", new BsonDocument
                {
                    { "BookId", 1 },
                    { "Language", 1 },
                    { "GenreIds", 1 },
                    { "AuthorId", 1 },
                    { "Rating", 1 },
                    { "PurchaseNumber", 1 },
                    { "IsPaperback", 1 },
                    { "weightedValues", 1 },
                    { "idealDistanceSquared", new BsonDocument("$add", new BsonArray
                        {
                            new BsonDocument("$pow", new BsonArray
                            {
                                new BsonDocument("$subtract", new BsonArray { weights["language"], "$weightedValues.language" }),
                                2
                            }),
                            new BsonDocument("$pow", new BsonArray
                            {
                                new BsonDocument("$subtract", new BsonArray { weights["genre"], "$weightedValues.genre" }),
                                2
                            }),
                            new BsonDocument("$pow", new BsonArray
                            {
                                new BsonDocument("$subtract", new BsonArray { weights["author"], "$weightedValues.author" }),
                                2
                            }),
                            new BsonDocument("$pow", new BsonArray
                            {
                                new BsonDocument("$subtract", new BsonArray { weights["rating"], "$weightedValues.rating" }),
                                2
                            }),
                            new BsonDocument("$pow", new BsonArray
                            {
                                new BsonDocument("$subtract", new BsonArray { weights["popularity"], "$weightedValues.popularity" }),
                                2
                            }),
                            new BsonDocument("$pow", new BsonArray
                            {
                                new BsonDocument("$subtract", new BsonArray { weights["type"], "$weightedValues.type" }),
                                2
                            })
                        })
                    },
                    { "antiIdealDistanceSquared", new BsonDocument("$add", new BsonArray
                        {
                            new BsonDocument("$pow", new BsonArray { "$weightedValues.language", 2 }),
                            new BsonDocument("$pow", new BsonArray { "$weightedValues.genre", 2 }),
                            new BsonDocument("$pow", new BsonArray { "$weightedValues.author", 2 }),
                            new BsonDocument("$pow", new BsonArray { "$weightedValues.rating", 2 }),
                            new BsonDocument("$pow", new BsonArray { "$weightedValues.popularity", 2 }),
                            new BsonDocument("$pow", new BsonArray { "$weightedValues.type", 2 })
                        })
                    }
                })
            ))

            // Stage 5: Calculate final distances and closeness coefficient
            .AppendStage(new BsonDocumentPipelineStageDefinition<BsonDocument, BsonDocument>(
                new BsonDocument("$project", new BsonDocument
                {
                    { "BookId", 1 },
                    { "Language", 1 },
                    { "GenreIds", 1 },
                    { "AuthorId", 1 },
                    { "Rating", 1 },
                    { "PurchaseNumber", 1 },
                    { "IsPaperback", 1 },
                    { "idealDistance", new BsonDocument("$sqrt", "$idealDistanceSquared") },
                    { "antiIdealDistance", new BsonDocument("$sqrt", "$antiIdealDistanceSquared") },
                    { "closenessCoefficient", new BsonDocument("$divide", new BsonArray
                        {
                            new BsonDocument("$sqrt", "$antiIdealDistanceSquared"),
                            new BsonDocument("$add", new BsonArray
                            {
                                new BsonDocument("$sqrt", "$idealDistanceSquared"),
                                new BsonDocument("$sqrt", "$antiIdealDistanceSquared")
                            })
                        })
                    }
                })
            ))

            // Stage 6: Sort by closeness coefficient (descending)
            .AppendStage(new BsonDocumentPipelineStageDefinition<BsonDocument, BsonDocument>(
                new BsonDocument("$sort", new BsonDocument
                {
                    { "closenessCoefficient", -1 }
                })
            ))           

            // Stage 7: Use $facet to handle pagination
            .AppendStage(new BsonDocumentPipelineStageDefinition<BsonDocument, BsonDocument>(
                new BsonDocument("$facet", new BsonDocument
                {
                    { "paginatedResults", new BsonArray
                        {
                            new BsonDocument("$skip", (pageNumber - 1) * pageSize),
                            new BsonDocument("$limit", pageSize),
                            new BsonDocument("$project", new BsonDocument
                            {
                                { "_id", 0 },
                                { "BookId", 1 }
                            })
                        }
                    }
                })
            ));


        // Execute the pipeline and convert results back to BookPortrait objects
        var results = await pipeline
            .As<BsonDocument>()
            .ToListAsync();

        var tet = _bookPortraits.AsQueryable().ToList();

        return results[0]["paginatedResults"].AsBsonArray
            .Select(doc => doc.AsBsonDocument["BookId"].AsInt32)
            .ToList();
    }

    public Task<IList<int>> UpdateNormalizedBooksAsync(BookPortrait userPortrait)
    {
        throw new NotImplementedException();
    }
}
