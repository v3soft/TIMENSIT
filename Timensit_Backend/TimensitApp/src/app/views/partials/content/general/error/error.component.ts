// Angular
import { Component, HostBinding, Input, Output, EventEmitter } from '@angular/core';

@Component({
	selector: 'kt-error',
	templateUrl: './error.component.html',
	styleUrls: ['./error.component.scss']
})
export class ErrorComponent {
	// Public properties
	// type of error template to be used, accepted values; error-v1 | error-v2 | error-v3 | error-v4 | error-v5 | error-v6
	@Input() type = 'error-v1';
	// full background image
	@Input() image: string;
	// error code, some error types template has it
	@Input() code = '404';
	// error title
	@Input() title: string;
	// error subtitle, some error types template has it
	@Input() subtitle: string;
	// error descriptions
	@Input() desc = 'Oops! Something went wrong!';
	// return back button title
	@Input() return = 'Return back';

	@Output() back: EventEmitter<any> = new EventEmitter();

	@HostBinding('class') classes = 'kt-grid kt-grid--ver kt-grid--root';

	close() {
		this.back.emit(true);
	}
}
