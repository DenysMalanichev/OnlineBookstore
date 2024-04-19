import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { BooksFilterComponent } from './components/books-filter/books-filter.component';
import { BookFullInfoComponent } from './components/book-full-info/book-full-info.component';
import { GenresListComponent } from './components/genres-list/genres-list.component';
import { AuthorsListComponent } from './components/authors-list/authors-list.component';
import { PublishersListComponent } from './components/publishers-list/publishers-list.component';
import { AuthorDetailsComponent } from './components/author-details/author-details.component';
import { PublisherDetailsComponent } from './components/publisher-details/publisher-details.component';
import { LoginComponent } from './components/auth/login/login.component';
import { AccountComponent } from './components/auth/account/account.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { BusketComponent } from './components/basket/basket.component';
import { MakeOrderComponent } from './components/make-order/make-order.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: 'books-filters',
    component: BooksFilterComponent
  },
  {
    path: 'book-details/:id',
    component: BookFullInfoComponent
  },
  {
    path: 'genres',
    component: GenresListComponent
  },
  {
    path: 'authors',
    component: AuthorsListComponent
  },
  {
    path: 'author/:id',
    component: AuthorDetailsComponent
  },
  {
    path: 'publishers',
    component: PublishersListComponent
  },
  {
    path: 'publisher/:id',
    component: PublisherDetailsComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'account',
    component: AccountComponent
  },
  {
    path: 'busket',
    component: BusketComponent
  },
  {
    path: '**',
    component: PageNotFoundComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
