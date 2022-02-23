import { Component, OnInit, Injectable } from '@angular/core';
import { ConfigService } from './config-service/config.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-config',
    templateUrl: './config.component.html',
})
@Injectable()
export class ConfigComponent implements OnInit {

  constructor(
		private configService : ConfigService
	) {}

  ngOnInit() {
    if (this.configService != undefined)
		this.configService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'Priority', 0, 10));
  }
}
