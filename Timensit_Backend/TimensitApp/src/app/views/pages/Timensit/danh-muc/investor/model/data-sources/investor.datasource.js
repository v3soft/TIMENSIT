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
exports.ChucDanhDataSource = void 0;
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var crud_1 = require("../../../../../../../core/_base/crud");
var ChucDanhDataSource = /** @class */ (function (_super) {
    __extends(ChucDanhDataSource, _super);
    function ChucDanhDataSource(ChucDanhService) {
        var _this = _super.call(this) || this;
        _this.ChucDanhService = ChucDanhService;
        return _this;
    }
    ChucDanhDataSource.prototype.loadList = function (queryParams) {
        var _this = this;
        this.ChucDanhService.lastFilter$.next(queryParams);
        this.loadingSubject.next(true);
        this.ChucDanhService.findData(queryParams)
            .pipe(operators_1.tap(function (resultFromServer) {
            _this.entitySubject.next(resultFromServer.data);
            var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
            _this.paginatorTotalSubject.next(totalCount);
        }), operators_1.catchError(function (err) { return rxjs_1.of(new crud_1.QueryResultsModel([], err)); }), operators_1.finalize(function () { return _this.loadingSubject.next(false); })).subscribe(function (res) {
            _this.ChucDanhService.ReadOnlyControl = res.Visible;
        });
    };
    return ChucDanhDataSource;
}(crud_1.BaseDataSource));
exports.ChucDanhDataSource = ChucDanhDataSource;
//# sourceMappingURL=chucdanh.datasource.js.map