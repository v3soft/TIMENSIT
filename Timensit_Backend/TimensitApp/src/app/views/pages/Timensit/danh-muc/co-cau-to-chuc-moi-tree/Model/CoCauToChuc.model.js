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
exports.OrgStructureModel = void 0;
var crud_1 = require("../../../../../../core/_base/crud");
var OrgStructureModel = /** @class */ (function (_super) {
    __extends(OrgStructureModel, _super);
    function OrgStructureModel() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    OrgStructureModel.prototype.clear = function () {
        this.IDParent = '';
        this.TitleParent = '';
        this.Level = '';
        this.Position = '';
        this.Title = '';
        this.ID = '0';
        this.RowID = 0;
        this.Code = '';
        this.ViTri = '0';
        this.ParentID = '0';
        this.ID_Goc = null;
        this.WorkingModeID = 0;
        this.level = '';
        this.IsAbove = true;
    };
    return OrgStructureModel;
}(crud_1.BaseModel));
exports.OrgStructureModel = OrgStructureModel;
//# sourceMappingURL=CoCauToChuc.model.js.map