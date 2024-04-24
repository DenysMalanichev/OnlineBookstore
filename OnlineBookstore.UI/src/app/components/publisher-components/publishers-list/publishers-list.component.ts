import { Component, OnInit } from '@angular/core';
import { BriefPublisherModel } from 'src/app/models/publisher-models/briefPublisherModel';
import { PublishersService } from 'src/app/services/publishers-service.service';

@Component({
  selector: 'app-publishers-list',
  templateUrl: './publishers-list.component.html',
  styleUrls: ['./publishers-list.component.css']
})
export class PublishersListComponent implements OnInit {  
  publishers!: BriefPublisherModel[];
  isAdd = false;

  constructor(private publishersService: PublishersService) {}

  ngOnInit(): void {
    this.getPublishers();
  }

  getPublishers(): void {
    this.publishersService.getAllPublishers().subscribe(x => this.publishers = x);
  }
}
