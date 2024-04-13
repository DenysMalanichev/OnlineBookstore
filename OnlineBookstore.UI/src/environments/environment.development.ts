export const environment = {
    apiBaseUrl: 'https://localhost:44317/api/',
    endpoints: {
        author: {
            authorBasePath: 'author/'
        },
        books: {
            booksBasePath: 'books/',
            getFilteredBooks: 'get-filtered-books/',
            getBookByAuthor: 'by-author/'
        },        
        genres: {
            genresBasePath: 'genres/',
            genresByBook: 'by-book/',
        },
        publishers: {
            publishersBasePath: 'publishers/'
        }
    }
};
