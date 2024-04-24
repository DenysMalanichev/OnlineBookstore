import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CreatePublisherModel } from 'src/app/models/publisher-models/createPublisherModel';
import { FullPublisherModel } from 'src/app/models/publisher-models/fullPublisherModel';
import { PublishersService } from 'src/app/services/publishers-service.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'publisher-form',
  templateUrl: './publisher-form.component.html',
  styleUrls: ['./publisher-form.component.css']
})
export class PublisherFormComponent implements OnInit {
  @Input()
  publisher?: FullPublisherModel;
  publisherForm!: FormGroup; 

  constructor(
    private publisherService: PublishersService
  ) {}

  ngOnInit(): void {
    this.publisherForm = new FormGroup({
      companyName: new FormControl(this.publisher?.companyName || '', Validators.required),
      contactName: new FormControl(this.publisher?.contactName || '', Validators.required),
      phone: new FormControl(this.publisher?.phone || ''),
      address: new FormControl(this.publisher?.address || '')
    });
  }

  updatePublisherData(): void {
    if(this.publisherForm.invalid || !this.publisher) {
      this.throwErrorAlert();
      return;
    }

    this.publisherService.updatePublisher(this.updatePublisherModel(this.publisherForm.value))
    .subscribe({
      next: () => {
        Swal.fire({
          position: "bottom-end",
          icon: "success",
          title: "Updated successfully",
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

  createPublisher(): void {
    if(this.publisherForm.invalid || this.publisher) {
      this.throwErrorAlert();
      return;
    }

    this.publisherService.createPublisher(this.createPublisherModel(this.publisherForm.value))
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

  private createPublisherModel(formData: any): CreatePublisherModel {
    return {
      companyName: formData.companyName,
      contactName: formData.contactName,
      phone: formData.phone || '',
      address: formData.address || ''
    };
  }

  private updatePublisherModel(formData: any): FullPublisherModel {
    return {
      id: this.publisher!.id,
      companyName: formData.companyName,
      contactName: formData.contactName,
      phone: formData.phone || '',
      address: formData.address || ''
    };
  }
}
