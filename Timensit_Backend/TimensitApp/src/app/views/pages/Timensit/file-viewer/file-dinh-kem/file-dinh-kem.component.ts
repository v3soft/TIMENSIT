import { Component, OnInit, ElementRef, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef, HostListener, SecurityContext, AfterViewInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonService } from '../../services/common.service';
import { LayoutUtilsService } from '../../../../../core/_base/crud';
import { DomSanitizer } from '@angular/platform-browser';
import { take } from 'rxjs/operators';
import { interval, Subscription } from 'rxjs';

@Component({
	selector: 'm-file-dinh-kem',
	templateUrl: './file-dinh-kem.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})
export class FileDinhKemViewerComponent implements OnInit, AfterViewInit {
	IdRow: number = 0;
	src: any;
	Url: any;
	@ViewChild('iframeRef', { static: false }) iframeRef: ElementRef;
	private checkIFrameSubscription: Subscription = null;
	constructor(private activatedRoute: ActivatedRoute,
		private router: Router,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		public commonService: CommonService,
		private changeDetectorRefs: ChangeDetectorRef,
		private sanitizer: DomSanitizer) {
		this.dialog.closeAll();
	}
	ngOnInit() {
		this.activatedRoute.queryParams.subscribe(params => {
			if (params.path) {
				this.Url = params.path;
				console.log(params.path)
				//let src = 'https://docs.google.com/viewer?url=' + params.path + '&embedded=true'; 
				let src = 'https://view.officeapps.live.com/op/view.aspx?src=' + params.path //embed
				//https://docs.google.com/viewer?url=
				//https://www.dropbox.com/home?preview=
				this.src = this.sanitizer.bypassSecurityTrustResourceUrl(this.sanitizer.sanitize(SecurityContext.URL, src));
				console.log(src)
				this.changeDetectorRefs.detectChanges();
			}
			if (params.id && params.id != '0') {
				this.IdRow = params.id;
				this.getUrl();
			}
		});
	}
	ngAfterViewInit() {
		let iframe = (this.iframeRef) ? this.iframeRef.nativeElement : null;
		this.checkIFrame(iframe);
		this.checkIFrameSubscription = interval(3000)
			.pipe(take(Math.round(20000 / 3000)))
			.subscribe(() => {
				if (iframe == null) {
					iframe = (this.iframeRef) ? this.iframeRef.nativeElement : null;
					this.checkIFrame(iframe);
				}
				this.reloadIFrame(iframe);
			});
	}

	checkIFrame(iframe: any) {
		if (iframe) {
			iframe.onload = () => {
				console.log("checkIFrame", new Date)
				if (this.checkIFrameSubscription) {
					this.checkIFrameSubscription.unsubscribe();
				}
			};
		}
	}

	reloadIFrame(iframe: any) {
		if (iframe) {
			console.log("reloadIFrame", new Date)
			//let src = 'https://docs.google.com/viewer?url=' + this.Url + '&embedded=true';
			let src = 'https://view.officeapps.live.com/op/view.aspx?src=' + this.Url
			iframe.src = src; // Google document viewer url.
		}
	}
	async getDetail() {
		return await this.commonService.view_dinhkem(this.IdRow).toPromise();
	}
	async getUrl() {
		let src = '';
		await this.getDetail().then(res => {
			if (res && res.status == 1) {
				// src = 'https://docs.google.com/viewer?url=' + res.data + '&embedded=true';
				src = 'https://view.officeapps.live.com/op/view.aspx?src=' + res.data
				this.src = this.sanitizer.bypassSecurityTrustResourceUrl(this.sanitizer.sanitize(SecurityContext.URL, src));
				this.changeDetectorRefs.detectChanges();
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}
	docLoaded() {
		// console.log("docLoaded", new Date)
	}
	download(pdf = false) {
		window.open(this.Url);
	}
	in() {
		document.querySelector('iframe').contentWindow.print()
	}
}
