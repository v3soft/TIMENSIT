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
exports.ChucDanhModel = void 0;
var crud_1 = require("../../../../../../core/_base/crud");
var ChucDanhModel = /** @class */ (function (_super) {
    __extends(ChucDanhModel, _super);
    function ChucDanhModel() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ChucDanhModel.prototype.clear = function () {
        this.Id_CV = 0;
        this.MaCV = '';
        this.TenCV = '';
        this.Cap = '';
        this.IsManager = false;
        this.IsTaiXe = false;
    };
    return ChucDanhModel;
}(crud_1.BaseModel));
exports.ChucDanhModel = ChucDanhModel;
//# sourceMappingURL=chucdanh.model.js.map