import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { BooksFilterComponent } from './components/books-filter/books-filter.component';
import { BookFullInfoComponent } from './components/book-full-info/book-full-info.component';
import { GenresListComponent } from './components/genres-list/genres-list.component';
import { AuthorsListComponent } from './components/authors-list/authors-list.component';
import { PublishersListComponent } from './components/publishers-list/publishers-list.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/books-filters',
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
    component: AuthorsListComponent
  },
  {
    path: 'publishers',
    component: PublishersListComponent
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
