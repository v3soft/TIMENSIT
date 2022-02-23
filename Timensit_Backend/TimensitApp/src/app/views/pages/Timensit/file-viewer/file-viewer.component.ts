import { Component, OnInit, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-file-viewer',
	templateUrl: './file-viewer.component.html',
})
@Injectable()
export class FileViewerComponent implements OnInit {

  constructor() {}

  ngOnInit() {
  }
}
