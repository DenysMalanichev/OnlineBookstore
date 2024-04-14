import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { MatGridListModule } from '@angular/material/grid-list';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import {MatListModule} from '@angular/material/list';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BooksListComponent } from './components/books-list/books-list.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { BookCardComponent } from './components/book-card/book-card.component';
import { BooksFilterComponent } from './components/books-filter/books-filter.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderComponent } from './components/header/header.component';
import { BookFullInfoComponent } from './components/book-full-info/book-full-info.component';
import { GenresListComponent } from './components/genres-list/genres-list.component';
import { AuthorsListComponent } from './components/authors-list/authors-list.component';
import { GenericListComponent } from './components/generic-list/generic-list.component';
import { PublishersListComponent } from './components/publishers-list/publishers-list.component';
import { AuthorDetailsComponent } from './components/author-details/author-details.component';
import { PublisherDetailsComponent } from './components/publisher-details/publisher-details.component';


@NgModule({
  declarations: [
    AppComponent,
    BooksListComponent,
    PageNotFoundComponent,
    BookCardComponent,
    BooksFilterComponent,
    HeaderComponent,
    BookFullInfoComponent,
    GenresListComponent,
    AuthorsListComponent,
    GenericListComponent,
    PublishersListComponent,
    AuthorDetailsComponent,
    PublisherDetailsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    MatGridListModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    MatCardModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MatListModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
