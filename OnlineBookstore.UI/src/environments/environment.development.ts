export const environment = {
    apiBaseUrl: 'https://localhost:44317/api/',
    endpoints: {
        users: {
            usersBasePath: 'users/',
            loginUserPath: 'login/',
            registerUserPath: 'register-user/',
            registerAdminPAth: 'register-admin/'
        },
        author: {
            authorBasePath: 'author/'
        },
        comments: {
            commentsBasePath: 'comments/',
            getCommentsByBookPath: 'comments-by-book/'
        },
        books: {
            booksBasePath: 'books/',
            getFilteredBooks: 'get-filtered-books/',
            getBookByAuthor: 'by-author/',
            getBookByPublisher: 'by-publisher/',
            getBooksAvgRating: 'avg-rating/'
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
