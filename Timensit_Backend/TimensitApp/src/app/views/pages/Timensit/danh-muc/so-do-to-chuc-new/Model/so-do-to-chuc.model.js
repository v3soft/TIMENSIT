"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
exports.ChartStaffModel = exports.UpdateThongTinChucVuModel = exports.OrgChartModel = void 0;
var crud_1 = require("../../../../../../core/_base/crud");
var OrgChartModel = /** @class */ (function (_super) {
    __extends(OrgChartModel, _super);
    function OrgChartModel() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    OrgChartModel.prototype.clear = function () {
        this.ID_PhongBan = '0';
        this.ID_ChucDanh = '0';
        this.ViTri = 0;
        this.StructureID = '0';
        this.Id_parent = '0';
        this.chucdanhParent = '';
        this.IsAbove = false;
    };
    return OrgChartModel;
}(crud_1.BaseModel));
exports.OrgChartModel = OrgChartModel;
var UpdateThongTinChucVuModel = /** @class */ (function (_super) {
    __extends(UpdateThongTinChucVuModel, _super);
    function UpdateThongTinChucVuModel() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.StructureID = '';
        return _this;
    }
    UpdateThongTinChucVuModel.prototype.clear = function () {
        this.MaCD = '';
        this.SoNhanVien = '';
        this.ViTri = '';
        this.ID_ChucVu = '';
        this.ID_ChucDanh = '';
        this.TenChucVu = '';
        this.TenTiengAnh = '';
        this.ID_DonVi = '';
        this.ID_PhongBan = '';
        this.ID_Cap = 0;
        this.HienThiDonVi = false;
        this.DungChuyenCap = false;
        this.HienThiCap = false;
        this.HienThiPhongBan = false;
        this.ID = 0;
        this.ID_Parent = 0;
        this.StructureID = '';
    };
    return UpdateThongTinChucVuModel;
}(crud_1.BaseModel));
exports.UpdateThongTinChucVuModel = UpdateThongTinChucVuModel;
var ChartStaffModel = /** @class */ (function () {
    function ChartStaffModel() {
    }
    return ChartStaffModel;
}());
exports.ChartStaffModel = ChartStaffModel;
//# sourceMappingURL=so-do-to-chuc.model.js.map