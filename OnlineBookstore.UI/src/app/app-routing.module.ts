import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { BooksFilterComponent } from './components/books-filter/books-filter.component';

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
    path: 'genrs',
    component: BooksFilterComponent
  },
  {
    path: 'authors',
    component: BooksFilterComponent
  },
  {
    path: 'publishers',
    component: BooksFilterComponent
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
