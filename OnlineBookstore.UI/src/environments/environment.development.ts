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
        books: {
            booksBasePath: 'books/',
            getFilteredBooks: 'get-filtered-books/',
            getBookByAuthor: 'by-author/',
            getBookByPublisher: 'by-publisher/'
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
