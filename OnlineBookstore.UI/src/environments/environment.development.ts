export const environment = {
    apiBaseUrl: 'https://localhost:44317/api/',
    endpoints: {
        orders: {
            ordersBasePath: 'orders/',
            addDetailToOrderPath: 'add-order-detail',
            getUserOpenOrderPath: 'users-active-order',
            shipUserOrderPath: 'ship-users-order',
            getUserHistoryPath: 'user-orders-history',
            getBooksStatisticsPath: 'get-books-order-statistics/'
        },
        users: {
            usersBasePath: 'users/',
            loginUserPath: 'login/',
            registerUserPath: 'register-user/',
            registerAdminPAth: 'register-admin/',
            isAdminCheckPath: 'is-admin'
        },
        author: {
            authorBasePath: 'author/'
        },
        comments: {
            commentsBasePath: 'comments/',
            getCommentsByBookPath: 'comments-by-book/',
            deleteCommentById: 'comments/'
        },
        books: {
            booksBasePath: 'books/',
            recommendedBooks: 'recommendations',
            getFilteredBooks: 'get-filtered-books/',
            getBookByAuthor: 'by-author/',
            getBookByPublisher: 'by-publisher/',
            getBooksAvgRating: 'avg-rating/',
            bookImage: 'image'
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
