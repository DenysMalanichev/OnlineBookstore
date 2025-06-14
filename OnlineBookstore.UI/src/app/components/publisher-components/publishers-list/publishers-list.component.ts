import { Component, OnInit } from '@angular/core';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
import { AuthService } from 'src/app/services/auth.service';
import { PublishersService } from 'src/app/services/publishers-service.service';

@Component({
  selector: 'app-publishers-list',
  templateUrl: './publishers-list.component.html',
  styleUrls: ['./publishers-list.component.css']
})
export class PublishersListComponent implements OnInit {  
  publishers!: BriefPublisherModel[];
  isAdd = false;
  isAdmin = false;


  constructor(
    private publishersService: PublishersService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.getPublishers();
    this.isAdminCheck();
  }

  getPublishers(): void {
    this.publishersService.getAllPublishers().subscribe(x => this.publishers = x);
  }

  isAdminCheck(): void {
    this.authService.isAdmin().subscribe(x => this.isAdmin = x);
  }
}
