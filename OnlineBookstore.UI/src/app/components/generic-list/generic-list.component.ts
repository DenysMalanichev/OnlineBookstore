import { Component, Input } from '@angular/core';

@Component({
  selector: 'generic-list',
  templateUrl: './generic-list.component.html',
  styleUrls: ['./generic-list.component.css']
})
export class GenericListComponent {
  @Input()
  title!: string;
}
