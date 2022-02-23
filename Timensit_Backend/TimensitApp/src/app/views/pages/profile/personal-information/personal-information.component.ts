// Angular
import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
// Services
// Widgets model
import { LayoutConfigService } from '../../../../core/_base/layout';
import { Observable } from 'rxjs';
import { CommonService } from '../../Timensit/services/common.service';
import { IfStmt } from '@angular/compiler';
import { LayoutUtilsService } from 'app/core/_base/crud';

@Component({
	selector: 'kt-personal-information',
	templateUrl: './personal-information.component.html',
	encapsulation: ViewEncapsulation.None
})
export class PersonalInformationComponent implements OnInit {

	user$: Observable<any>;
	ItemSave: any = {};
	avatar: any = { strBase64: '', filename: '' };
	sign: any = { strBase64: '', filename: '' };
	constructor(private layoutConfigService: LayoutConfigService,
		private commonService: CommonService,
		private changeDetect: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService, ) {
	}

	ngOnInit(): void {

		let data = JSON.parse(localStorage.getItem("UserInfo"));
		console.log(data);
		this.GetInfoByUserID();
	}

	GetInfoByUserID() {
		this.commonService.GetInfoUser().subscribe(res => {
			this.user$ = new Observable((observer) => {
				// observable execution
				observer.next(res.data);
				observer.complete();
				this.changeDetect.detectChanges();
			})
			this.ItemSave.Cast_NgaySinh = res.data.NgaySinh.split('T')[0];
			this.ItemSave.FullName = res.data.FullName;
			this.ItemSave.CMTND = res.data.CMTND;
			this.ItemSave.PhoneNumber = res.data.PhoneNumber;
			this.ItemSave.Email = res.data.Email;
			this.ItemSave.Sign = res.data.Sign;
			this.ItemSave.Avata = res.data.Avata;
			this.changeDetect.detectChanges();
		})
	}

	ValueChanged(value: any, ind: number) {
		if (ind == 1) {
			let NgaySinh = value.targetElement.value.replace(/-/g, '/').split('T')[0].split('/');
			if (+NgaySinh[0] < 10 && NgaySinh[0].length < 2)
				NgaySinh[0] = '0' + NgaySinh[0];
			if (+NgaySinh[1] < 10 && NgaySinh[1].length < 2)
				NgaySinh[1] = '0' + NgaySinh[1];

			this.ItemSave.Cast_NgaySinh = NgaySinh[2] + '-' + NgaySinh[1] + '-' + NgaySinh[0];
		}
		if (ind == 2) { this.ItemSave.FullName = value; }
		if (ind == 3) { this.ItemSave.CMTND = value; }
		if (ind == 4) { this.ItemSave.PhoneNumber = value; }
		if (ind == 5) { this.ItemSave.Email = value; }
		if (ind == 6) { this.ItemSave.Sign = value; }
	}



	Onsubmit() {
		if (this.ItemSave.Email != '') {
			var re = /^[a-z][a-z0-9_\.]{5,32}@[a-z0-9]{2,}(\.[a-z0-9]{2,4}){1,2}$/;
			if (!this.ItemSave.Email.match(re)) {
				this.layoutUtilsService.showError("Email không đúng định dạng");
				return;
			}
		}
		if (this.ItemSave.CMTND != '') {
			var re = /[0-9]/;
			if (!this.ItemSave.CMTND.match(re)) {
				this.layoutUtilsService.showError("CMND không đúng định dạng");
				return;
			}
		}
		if (this.ItemSave.PhoneNumber != '') {
			var re = /[0-9]/;
			if (!this.ItemSave.PhoneNumber.match(re)) {
				this.layoutUtilsService.showError("Điện thoại không đúng định dạng");
				return;
			}
		}
		if (this.ItemSave.FullName == '') {
			this.layoutUtilsService.showError("Họ tên không được bỏ trống");
			return;
		}

		this.ItemSave.avatar = this.avatar;
		this.ItemSave.Sign=this.sign;

		this.commonService.UpdateInfoUser(this.ItemSave).subscribe(res => {
			this.GetInfoByUserID();
			this.layoutUtilsService.showInfo("Thông tin người dùng cập nhật thành công");
		})


	}

	FileSelectedPrivate(evt: any, ind: any) {
		if (evt.target.files && evt.target.files.length) {//Nếu có file
			let file = evt.target.files[0]; // Ví dụ chỉ lấy file đầu tiên
			let size = file.size;
			let filename = file.name;
			// if (size >= 9999999) {
			// 	const message = `Không thể upload hình dung lượng quá cao.`;
			// 	this.layoutUtilsService.showActionNotification(message, MessageType.Update, 10000, true, false);
			// 	return;
			// }
			let reader = new FileReader();
			reader.readAsDataURL(evt.target.files[0]);
			let base64Str;
			reader.onload = function () {
				base64Str = reader.result as String;
				var metaIdx = base64Str.indexOf(';base64,');
				base64Str = base64Str.substr(metaIdx + 8); // Cắt meta data khỏi chuỗi base64
				//item.Image = "";
				let f = document.getElementById("imgIcondd" + ind);
				f.setAttribute("src", "data:image/png;base64," + base64Str);

			};
			setTimeout(res => {
				if (ind == 1) {
					this.avatar.strBase64 = base64Str;
					this.avatar.filename = filename;
					console.log('avatar',this.avatar);
					this.changeDetect.detectChanges();
				}
				else if (ind == 2) { 
					this.sign.strBase64 = base64Str;
					this.sign.filename = filename;
					console.log('sign',this.sign);
					this.changeDetect.detectChanges();
				}
			}, 1000);
		}
	}

	selectFile(ind) {
		let f = document.getElementById("imgInpdd" + ind);
		f.click();
	}
}
