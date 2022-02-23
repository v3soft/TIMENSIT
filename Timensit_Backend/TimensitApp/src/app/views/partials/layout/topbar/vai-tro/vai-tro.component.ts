// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
// Material
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
// RxJS
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
// NGRX
// Service
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
import { AuthService } from '../../../../../core/auth';
import { TokenStorage } from '../../../../../core/auth/_services/token-storage.service';

@Component({
	selector: 'kt-vai-tro',
	templateUrl: './vai-tro.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class VaiTroComponent implements OnInit, OnDestroy {
	// Public properties
	isZoomSize: boolean = false;
	ListVaiTro: any[];
	private componentSubscriptions: Subscription;

	displayedColumns: string[] = ['STT', 'VaiTro', 'DonVi'];
	disabledBtn:boolean=false;
	viewLoading:boolean=false;
	constructor(
		public dialogRef: MatDialogRef<VaiTroComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private auth: AuthService,
		private tokenStorage: TokenStorage) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		if (this.data && this.data.VaiTros)
			this.ListVaiTro = this.data.VaiTros;
	}
	/**
	 * On destroy
	 */
	ngOnDestroy() {
		if (this.componentSubscriptions) {
			this.componentSubscriptions.unsubscribe();
		}
	}

	/**
	 * Update item
	 *
	 * @param item: any
	 */
	update(item: any) {
		if (!item.InUse) {
			this.auth.doiVaiTro(item.IdGroup).subscribe(response => {
				if (response && response.status == 1) {
					response.data.Token = response.data.ResetToken;
					this.tokenStorage.updateStorage(response.data);
					window.location.reload();
				}
				else {
					this.layoutUtilsService.showError(response.error.message);
				}
			});
		}
	}

	closeDialog() {
		this.dialogRef.close();
	}
	resizeDialog() {
		if (!this.isZoomSize) {
			this.dialogRef.updateSize('100vw', '100vh');
			this.isZoomSize = true;
		}
		else if (this.isZoomSize) {
			this.dialogRef.updateSize('900px', 'auto');
			this.isZoomSize = false;
		}

	}
}
