  # Online Bookstore
Online Bookstore is a learning project. Its purpose is to create marketplace for books, that users would be able to buy and order shipping right to their homes.
Every order may have more than 1 book and every book may have its own quantity. To order book customer would need to register in this system.
Users can write reviews to books and give marks (1-5 stars).
Admins are able to manage every busines entity.

Online Bookstore is written using the following **technologies**:
### Back-end
- ASP.NET Core
- EF Core
- MS SQL Server
- MongoDB
- xUnit (along with Moq, NBuilder, Bogus, FluentAssertion, InMemory)
- ASP.NET Core Identity for auth
### Front-end
- Angular
- HTML, CSS, TS
- Karma (for future testing)

### Expected scenario of expluatation:
Back-end part constitutes a public API that every one can use.
In our case front-end web-application, written on Angular, 
use this public API to get data from DB and execute business operations.

![layers-diagram](https://github.com/user-attachments/assets/4d7a060b-46ea-448f-99d6-a41b5904c3df)

## Back-end part of the project is written using N-layerd architecture:
Here we have 3 main layers:
- Presentation Layer - that contains API conterollers. Its mission is to orchestrate requests to API and call corresponding Service from Business Logic Layer
- Business Logic Layer - containce all BL and calls Repositories from Data Access Layer to query DB
- Data Access Logic - its Repositories ancapsulates queries to specific resources in DB
![layers-diagram](https://github.com/DenysMalanichev/OnlineBookstore/assets/58270142/57deb170-2078-43ac-a35e-4d94a1ef2659)

### Project structure of Back-end
Business Logic Layer is divided into 3 separate projects clalled: Application, Domain, and Features.
Application projects encapsulates Business Logic (Services).
Domain containce domain models for entities.
Features containce additional classes, like DTOs, exceptions and configurations.

Infrastructire consists of only project called Persistence. It contains Data Context and Migrations used by EF and Repositories.

Finally Presentaions consists of Api project, that contains Program.cs entrypoint, controllers and middleware.
![image](https://github.com/DenysMalanichev/OnlineBookstore/assets/58270142/fea94132-cc7d-4799-910f-67725771f5e5)

## Database schema:
This SQL diagram is simplyfied and aims to show only main idea of DB. It is not including tables provided by ASP.NET Core Identity, like ASP.NET Users, Roles, etc.
![image](https://github.com/user-attachments/assets/0d26cac5-179f-46be-a78c-d097bff7c57b)

## Aplication Features
- Except of CRUD operations for Bussiness Entities (Author, Book, Comment, Genre, Order, Order Detail, Publisher, User) 
user can also add any nu,ber of books to order, specify quantity. Created order can by closed by enering shipping address.
- User can filter on books on multiple criterias, like book name, genre, author name, publisher name or price. This functional is build using Specification pattern.
- Result of filtering books, requesting publishers and authors will be returned in paged format. User can specify number of entites in page.

## Test coverage 
API is tested with unit, integration and end-to-end tests.
xUnit along with Moq, NBuilder, Bogus, FluentAssertion, InMemory libraries used.
![image](https://github.com/DenysMalanichev/OnlineBookstore/assets/58270142/3ac5b38d-0bba-46f9-8a81-376d172fb529)
![image](https://github.com/DenysMalanichev/OnlineBookstore/assets/58270142/7b889f69-4a23-46b1-ae7e-9c5f2713af42)

## API documentation
Create Author
/api/author (POST)
```
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com"
}
```
### Authors
Update Author Data
/api/author (PUT)
```
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "id": 0
}
```

Get Author Data
/api/author/{authorId} (GET)

Get All Authors 
/api/author (GET)

Delete Author
/api/author (DELETE)
```
authorId: 0
```

### Books
Create Book
/api/books (POST)
```
{
  "name": "string",
  "description": "string",
  "price": 2147483647,
  "authorId": 0,
  "publisherId": 0,
  "genreIds": [
    0
  ]
}
```

Update Book Data
/api/books (PUT)
```
{
  "id": 0,
  "name": "string",
  "description": "string",
  "price": 2147483647,
  "authorId": 0,
  "publisherId": 0,
  "genreIds": [
    0
  ]
}
```

Get Book Data
/api/books (GET)
```
bookId: 0
```

Delete Book
/api/books (DELETE)
```
bookId: 0
```

Get Filtered Books
/api/books/get-filtered-books (GET) **all from query**
```
  name: string
  authorName: string
  publisherId: number
  isDescending: boolean
  minPrice: number
  maxPrice: number
  page: number
  itemsOnPage: number
  genres: number[]
```

Get Books By Author
/api/books/by-author (GET) **all from query**
```
  authorId: number
  page: number
  itemsOnPage: number
```

Get Books By Publisher
/api/books/by-publisher (GET) **all from query**
```
  publisherId: number
  page: number
  itemsOnPage: number
```

Get Books Avarage Rating
/api/books/by-publisher/{bookId} (GET)

### Comments
Create Comment
/api/comments (POST)
```
{
  "title": "string",
  "body": "string",
  "bookRating": 5,
  "bookId": 0
}
```

Get Comment
/api/comments (GET) **from query**
```
  commentId: number
```

Get Comments By Book
/api/comments/comments-by-book (GET) **from query**
```
  bookId: number
```

### Genres
Create Genre
/api/genres (POST)
```
{
  "name": "string",
  "description": "string"
}
```

Update Genre
/api/genres (PUT)
```
{
  "id": 0,
  "name": "string",
  "description": "string"
}
```

Get All Genres
/api/genres (GET)

Delete Genre
/api/genres (DELETE) **from query**
```
genreId: number
```
Get Genre
/api/genres/{genreId} (GET)

Get Genres By Book
/api/genres/by-book/{bookId} (GET)

### Orders
Get User Active Order (or automatically create one)
/api/orders/users-active-order (GET)
Takes no params but JWT token in request header

Get Users Orders
/api/orders/users-orders-history (GET)
Takes no params but JWT token in request header

Ship User Order
/api/orders/ship-users-order (POST)
```
{
  "shipAddress": "string",
  "shipCity": "string"
}
```

Add Detail To Order
/api/orders/add-order-detail (POST)
```
{
  "bookId": 0,
  "quantity": 0,
  "orderId": 0
}
```

Get Order Detail
/api/orders/get-order-detail (GET) **from query**
```
orderDetailId: number
```

Update Order Detail
/api/orders/update-order-detail (PUT)
```
{
  "id": 0,
  "bookId": 0,
  "quantity": 0
}
```

Delete Order Detail
/api/orders/delete-order-detail (DELETE) **from query**
```
orderDetailId: number
```

### Publishers
Get Publisher Data
/api/publishers/{publisherId} (GET)

Get All Publishers
/api/publishers (GET)

Create Publisher
/api/publishers (POST)
```
{
  "companyName": "string",
  "contactName": "string",
  "phone": "string",
  "address": "string"
}
```

Update Publisher Data
/api/publishers (PUT)
```
{
  "companyName": "string",
  "contactName": "string",
  "phone": "string",
  "address": "string",
  "id": 0
}
```

Delete Publisher
/api/publishers (DELETE) **from query**
```
publisherId: number
```

### Users
Get User Data
/api/users (GET)
Takes no params but JWT token in request header

Check If User if an Admin
/api/users/is-admin (GET)
Takes no params but JWT token in request header

Register User
/api/users/register-user (POST)
```
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "password": "string",
  "confirmPassword": "string"
}
```

Register Admin
/api/users/register-admin (POST)
```
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "password": "string",
  "confirmPassword": "string"
}
```

Login
/api/users/login (POST)
```
{
  "email": "user@example.com",
  "password": "string"
}
```




