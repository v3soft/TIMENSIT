import { Component, ChangeDetectionStrategy, Input } from '@angular/core';
import { TableService } from '../table.service';

@Component({
	selector: 'kt-column-option',
	templateUrl: './column-option.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class ColumnOptionComponent {
	@Input() gridService: TableService
	constructor() { }
}
