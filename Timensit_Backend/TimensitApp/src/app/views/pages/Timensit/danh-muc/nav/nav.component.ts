import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
	selector: 'm-nav',
	templateUrl: './nav.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class navComponent implements OnInit {
	constructor() {}

	ngOnInit() {}
}
