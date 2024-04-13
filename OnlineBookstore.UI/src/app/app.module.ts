import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { MatGridListModule } from '@angular/material/grid-list';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BooksListComponent } from './components/books-list/books-list.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { BookCardComponent } from './components/book-card/book-card.component';
import { BooksFilterComponent } from './components/books-filter/books-filter.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderComponent } from './components/header/header.component';
import { BookFullInfoComponent } from './components/book-full-info/book-full-info.component';


@NgModule({
  declarations: [
    AppComponent,
    BooksListComponent,
    PageNotFoundComponent,
    BookCardComponent,
    BooksFilterComponent,
    HeaderComponent,
    BookFullInfoComponent
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
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
