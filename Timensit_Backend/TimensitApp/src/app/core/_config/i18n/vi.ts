// Tiếng Việt
export const locale = {
	lang: 'vi',
	data: {
		TRANSLATOR: {
			SELECT: 'Chọn ngôn ngữ của bạn',
		},
		MENU: {
			NEW: 'new',
			ACTIONS: 'Actions',
			CREATE_POST: 'Create New Post',
			PAGES: 'Pages',
			FEATURES: 'Features',
			APPS: 'Apps',
			DASHBOARD: 'Trang chủ',
		},
		AUTH: {
			GENERAL: {
				OR: 'Or',
				SUBMIT_BUTTON: 'Hoàn tất',
				NO_ACCOUNT: 'Don\'t have an account?',
				SIGNIN_BUTTON: 'Đăng nhập',
				SIGNUP_BUTTON: 'Đăng ký',
				FORGOT_BUTTON: 'Quên mật khẩu',
				BACK_BUTTON: 'Trở lại',
				PRIVACY: 'Privacy',
				LEGAL: 'Legal',
				CONTACT: 'Contact',
			},
			LOGIN: {
				TITLE: 'Đăng nhập',
				BUTTON: 'Đăng nhập',
			},
			FORGOT: {
				TITLE: 'Quên mật khẩu?',
				DESC: 'Enter your email to reset your password',
				SUCCESS: 'Reset mật khẩu thành công'
			},
			REGISTER: {
				TITLE: 'Sign Up',
				DESC: 'Enter your details to create your account',
				SUCCESS: 'Your account has been successfuly registered.'
			},
			INPUT: {
				EMAIL: 'Email',
				FULLNAME: 'Họ tên',
				PASSWORD: 'Mật khẩu',
				CONFIRM_PASSWORD: 'Xác nhận mật khẩu',
				USERNAME: 'Tên đăng nhập'
			},
			VALIDATION: {
				INVALID: '{{name}} không hợp lệ',
				REQUIRED: '{{name}} là bắt buộc nhập',
				MIN_LENGTH: '{{name}} tối thiểu phải {{min}} ký tự',
				AGREEMENT_REQUIRED: 'Accepting terms & conditions are required',
				NOT_FOUND: 'The requested {{name}} is not found',
				INVALID_LOGIN: 'The login detail is incorrect',
				REQUIRED_FIELD: 'Required field',
				MIN_LENGTH_FIELD: 'Chiều dài tối thiểu:',
				MAX_LENGTH_FIELD: 'Chiều dài tối đa:',
				INVALID_FIELD: 'Không hợp lệ',
				ERROR_MESSAGE: '{{error}}'
			}
		},
		COMMON: {
			SELECTED_RECORDS_COUNT: 'Số dòng được chọn: ',
			FILTER_BY: 'Lọc theo',
			SEARCH: 'Tìm kiếm',
			IN_ALL_FIELDS: 'trong tất cả các trường',
			NO_RECORD_FOUND: 'Không có dữ liệu',
			ITEM_PER_PAGE: 'Số dòng trên trang',
			NEXT_PAGE: 'Trang tiếp theo',
			PRE_PAGE: 'Trang trước',
			OF: 'trong',
			FIRST_PAGE: 'Trang đầu tiên',
			LAST_PAGE: 'Trang cuối cùng',
			PLEASE_WAIT: 'Vui lòng đợi trong giây lát....',
			CREATE: 'Thêm mới',
			UPDATE: 'Sửa',
			DELETE: 'Xóa',
			DELETE_ALL: 'Xóa tất cả',
			BACK: 'Trở lại',
			THOAT: 'Thoát',
			CANCEL: 'Đóng',
			CANCEL_CHANGE: 'Hủy thay đổi',
			SAVE: 'Lưu',
			DETAIL: 'Xem',
			BLOCK: 'Khóa',
			UNBLOCK: 'Mở khóa',
			SAVE_CHANGES: 'Lưu thay đổi',
			LUU_DONG: 'Lưu và đóng',
			LUU_TIEPTUC: 'Lưu và tiếp tục',
			LUU_THEMMOI: 'Lưu và thêm mới',
			STT: 'STT',
			NOTE: 'Ghi chú',
			TACVU: 'Tác vụ',
			CREATED_BY: 'Người tạo',
			CREATED_DATE: 'Ngày tạo',
			UPDATED_BY: 'Người sửa',
			UPDATED_DATE: 'Ngày sửa',
			PRINT: 'In',
			EXPORT: 'Xuất',
			DOWNLOAD: 'Tải file',
			PRIORITY: 'Thứ tự',
			CHON: 'Chọn',
			TIEPTUC: 'Tiếp tục',
		},
		OBJECT: {
			NAME: '{{name}}',
			LIST: 'Danh sách {{name}}',
			DELETE: {
				TITLE: `Xóa {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn xóa {{name}}?`,
				WAIT_DESCRIPTION: `{{name}} đang được xóa...`,
				MESSAGE: `Xóa {{name}} thành công`
			},
			DELETE_MULTY: {
				TITLE: `Xóa nhiều {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn xóa {{name}} được chọn?`,
				WAIT_DESCRIPTION: `{{ name }} được chọn đang được xóa...`,
				MESSAGE: `Xóa {{ name }} thành công`
			},
			UPDATE_STATUS: {
				TITLE: `Trạng thái đã được cập nhật cho {{ name }} được chọn`,
				MESSAGE: `Cập nhật trạng thái {{ name }} thành công`
			},
			EDIT: {
				UPDATE_MESSAGE: `Cập nhật {{ name }} thành công`,
				ADD_MESSAGE: `Thêm mới {{ name }} thành công`,
				DUPLICATE: `Nhân bản {{ name }} thành công`,
			},
			LOCK: {
				TITLE: `Khoá {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn khoá {{name}}?`,
				WAIT_DESCRIPTION: `{{ name }} được chọn đang được khóa...`,
				MESSAGE: `Khóa {{name}} thành công`,
			},
			UNLOCK: {
				TITLE: `Mở khoá {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn mở khoá {{name}}?`,
				WAIT_DESCRIPTION: `{{ name }} được chọn đang được mở khoá...`,
				MESSAGE: `Mở khóa {{name}} thành công`,
			},
			DUYET: {
				TITLE: `Duyệt {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn duyệt {{name}}?`,
				WAIT_DESCRIPTION: `{{name}} đang được duyệt...`,
				MESSAGE: `Duyệt {{name}} thành công`
			},
			KHONGDUYET: {
				TITLE: `Không duyệt {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn không duyệt {{name}}?`,
				WAIT_DESCRIPTION: `{{name}} đang được không duyệt...`,
				MESSAGE: `Không duyệt {{name}} thành công`
			},
			THUHOI: {
				TITLE: `Thu hồi {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn thu hồi {{name}}?`,
				WAIT_DESCRIPTION: `{{name}} đang được thu hồi...`,
				MESSAGE: `Thu hồi {{name}} thành công`
			},
			GUIDUYET: {
				TITLE: `Gửi duyệt {{name}}`,
				DESCRIPTION: `Bạn có chắc muốn gửi duyệt {{name}}?`,
				WAIT_DESCRIPTION: `{{name}} đang được gửi duyệt ...`,
				MESSAGE: `Gửi duyệt {{name}} thành công`
			},
		},
		CUSTOMER: {
			NAME: 'Khách hàng'
		},
		PROVINCE: {
			NAME: 'Tỉnh thành'
		},
		DISTRICT: {
			NAME: 'Quận huyện'
		},
		WARD: {
			NAME: 'Phường xã'
		},
		DANTOC: {
			NAME: 'Dân tộc',
			tendantoc:'Tên dân tộc'
		},
		TONGIAO: {
			NAME: 'Tôn giáo',
			tentongiao:'Tên tôn giáo'
		},
		CAP_QL: {
			NAME: 'Cấp quản lý',
			TIEU_DE: 'Tiêu đề',
			CAP: 'Cấp',
			MO_TA: 'Mô tả'
		},
		LOAI_DD:
		{
			NAME: 'Loại điều dưỡng',
			LOAIDIEUDUONG: 'Loại điều dưỡng',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',

		},
		DM_KHAC:
		{
			NAME: 'Danh mục khác',
			DANHMUCKHAC: 'Danh mục khác',
			LOAIHOSO: 'Loại hồ sơ',
			MALOAIHOSO: 'Mã loại hồ sơ',
			MOTA: 'Mô tả',
			MAUCONGNHAN: 'Mẫu công nhận',
			LOAIGIAYTO: 'Loại giấy tờ',
			LOAIGIAYTOCC: 'Loại giấy tờ căn cứ',

		},
		LOAI_GT:
		{
			NAME: 'Loại giấy tờ',
			LOAIGIAYTO: 'Loại giấy tờ',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
		},
		PHI_SO_LIEU:
		{
			NAME: 'Nhập số liệu',
			PhiSoLieu: 'Tên nhập số liệu',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
		},
		CHUC_DANH: {
			NAME: 'Chức danh',
			machucdanh: 'Mã chức danh',
			tenchucdanh: 'Tên chức danh',
			cap: 'Cấp',
			laquanly: 'Là quản lý'
		},
		Investor: {
			NAME: 'Nhà đầu tư',
			email: 'Email',
			phone: 'Số điện thoại',
			address: 'Địa chỉ',
			citizenid: 'CMMD/CCCD'
		},
		CONTRACT: {
			NAME: 'Hợp đồng',
			code: 'Số hợp đồng',
			Amount: 'Số tiền',
			StartDate: 'Ngày bắt đầu',
			EndDate: 'Ngày kết thúc',
			DepositPeriod: 'Kỳ gửi tiền',
			Fund: 'Quỹ',
			ProfitShare: 'Chia sẻ lợi nhuận',
		},
		RECEIPTS: {
			NAME: 'Biên nhận',
			Amount: 'Số tiền',
			ReceiptDate: 'Ngày nhận',
			EffectiveDate: 'Ngày có hiệu lực',
			Type: 'Loại',
		},
		NAV: {
			NAME: 'Net Asset Value',
			Amount: 'Số tiền',
			RecordDate: 'Ngày ghi',
			EffectiveDate: 'Ngày có hiệu lực',
			Type: 'Loại',
		},
		CHUC_VU: {
			NAME: 'Chức vụ',
			tenchucvu: 'Tên chức vụ',
			tentienganh: 'Tên tiếng anh'
		},
		NHOM_LE_TET: {
			NAME: 'Nhóm lễ tết',
			tennhomletet: 'Tên nhóm lễ tết',
			mota: 'Mô tả',
			ADD: 'Thêm nhóm lễ tết',
			UPDATE: 'Cập nhật nhóm lễ tết',
			DETAIL: 'Chi tiết nhóm lễ tết'
		},
		DUNG_CU_CHINH_HINH: {
			NAME: 'Dụng cụ chỉnh hình',
			tendungcu: 'Tên dụng cụ',
			madungcu: 'Mã dụng cụ',
			mota: 'Mô tả',
			thoigian: 'Thời gian',
			trigia: 'Trị giá',
			ADD: 'Thêm dụng cụ chỉnh hình',
			UPDATE: 'Cập nhật dụng cụ chỉnh hình',
			UPDATE_COST: 'Cập nhật trị giá',
			DETAIL: 'Chi tiết dụng cụ chỉnh hình'
		},
		SO_DO_TO_CHUC: {
			NAME: 'Sơ đồ tổ chức',
			themchucdanhnhohonchucdanhchon: 'Thêm chức danh nhỏ hơn',
			xoachucdanhchon: 'Xóa chức danh',
			capnhatthongtinchucvu: 'Cập nhật thông tin',
			tenchucvu: 'Tên chức vụ',
			SoNhanVien: 'Số nhân viên',
			ViTri: 'Vị trí',
			HienThiDonVi: 'Hiển thị đơn vị',
			DungChuyenCap: 'Dừng chuyển cấp',
			organizationalchart: 'Sơ đồ tổ chức',
			confirmxoa: 'Chương trình sẽ xóa tất cả chức vụ con và nhân viên có chức vụ đó. Bạn có chắc xóa không?',
			MaCD: 'Mã chức vụ',
			tenchucdanh: 'Tên chức vụ',
			TenTiengAnh: 'Tên tiếng anh',
			HienThiCap: 'Hiển thị cấp',
			HienThiPhongBan: 'Hiển thị phòng ban',
			tentienganh: 'Tên tiếng anh',
			choncapquanly: 'Chọn cấp quản lý',
			vesodotochuc: 'Xem sơ đồ tổ chức',
			themmoi: 'Thêm chức vụ bên dưới chức vụ',
			HienThiID: 'Hiện thị thông tin ID',
			chonchucdanh: 'Chọn chức danh',
			chonchucvu: 'Chọn chức vụ'
		},
		CO_CAU_TO_CHUC: {
			NAME: 'Cơ cấu tổ chức',
			tieude: 'Phòng ban/Bộ phận',
			ma: 'Code',
			ten: 'Tên',
			capcocau: 'Cấp cơ cấu',
			vitri: 'Vị trí',
			themmoi: 'Thêm cơ cấu tổ chức nhỏ hơn cơ cấu chọn',
			xoacocau: 'Xóa cơ cấu tổ chức chọn',
			capnhat: 'Cập nhật thông tin cơ cấu tổ chức',
			donvihanhchanh: 'Đơn vị hành chánh tương ứng',
			chedo: 'Chế độ làm việc'
		},
		QUY_TRINH_DUYET: {
			NAME: 'Quy trình duyệt',
			CAP_QUAN_LY_DUYET: 'Cấp quản lý duyệt',
			tieude1: 'Nhập quy trình duyệt',
			tenquytrinh: 'Tên quy trình',
			mota: 'Mô tả',
			tieude2: 'Danh sách nhận email khi kết thúc quy trình và đơn được duyệt',
			tieude3: 'Danh sách nhận email khi kết thúc quy trình và đơn không được duyệt',
			tieude4: 'Danh sách nhận email khi không tìm thấy người duyệt khi quy trình đang chạy',
			stt: 'STT',
			nhanemailkhiduyetdon: 'Nhận mail khi duyệt đơn',
			nhanemailkhikhongduyetdon: 'Nhận mail khi không duyệt đơn',
			nhanemailkhikhongthaynguoiduyetdon: 'Nhận mail khi không tìm thấy người duyệt đơn',
			tieude5: 'Nhập cấp quản lý duyệt',
			capduyet: 'Cấp duyệt',
			donvi: 'Đơn vị',
			phongban: 'Phòng ban',
			chucdanh: 'Chức danh',
			chucvu: 'Chức vụ',
			ghichu: 'Ghi chú',
			danhsachnhanemail: 'Danh sách nhận email',
			trolai: 'Trở lại',
			themmoi: 'Thêm mới',
			tieude: 'Tiêu đề',
			vitri: 'Vị trí',
			capquanly: 'Cấp quản lý',
			nguoinhanemail: 'Người nhận email',
			themnguoinhanemailkhiduyetdon: 'Thêm người nhận mail khi duyệt đơn',
			themnguoinhanemailkhikhongduyetdon: 'Thêm người nhận mail khi không duyệt đơn',
			themnguoinhanemailkhikhongthaynguoiduyetdon: 'Thêm người nhận mail khi không tìm thấy người duyệt đơn',
			luuvatieptuc: 'Lưu và tiếp tục',
			tieptuc: 'Tiếp tục',
			boqua: 'Bỏ qua',
			tencapduyet: 'Tên cấp duyệt',
			capduyetmax: 'Cấp duyệt lớn nhất',
			capnhat: 'Cập nhật',
			themnguoinhanemail: 'Thêm người nhận email',
			luu: 'Lưu',
			// =========================Bổ sung=======================
			thongtinchung: 'Thông tin chung',
			cacbuocduyet: 'Các bước duyệt',
			themmoiquytrinh: 'Thêm mới quy trình duyệt',
			capnhatquytrinh: 'Cập nhật quy trình duyệt',
			apdungkyduyet: 'Áp dụng ký duyệt'
		},
		DOITUONG_NCC: {
			NAME: 'Đối tượng người có công',
			DOITUONG: 'Đối tượng',
			MADOITUONG: 'Mã đối tượng',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
			CREATEDBY: 'Tạo bởi',
			LOAI: 'loại',
			NHOMLOAIDOITUONGNCC: 'Nhóm loại đối tượng người có công',
			CREATEDDATE: 'Ngày tạo',
		},
		DOITUONGNHANQUA: {
			NAME: 'Đối tượng nhận quà',
			DOITUONG: 'Đối tượng',
			MADOITUONG: 'Mã đối tượng',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
			CREATEDBY: 'Tạo bởi',
			LOAI: 'loại',
			CREATEDDATE: 'Ngày tạo',
		},
		DIENCHINHHINH: {
			NAME: 'Diện chỉnh hình',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
			CREATEDBY: 'Tạo bởi',
			LOAI: 'loại',
			CREATEDDATE: 'Ngày tạo',
		},
		MUCQUA: {
			NAME: 'Mức quà',
			MOTA: 'Mô tả',
			LOCKED: 'Tình trạng',
			SOTIEN: 'Số tiền',
			PRIORITY: 'Thứ tự',
			CREATEDBY: 'Tạo bởi',
			LOAI: 'loại',
			CREATEDDATE: 'Ngày tạo',
		},
		CHE_DO_UU_DAI: {
			NAME: 'Chế độ ưu đãi',
			tenchedo: 'Tên chế độ ưu đãi',
			mota: 'Mô tả',
			ADD: 'Thêm chế độ ưu đãi',
			UPDATE: 'Cập nhật chế độ ưu đãi',
			DETAIL: 'Chi tiết chế độ ưu đãi'
		},
		QUANHEGIADINH: {
			NAME: 'Quan hệ gia đình',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
			CREATEDBY: 'Tạo bởi',
			LOAI: 'loại',
			CREATEDDATE: 'Ngày tạo',
		},
		SO_LIEU: {
			NAME: 'Số liệu',
			tensolieu: 'Tên số liệu',
			mota: 'Mô tả',
			loaisolieu: 'Loại số liệu',
			ADD: 'Thêm số liệu',
			UPDATE: 'Cập nhật số liệu',
			DETAIL: 'Chi tiết số liệu'
		},
		LOAI_SO_LIEU: {
			NAME: 'Loại số liệu',
			tennhomsolieu: 'Tên loại số liệu',
			mota: 'Mô tả',
			ADD: 'Thêm loại số liệu',
			UPDATE: 'Cập nhật loại số liệu',
			DETAIL: 'Chi tiết loại số liệu'
		},
		DOT_TANG_QUA: {
			NAME: 'Đợt tặng quà lễ tết',
			tendottangqua: 'Tên đợt tặng quà',
			mota: 'Mô tả',
			ADD: 'Thêm đợt tặng quà lễ tết',
			UPDATE: 'Cập nhật đợt tặng quà lễ tết',
			DETAIL: 'Chi tiết đợt tặng quà lễ tết',
			NHANBAN: 'Nhân bản đợt tặng quà lễ tết',
		},
		DE_XUAT: {
			NAME: 'Đề xuất tặng quà',
			ADD: 'Thêm đề xuất tặng quà',
			UPDATE: 'Cập nhật đề xuất tặng quà',
			DETAIL: 'Chi tiết đề xuất tặng quà',
			DUYET: 'Duyệt đề xuất tặng quà'
		},
		NGUONKINHPHI: {
			NAME: 'Nguồn kinh phí',
			LOCKED: 'Tình trạng',
			PRIORITY: 'Thứ tự',
			CREATEDBY: 'Tạo bởi',
			LOAI: 'loại',
			CREATEDDATE: 'Ngày tạo',
		},
		THANNHAN: {
			NAME: 'Thân nhân người có công'
		},
		QUYETDINH: {
			NAME: 'Quyết định'
		},
		TROCAP: {
			NAME: 'Trợ cấp'
		},
		DICHUYEN: {
			NAME: 'Di chuyển'
		},
		DINHCHINH: {
			NAME: 'Đính chính',
			DUYET: 'Duyệt',
			TCDUYET: 'Từ chối duyệt',
		},
		THONG_TIN_NCC:
		{
			hoten: 'Họ tên',
			hotenls: 'Họ tên liệt sỹ',
			ngaysinh: 'Ngày sinh đối tượng',
			namsinh: 'Năm sinh đối tượng',
			bidanh: 'Bí danh',
			gioitinh: 'Giới tính',
			nguyenquan: 'Nguyên quán',
			doituong: 'Đối tượng người có công',
			tinh: 'Tỉnh',
			huyen: 'Huyện',
			xa: 'Xã/phường',
			ap:'Khóm/ấp',
			truquan: 'Trú quán',
			diachi: 'Địa chỉ nhà, tên đường',
			ngaynhapngu: 'Ngày nhập ngũ',
			namnhapngu: 'Năm nhập ngũ',
			noicongtac: 'Nơi công tác',
			ngayxuatngu: 'Ngày xuất ngũ',
			namxuatngu: 'Năm xuất ngũ',
			mo: 'Nơi an táng Hài cốt',
			id_doituongncc: 'Đối tượng NCC',
			ngayvaodang: 'Ngày vào Đảng',
			ngayvaodang_ct: 'Ngày vào Đảng chính thức',
			loaihoso: 'Loại hồ sơ',
			sohoso: 'Số Hồ sơ',
			ischet: 'Đối tượng đã mất',
			ngaychet: 'Ngày từ trần của đối tượng',
			sokhaitu: 'Số khai tử của đối tượng',
			ngaykhaitu: 'Ngày khai tử của đối tượng',
			noikhaitu: 'Nơi khai tử của đối tượng',
			hoten1: 'Họ tên của thân nhân',
			ngaysinh1: 'Ngày sinh của thân nhân',
			namsinh1: 'Năm sinh của thân nhân',
			gioitinh1: 'Giới tính của thân nhân',
			nguyenquan1: 'Nguyên quán của thân nhân',
			diachi1: 'Trú quán của thân nhân',
			quanhe: 'Quan hệ với đối tượng',
			ischet1: 'Thân nhân đã mất',
			ngaychet1: 'Ngày từ trần của thân nhân',
			sokhaitu1: 'Số khai tử của thân nhân',
			ngaykhaitu1:'Ngày khai tử của thân nhân',
			noikhaitu1: 'Nơi khai tử của thân nhân',
		},
		LOAIHOSO: {
			NAME: 'Loại hồ sơ'
		},
		CANCU: {
			NAME: 'Căn cứ',
			SO: 'Số căn cứ',
			HieuLuc_From:'Hiệu lực từ',
			HieuLuc_To: 'Hiệu lực đến',
			IsHieuLuc: 'Có hiệu lực',
			NgayBanHanh: 'Ngày ban hành',
			NguoiKy: 'Người ký',
			CoQuanBanHanh: 'Cơ quan ban hành',
			PhanLoai:'Phân loại',
			TrichYeu:'Trích yếu',
		},
		BIEUMAU: {
			NAME: 'Biểu mẫu',
			SO: 'Số',
			VERSION:'Phiên bản'
		}
	}
};
