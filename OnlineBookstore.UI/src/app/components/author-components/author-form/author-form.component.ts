import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthorModel } from 'src/app/models/author-models/authorModel';
import { CreateAuthorModel } from 'src/app/models/author-models/createAuthorModel';
import { AuthorService } from 'src/app/services/author-service.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'author-form',
  templateUrl: './author-form.component.html',
  styleUrls: ['./author-form.component.css']
})
export class AuthorFormComponent implements OnInit {
  @Input()
  author?: AuthorModel;

  authorForm!: FormGroup;

  constructor(
    private authorService: AuthorService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authorForm = new FormGroup({
      firstName: new FormControl(this.author?.firstName || '', Validators.required),
      lastName: new FormControl(this.author?.lastName || '', Validators.required),
      email: new FormControl(this.author?.email || '', Validators.required)
    });
  }

  updateAuthorData(): void {
    if(this.authorForm.invalid || !this.author) {
      this.throwErrorAlert();
      return;
    }

    this.authorService.updateAuthor(this.updateAuthorModel(this.authorForm.value))
    .subscribe({
      next: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "success",
          title: "Updated successfully",
          showConfirmButton: false,
          timer: 2500
        });
        this.router.navigate(['/authors']);
        return;
      },
      error: () => {
        this.throwErrorAlert();
      }
    });
  }

  createAuthor(): void {
    if(this.authorForm.invalid || this.author) {
      this.throwErrorAlert();
      return;
    }

    this.authorService.createAuthor(this.createAuthorModel(this.authorForm.value))
    .subscribe({
      next: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "success",
          title: "Created successfully",
          showConfirmButton: false,
          timer: 2500
        });
        location.reload();
        return;
      },
      error: () => {
        this.throwErrorAlert();
      }
    });
  }

  private throwErrorAlert(): void {
    Swal.fire({
      position: "bottom-end",
      icon: "error",
      title: "Error. Enter valid data",
      showConfirmButton: false,
      timer: 2500
    });
  }

  private updateAuthorModel(formData: any): AuthorModel {
    return {
      id: this.author!.id,
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email
    };
  }

  private createAuthorModel(formData: any): CreateAuthorModel {
    return {
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email
    };
  }
}
