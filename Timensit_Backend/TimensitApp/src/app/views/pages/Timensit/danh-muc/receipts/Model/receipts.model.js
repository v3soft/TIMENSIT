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
exports.chucvuModel = void 0;
var crud_1 = require("../../../../../../core/_base/crud");
var chucvuModel = /** @class */ (function (_super) {
    __extends(chucvuModel, _super);
    function chucvuModel() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    chucvuModel.prototype.clear = function () {
        this.Id_CV = '';
        this.Tenchucdanh = '';
        this.Tentienganh = '';
        this.tenchucvu = '';
        this.Id_row = 0;
    };
    return chucvuModel;
}(crud_1.BaseModel));
exports.chucvuModel = chucvuModel;
//# sourceMappingURL=chucvu.model.js.map