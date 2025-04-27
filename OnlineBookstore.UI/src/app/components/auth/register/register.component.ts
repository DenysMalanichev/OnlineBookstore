import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterModel } from 'src/app/models/auth-models/registerModel';
import { BriefGenreModel } from 'src/app/models/genre-models/briefGenreModel';
import { AuthService } from 'src/app/services/auth.service';
import { AuthorService } from 'src/app/services/author-service.service';
import { GenresService } from 'src/app/services/genres-service.service';

interface Language {
  name: string,
  code: string
}

interface AuthorDisplayModel {
  name: string,
  id: number
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  registerFormControl = new FormGroup({
    firstName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8)
    ]),
    confirmPassword: new FormControl('', [
      Validators.required
    ]),
    preferedLanguages: new FormControl('',[
      Validators.required
    ]),
    preferedGenres: new FormControl('',[
      Validators.required
    ]),
    preferedAuthores: new FormControl('', []),
    isPaperbackPrefered: new FormControl('false', [])
  });

  languages: Language[] =  [];
  genres: BriefGenreModel[] =  [];
  authors: AuthorDisplayModel[] =  [];
  personalInfoLabel: string = 'Personal Preferences \u24D8';

  constructor(
    private genresService: GenresService,    
    private authorsService: AuthorService,    
    private authService: AuthService,
    private router: Router) {
    this.languages = [
      { name: "English", code: "EN"},
      { name: "Українська", code: "UA"},
      { name: "Español", code: "ES" },
      { name: "Français", code: "FR" },
      { name: "Deutsch", code: "DE" },
      { name: "Italiano", code: "IT" },
    ]
  }
  ngOnInit(): void {
    this.getAllGenres();
    this.getAllAuthors();
  }

  register(): void {
    if(this.registerFormControl.invalid || this.passwordMatchValidator()) {
      this.registerFormControl.markAllAsTouched();
      return;
    }

    const registerModel = this.generateRegisterModel(this.registerFormControl.value);
    this.authService.register(registerModel).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      }
    });
  }

  generateRegisterModel(formValue: any): RegisterModel {
    return {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      password: formValue.password,
      confirmPassword: formValue.confirmPassword,
      preferedLanguages: formValue.preferedLanguages.map((l: Language) => l.code),
      preferedAuthoreIds: formValue.preferedAuthores.map((l: AuthorDisplayModel) => l.id),
      preferedGenreIds: formValue.preferedGenres.map((l: BriefGenreModel) => l.id),
      isPaperbackPrefered: formValue.isPaperbackPrefered
    }
  }

  passwordMatchValidator(): boolean {
    const password = this.registerFormControl?.get('password');
    const confirmPassword = this.registerFormControl?.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return true;
    }
    return false;
  }

  private getAllGenres() {
    this.genresService.getAllGenres()
      .subscribe(x => this.genres = x);
  }

  private getAllAuthors() {
    this.authorsService.getAllAuthors()
      .subscribe(x => this.authors = x.map(a => ({ 
        name: a.firstName + ' ' + a.lastName, 
        id: a.id 
      })));
  }
}
