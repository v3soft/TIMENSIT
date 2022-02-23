import { Injectable } from '@angular/core';
import { LayoutUtilsService, QueryParamsModel, QueryResultsModel, HttpUtilsService } from '../../../../core/_base/crud';
import { FormGroup, FormBuilder } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { TokenStorage } from '../../../../core/auth/_services/token-storage.service';
import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
import { environment } from '../../../../../environments/environment';
import { AuthService } from '../../../../core/auth';
import { AngularEditorConfig } from '@kolkov/angular-editor';

@Injectable()
export class CommonService {

	constructor(private layoutUtilsService: LayoutUtilsService,
		private http: HttpClient,
		private httpUtils: HttpUtilsService,
		private fb: FormBuilder,
		private tokenStorage: TokenStorage,
		private idle: Idle,
		private auth: AuthService,) { }
	Form: FormGroup;
	fixedPoint: number = 0;
	thousandSeparator: string = '.';
	decimalSeperator: string = ',';

	lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 10));
	lastFilterDSExcel$: BehaviorSubject<any[]> = new BehaviorSubject([]);
	lastFilterInfoExcel$: BehaviorSubject<any> = new BehaviorSubject(undefined);
	lastFileUpload$: BehaviorSubject<{}> = new BehaviorSubject({});
	data_import: BehaviorSubject<any[]> = new BehaviorSubject([]);
	ReadOnlyControl: boolean;

	//#region ckeditor
	ckeditor: any = {
		config: {
			language: 'vi',
			uiColor: '#FF8F35',
			// Define the toolbar: https://ckeditor.com/docs/ckeditor4/latest/features/toolbar.html
			// The standard preset from CDN which we used as a base provides more features than we need.
			// Also by default it comes with a 2-line toolbar. Here we put all buttons in a single row.
			toolbar: [
				{ name: 'document', items: ['Print'] },
				{ name: 'clipboard', items: ['Undo', 'Redo'] },
				{ name: 'styles', items: ['Format', 'Font', 'FontSize'] },
				{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'RemoveFormat', 'CopyFormatting'] },
				{ name: 'colors', items: ['TextColor', 'BGColor'] },
				{ name: 'align', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
				{ name: 'links', items: ['Link', 'Unlink'] },
				{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote'] },
				{ name: 'insert', items: ['Image', 'Table'] },
				{ name: 'tools', items: ['Maximize', 'Source'] },
				{ name: 'editing', items: ['Scayt'] }
			],
			extraPlugins: 'colordialog,tableresize',
			// Make the editing area bigger than default.
			height: 300,
		},
		editorUrl: './assets/plugins/ckeditor/ckeditor.js'
	}
	editorConfig: AngularEditorConfig = {
		editable: true,
		spellcheck: true,
		height: '15rem',
		minHeight: '5rem',
		placeholder: 'Nhập vào nội dung',
		translate: 'no',
		sanitize: false,
		defaultFontSize: '2',
		//uploadUrl: SystemConstants.BASE_API + 'api/tien-ich/upload-img?token=' + JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)).token, // if needed
		customClasses: [ // optional
			{
				name: "no-margin",
				class: "no-margin",
			}
		]
	};
	tinyConfig: any = {
		language: 'vi',
		base_url: '/tinymce', // Root for resources
		suffix: '.min', // Suffix to use when loading resources
		plugins: 'print preview  importcss searchreplace autolink autosave save directionality visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap quickbars emoticons',
		//powerpaste casechange tinydrive advcode mediaembed tinymcespellchecker checklist a11ychecker formatpainter permanentpen pageembed tinycomments mentions linkchecker advtable
		//tinydrive_token_provider: 'URL_TO_YOUR_TOKEN_PROVIDER',
		//tinydrive_dropbox_app_key: 'YOUR_DROPBOX_APP_KEY',
		//tinydrive_google_drive_key: 'YOUR_GOOGLE_DRIVE_KEY',
		//tinydrive_google_drive_client_id: 'YOUR_GOOGLE_DRIVE_CLIENT_ID',
		mobile: {
			plugins: 'print preview  importcss searchreplace autolink autosave save directionality visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap quickbars emoticons',
		},
		menu: {
			tc: {
				title: 'TinyComments',
				items: 'addcomment showcomments deleteallconversations'
			}
		},
		menubar: 'file edit view insert format tools table tc help',
		toolbar: 'undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent | numlist bullist checklist | forecolor backcolor casechange permanentpen formatpainter removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media pageembed template link anchor codesample | a11ycheck ltr rtl | showcomments addcomment',
		fontsize_formats: "8pt 10pt 12pt 13pt 14pt 18pt 24pt 36pt",
		autosave_ask_before_unload: true,
		autosave_interval: '30s',
		autosave_prefix: '{path}{query}-{id}-',
		autosave_restore_when_empty: false,
		autosave_retention: '2m',
		image_advtab: true,
		link_list: [
			{ title: 'My page 1', value: 'https://www.tiny.cloud' },
			{ title: 'My page 2', value: 'http://www.moxiecode.com' }
		],
		image_list: [
			{ title: 'My page 1', value: 'https://www.tiny.cloud' },
			{ title: 'My page 2', value: 'http://www.moxiecode.com' }
		],
		image_class_list: [
			{ title: 'None', value: '' },
			{ title: 'Some class', value: 'class-name' }
		],
		importcss_append: true,
		templates: [
			{ title: 'New Table', description: 'creates a new table', content: '<div class="mceTmpl"><table width="98%%"  border="0" cellspacing="0" cellpadding="0"><tr><th scope="col"> </th><th scope="col"> </th></tr><tr><td> </td><td> </td></tr></table></div>' },
			{ title: 'Starting my story', description: 'A cure for writers block', content: 'Once upon a time...' },
			{ title: 'New list with dates', description: 'New List with dates', content: '<div class="mceTmpl"><span class="cdate">cdate</span><br /><span class="mdate">mdate</span><h2>My List</h2><ul><li></li><li></li></ul></div>' }
		],
		template_cdate_format: '[Date Created (CDATE): %m/%d/%Y : %H:%M:%S]',
		template_mdate_format: '[Date Modified (MDATE): %m/%d/%Y : %H:%M:%S]',
		height: 600,
		image_caption: true,
		quickbars_selection_toolbar: 'bold italic | quicklink h2 h3 blockquote quickimage quicktable',
		noneditable_noneditable_class: 'mceNonEditable',
		toolbar_mode: 'sliding',
		spellchecker_ignore_list: ['Ephox', 'Moxiecode'],
		tinycomments_mode: 'embedded',
		content_style: '.mymention{ color: gray; } p {margin: 0}',
		contextmenu: 'link image imagetools table configurepermanentpen',
		a11y_advanced_options: true,
		//skin: useDarkMode ? 'oxide-dark' : 'oxide',
		//content_css: useDarkMode ? 'dark' : 'default',
		/*
		The following settings require more configuration than shown here.
		For information on configuring the mentions plugin, see:
		https://www.tiny.cloud/docs/plugins/premium/mentions/.
		*/
		mentions_selector: '.mymention',
		//mentions_fetch: mentions_fetch,
		//mentions_menu_hover: mentions_menu_hover,
		//mentions_menu_complete: mentions_menu_complete,
		//mentions_select: mentions_select,
		mentions_item_type: 'profile'
	};
	//#endregion

	public static list_button() {
		var temp = localStorage.getItem('DROP_BUTTON');
		return temp == "1";
	};
	ValidateChangeNumberEvent(columnName: string, item: any, event: any) {
		var count = 0;
		for (let i = 0; i < event.target.value.length; i++) {
			if (event.target.value[i] == this.decimalSeperator) {
				count += 1;
			}
		}
		var regex = /[a-zA-Z -!$%^&*()_+|~=`{}[:;<>?@#\]]/g;
		var found = event.target.value.match(regex);
		if (found != null) {
			const message = 'Dữ liệu không gồm chữ hoặc kí tự đặc biệt';
			this.layoutUtilsService.showError(message);
			return false;;
		}
		if (count >= 2) {
			const message = 'Dữ liệu không thể có nhiều hơn 2 dấu .';
			this.layoutUtilsService.showError(message);
			return false;;
		}
		return true;
	}
	/**
	 * Phonenumber: type= 'phone'
	 * Domain: type= 'domain'
	 * fax: type='fax'
	 */
	ValidateFormatRegex(type: string): any {
		if (type == 'phone') {
			return /[0][0-9]{9}/;
		}
		if (type == 'domain') {
			return /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.[a-zA-Z]{2,}$/;
		}
		if (type == 'fax') {
			return /[0][0-9]{9}/; //^(\+?\d{1,}(\s?|\-?)\d*(\s?|\-?)\(?\d{2,}\)?(\s?|\-?)\d{3,}\s?\d{3,})$
		}
		if (type == 'password') {
			return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/; //^(\+?\d{1,}(\s?|\-?)\d*(\s?|\-?)\(?\d{2,}\)?(\s?|\-?)\d{3,}\s?\d{3,})$
		}
		if (type == 'integer')
			return /^-?[0-9][^\.]*$/;
		if (type == 'prior')
			return /(^[1-9][0-9]*$)/
	}

	formatNumber(item: string) {
		if (item == '' || item == null || item == undefined) return '';
		return Number(Math.round(parseFloat(item + 'e' + this.fixedPoint)) + 'e-' + this.fixedPoint).toFixed(this.fixedPoint)
	}

	f_currency(value: string): any {
		if (value == null || value == undefined || value == '') value = '0' + this.decimalSeperator + '00';
		let nbr = Number((value.substring(0, value.length - (this.fixedPoint + 1)) + '').replace(/,/g, ""));
		return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1" + this.thousandSeparator) + value.substr(value.length - (this.fixedPoint + 1), (this.fixedPoint + 1));
	}

	f_currency_V2(value: string): any {
		if (value == '-1') return '';
		if (value == null || value == undefined || value == '') return '';
		let nbr = Number((value + '').replace(/,/g, ""));
		return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1" + this.thousandSeparator);
	}
	stringToInt(value) {
		return parseInt(value.replaceAll(this.thousandSeparator, ''));
	}

	numberOnly(event): boolean {
		const charCode = (event.which) ? event.which : event.keyCode;
		if (charCode > 31 && (charCode < 48 || charCode > 57)) {
			return false;
		}
		return true;
	}

	getFormatDate(v: string = '') {
		if (v != '') {
			return v.includes('T') ? v.replace(/(\d{4})(-)(\d{2})(-)(\d{2})(T)(\d{2})(:)(\d{2})(:)(\d{2}).*$/g, "$5/$3/$1") : v.replace(/(\d{4})(-)(\d{2})(-)(\d{2})/g, "$5/$3/$1");
		}
		return '';
	}

	f_convertDate(v: any) {
		if (!v)
			return null;
		let a = v === "" ? new Date() : new Date(v);
		return a.getFullYear() + "-" + ("0" + (a.getMonth() + 1)).slice(-2) + "-" + ("0" + (a.getDate())).slice(-2) + "T00:00:00.0000000";
	}

	//#region form helper
	buildForm(data: any) {
		this.Form = this.fb.group(data);
	}
	/**
	 * Checking control validation
	 *
	 * @param controlName: string => Equals to formControlName
	 * @param validationType: string => Equals to valitors name
	 */
	isControlHasError(controlName: string, validationType: string): boolean {
		const control = this.Form.controls[controlName];
		if (!control) {
			return false;
		}

		const result = control.hasError(validationType) && (control.dirty || control.touched);
		return result;
	}
	//#endregion

	//#region file đính kèm
	public download_dinhkem(Id: number): Observable<any> {
		var _token = '';
		this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
		let headers = new HttpHeaders({
			'Authorization': 'Bearer ' + _token,
		})
		headers.append("Content-Type", "multipart/form-data");
		return this.http.get(environment.ApiRoot + '/lite/download-dinhkem?id=' + Id, { headers });//, responseType: 'blob'
	}

	view_dinhkem(Id: number): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/view-dinhkem?id=' + Id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	get_dinhkem(Loai: number, Id: number): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/get-dinhkem?loai=' + Loai + '&id=' + Id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	lite_emotion(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/lite_emotion';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	//#endregion
	//#region danh mục DM_DanhMuc
	GetAllProvinces() {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/GetListProvinces';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GetListDistrictByProvinces(idTinh) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/GetListDistrictByProvinces?id_provinces=' + idTinh;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GetListWardByDistrict(id_district) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/GetListWardByDistrict?id_district=' + id_district;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GetListKhomApByWard(id_ward) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/GetListKhomApByWard?id_ward=' + id_ward;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	//#endregion
	//#region ***filter***
	getFilterGroup(column: string, url: string): Observable<any> {
		return this.http.get<any>(environment.ApiRoot + url + `${column}`);
	}
	//#endregion

	//#region ds lite
	ListLoaiChungThu(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/loai-chung-thu';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ListLoaiBieuMau(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/loai-bieu-mau';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ListGioiTinh(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/gioi-tinh';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ListDanToc(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/dan-toc-lite';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ListTonGiao(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/ton-giao-lite';
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteNhomLoaiDoiTuongNCC(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/nhom-loai-doi-tuong-ncc`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteLyDoGiam(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/ly-do-giam`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteQHGiaDinh(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/quan-he-gia-dinh-lite?ByQua=true&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteQHGiaDinhNCC(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/quan-he-gia-dinh-lite?ByQua=false&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteLoaiGiayTo(Id_LoaiGiayTo: number = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/loai-giay-to-lite?Id_LoaiGiayTo=${Id_LoaiGiayTo}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteGiayTo(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/giay-to-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteCheDoUuDai(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/che-do-uu-dai-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteDoiTuongNhanQua(locked: boolean = false, include_muc: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/doi-tuong-nhan-qua-lite?Locked=${locked}&include_muc=${include_muc}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteDoiTuongNCC(locked: boolean = false, Id_LoaiHoSo: number = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/doi-tuong-ncc-lite?Locked=${locked}&Id_LoaiHoSo=${Id_LoaiHoSo}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteDoiTuongNCC_LoaiHS(locked: boolean = false, tendt: string = ""): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/doi-tuong-ncc-loai-hs-lite?Locked=${locked}&tendt=${tendt}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteMauQDTangQua(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/mau-quyet-dinh-tang-qua`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteNhomLeTet(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/nhom-le-tet-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteLoaiDieuDuong(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/loai-dieu-duong-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteDungCuChinhHinh(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/dung-cu-chinh-hinh-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteDienChinhHinh(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/dien-chinh-hinh-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteFilter(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/filter-lite`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteFilterOperator(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/filter-operator-lite`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteLoaiSoLieu(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/loai-so-lieu-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteSoLieu(Id_LoaiSoLieu: number = 0, locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/so-lieu-lite?Id_LoaiSoLieu=${Id_LoaiSoLieu}&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteSoLieuParentIsNull(Id_LoaiSoLieu: number = 0, locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/lite-so-lieu-parent-is-null?Id_LoaiSoLieu=${Id_LoaiSoLieu}&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	litePhiSoLieu(Id_PhiSoLieu: number = 0, locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/lite-phi-so-lieu?Id_LoaiSoLieu=${Id_PhiSoLieu}&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteMucQua(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/muc-qua-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteDotQua(locked: boolean = false, nam: number = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/dot-tang-qua-lite?Locked=${locked}&Nam=${nam}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteNguonKinhPhi(locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/nguon-kinh-phi-lite?Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteNguoiNCC(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/ncc?more=true`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteConstLoaiHoSo(include_giayto: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/const-loai-ho-so-lite?include_giayto=${include_giayto}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteConstLoaiHoSo_DT(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/const-loai-ho-so-dt-lite`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteConstLoaiQuyetDinh(includeBM: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/const-loai-quyet-dinh-lite?includeBM=` + includeBM;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteMauSoLieuTheoDonVi(id = 0, lock: boolean = true): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/mau-so-lieu-don-vi?Id=` + id + `&Locked=` + lock;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteMauSoLieu(lock: boolean = true, isMauPhong: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/mau-so-lieu?Locked=${lock}&isMauPhong=${isMauPhong}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteCanCu(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/can-cu-lite`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	liteBieuMau(loai: number = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/bieu-mau-lite?loai=${loai}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	//id:id loại hồ sơ: const_LoaiHoSo
	liteConstLoaiTroCap(id = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/const-loai-tro-cap-lite?id=${id}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	liteConstLoaiTroCapCha(Id_LoaiHoSo = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/const-loai-tro-cap-cha-lite?Id_LoaiHoSo=${Id_LoaiHoSo}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	getListNhomNguoiDung(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/nhom-nguoi-dung';
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	ListChucVu(Id): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/DM_ChucVu?Iddonvi=' + Id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	TreeChucVu(dv, locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/tree-chuc-vu?Donvi=${dv}&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	//#region quyền
	getTreeQuyen(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/tree-quyen?IdGroup=${itemId}`, { headers: httpHeaders });
	}

	CheckRole(IDRole): any {
		var roles = JSON.parse(localStorage.getItem('userRoles'));
		return roles.filter(x => x == IDRole);
	}
	//#endregion
	//#region Vai trò
	ListVaiTroByDonVi(Id): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/vai-tro?IdDonVi=' + Id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ListVaiTroAll(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/vai-tro';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ListVaiTroPhanTrang(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const httpParams = this.httpUtils.getFindHTTPParams(queryParams);
		return this.http.get<any>(environment.ApiRoot + '/nhom-nguoi-dung/list', { headers: httpHeaders, params: httpParams });
	}
	//#endregion
	//#region đơn vị
	GetTreeDonVi() {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/LayTreeDonVi`, { headers: httpHeaders });
	}
	GetTreeDonViHC(idParent = 0, idParentGoc = 0) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/LayTreeDonVi?idParent=${idParent}&idParentGoc=${idParentGoc}&isHC=true`, { headers: httpHeaders });
	}
	TreeDonVi(type: number = 0, idParent: number = 0, locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		var url = environment.ApiRoot + `/lite/DM_PhongBan_Tree?Id=${idParent}&Locked=${locked}`;
		if (type == 1) {//đơn vị theo cơ cấu
			url = environment.ApiRoot + `/lite/DM_PhongBan_Tree_CC?Id=${idParent}&Locked=${locked}`;
		}
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	TreeDonViByParent(Id): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/DM_PhongBan_Tree?Id=' + Id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	TreeDonViCC(Id, locked: boolean = false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/DM_PhongBan_Tree_CC?Id=${Id}&Locked=${locked}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	//#endregion

	//#region user
	GetThongBao(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/user/GetThongBao`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GetThongBaoLastest(lastID: string): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/user/GetThongBao?lastid=${lastID}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GetThongBaoPage(pagesize: number, pageindex: number): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/user/GetThongBaoPage?more=false&record=${pagesize}&page=${pageindex}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	ReadNotify(Id): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/user/ReadNotify?Id=' + Id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GetInfoUser(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/user/GetInfoUser';
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	UpdateInfoUser(Item): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/user/UpdateInfoUser';
		return this.http.post<any>(url, Item, { headers: httpHeaders });
	}

	changePassword(data): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/user/change-password';
		return this.http.post<any>(url, data, { headers: httpHeaders });
	}
	//#endregion

	Log_HanhDong(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/Log_HanhDong';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	Log_LoaiLog(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/Log_Loai';
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	//#region DuLam get dropdown  danh-sach-nguoi-lite
	//drop người dùng theo đơn vị
	getDSNguoiDungLite(useVaiTro: boolean = true, idDV: number = 0): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + `/lite/danh-sach-nguoi-lite?useVaiTro=${useVaiTro}&idDV=${idDV}`;
		return this.http.post<any>(url, null, { headers: httpHeaders });
	}
	//nhom lam viec lite
	DMNhomLamViec(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/dm-nhom-lam-viec-lite?loai=1';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	DMNhomLamViecLienThong(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/dm-nhom-lam-viec-lite?loai=2';
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	//drop  đơn vị (auto complete)
	getDSDonViLite(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/danh-sach-don-vi-lite';
		return this.http.post<any>(url, null, { headers: httpHeaders });
	}
	getTreeNguoiDungDonVi(itemId: any, useVaiTro: boolean = true): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/tree-nguoi-dung-don-vi?Id=${itemId}&useVaiTro=${useVaiTro}`, { headers: httpHeaders });
	}
	getDonViTheoParent(Id: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/Lite_DanhSachDonVi_Prent?Id=${Id}`, { headers: httpHeaders });
	}
	//#endregion

	//#region config
	getConfig(code: string[]): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/get-config';
		return this.http.post<any>(url, code, { headers: httpHeaders });
	}
	//#endregion

	Delete_FileDinhKem(id): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = environment.ApiRoot + '/lite/delete-dinhkem?id=' + id;
		return this.http.get<any>(url, { headers: httpHeaders });
	}

	TimesOutExpire() {
		if (localStorage.getItem('TIME_LOGOUT') != undefined && +localStorage.getItem('TIME_LOGOUT') != 0) {
			// khi user không hđ trong ('TIME_LOGOUT'/1000)s (không di chuyển chuột, nhấn bất kỳ phím hoặc nút nào,...),
			// cảnh báo sẽ được hiển thị khi hết khoảng tg này
			this.idle.setIdle(+localStorage.getItem('TIME_LOGOUT') / 1000);

			// chờ thêm 1s người dùng vẫn ko hđ sẽ logout
			this.idle.setTimeout(1);

			// sets the default interrupts, in this case, things like clicks, scrolls, touches to the document
			this.idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);

			this.idle.onTimeout.subscribe(() => {
				//this.idleState = 'Timed out!';
				this.layoutUtilsService.showInfo('Bạn đã không thao tác trong ' + (+localStorage.getItem('TIME_LOGOUT') / 1000 / 60) + ' phút, phần mềm sẽ đăng xuất');
				this.auth.logout(true);
				//this.timedOut = true;
			});

			this.idle.watch();
			//this.timedOut = false;
		}
	}

	ThongKeDasboard() {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/thong-ke/thong-ke-dasboard', { headers: httpHeaders });
	}
	BieuDoThongKeVanBan() {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/thong-ke/bieudo-vanban', { headers: httpHeaders });
	}
	LastestFeedbackDasboard(queryParams: QueryParamsModel) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const httpParms = this.httpUtils.getFindHTTPParams(queryParams)
		return this.http.get<any>(environment.ApiRoot + '/thong-bao/get-thong-bao-dashboard', { headers: httpHeaders, params: httpParms });
	}

	getAllChucdanh(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<QueryResultsModel>(environment.ApiRoot + '/chuc-danh?more=true', { headers: httpHeaders });
	}
	getAllInvestor(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<QueryResultsModel>(environment.ApiRoot + '/investor?more=true', { headers: httpHeaders });
	}
	getAllContract(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<QueryResultsModel>(environment.ApiRoot + '/contract?more=true', { headers: httpHeaders });
	}
	findData_Emp(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const httpParams = this.httpUtils.getFindHTTPParams(queryParams);
		const url = environment.ApiRoot + '/lite/Get_DSNhanVien';
		return this.http.get<QueryResultsModel>(url, {
			headers: httpHeaders,
			params: httpParams
		});
	}

	GetListPosition(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/GetListPosition`, { headers: httpHeaders });
	}
	GetListPositionbyStructure(id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/GetListPositionbyStructure?structureid=${id}`, { headers: httpHeaders });
	}
	GetListJobtitleByStructure(cd, id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/GetListJobtitleByStructure?id_cv=${cd}&&structureid=${id}`, { headers: httpHeaders });
	}
	Get_CoCauToChuc(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/DM_PhongBan_Tree', { headers: httpHeaders });
	}
	GetListPositionbyStructure_All(id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListPositionbyStructure_All', { headers: httpHeaders });
	}
	GetListJobtitleByStructure_All(cd, id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListJobtitleByStructure_All', { headers: httpHeaders });
	}
	GetListBranchbyCustomerID(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListBranchbyCustomerID', { headers: httpHeaders });
	}
	GetListDepartmentbyBranch(id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListDepartmentbyBranch', { headers: httpHeaders });
	}
	GetListTeamByDepartment(id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListTeamByDepartment', { headers: httpHeaders });
	}
	GetListPositionbyDepartment(id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListPositionbyDepartment', { headers: httpHeaders });
	}
	GetListJobtitleByPosition(cd, id): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListJobtitleByPosition', { headers: httpHeaders });
	}
	GetListNhomChucDanhTheoChucDanh(id_cv): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListNhomChucDanhTheoChucDanh?id_cv=' + id_cv, { headers: httpHeaders });
	}
	GetListOnlyNhomChucDanh(id_nhomcd): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListOnlyNhomChucDanh?id_nhomcd=' + id_nhomcd, { headers: httpHeaders });
	}
	getCapQuanLy(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListCapQuanLy', { headers: httpHeaders });
	}
	GetListShift(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + '/lite/GetListShift', { headers: httpHeaders });
	}
	GetListOrganizationalChartStructure(): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<QueryResultsModel>(environment.ApiRoot + '/lite/cap-co-cau', { headers: httpHeaders });
	}


	//#endregion
}
