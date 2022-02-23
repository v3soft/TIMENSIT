import { MatPaginatorIntl } from '@angular/material';
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class CustomMatPaginatorIntl extends MatPaginatorIntl {
	constructor(private translate: TranslateService) {
		super();

		this.translate.onLangChange.subscribe((e: Event) => {
			this.getAndInitTranslations();
		});
		this.getAndInitTranslations();
	}

	getAndInitTranslations() {
		this.translate.get([
			'COMMON.ITEM_PER_PAGE',
			'COMMON.NEXT_PAGE',
			'COMMON.PRE_PAGE',
			'COMMON.FIRST_PAGE',
			'COMMON.LAST_PAGE',
			'COMMON.OF'
		]).subscribe(translation => {
			this.itemsPerPageLabel = translation['COMMON.ITEM_PER_PAGE'];
			this.nextPageLabel = translation['COMMON.NEXT_PAGE'];
			this.previousPageLabel = translation['COMMON.PRE_PAGE'];
			this.firstPageLabel = translation['COMMON.FIRST_PAGE'];
			this.lastPageLabel = translation['COMMON.LAST_PAGE'];
			this.changes.next();
		});
	}
	getRangeLabel = (page: number, pageSize: number, length: number) => {
		if (length === 0 || pageSize === 0) {
			return `0 ${this.translate.instant('COMMON.OF')} ${length}`;
		}
		length = Math.max(length, 0);
		const startIndex = page * pageSize;
		const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;
		return `${startIndex + 1} - ${endIndex} ${this.translate.instant('COMMON.OF')} ${length}`;
	}
}
