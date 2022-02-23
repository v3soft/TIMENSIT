import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
	selector: 'm-receipts',
	templateUrl: './receipts.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class receiptsComponent implements OnInit {
	constructor() {}

	ngOnInit() {}
}
