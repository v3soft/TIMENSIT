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
exports.DM_DonViDataSource = void 0;
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var crud_1 = require("../../../../../../core/_base/crud");
var dm_don_vi_model_1 = require("../dm-don-vi-model/dm-don-vi.model");
var DM_DonViDataSource = /** @class */ (function (_super) {
    __extends(DM_DonViDataSource, _super);
    function DM_DonViDataSource(productsService) {
        var _this = _super.call(this) || this;
        _this.productsService = productsService;
        return _this;
    }
    DM_DonViDataSource.prototype.loadDM_DonVis = function (queryParams, HasLoaiXL) {
        var _this = this;
        if (HasLoaiXL === void 0) { HasLoaiXL = false; }
        this.productsService.lastFilter$.next(queryParams);
        this.loadingSubject.next(true);
        this.productsService.getData(queryParams)
            .pipe(operators_1.tap(function (resultFromServer) {
            // console.log("data", resultFromServer);
            if (resultFromServer.status == 1) {
                if (resultFromServer.data) {
                    if (HasLoaiXL) {
                        var tmpdm_donvisResult_1 = [];
                        resultFromServer.data.forEach(function (el) {
                            var tmpElement = new dm_don_vi_model_1.DM_DonViModel();
                            tmpElement.copy(el);
                            tmpdm_donvisResult_1.push(tmpElement);
                        });
                        _this.entitySubject.next(tmpdm_donvisResult_1);
                    }
                    else {
                        _this.entitySubject.next(resultFromServer.data);
                    }
                }
                else {
                    _this.entitySubject.next([]);
                }
                if (resultFromServer.page != null) {
                    var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
                    _this.paginatorTotalSubject.next(totalCount);
                }
                else {
                    _this.paginatorTotalSubject.next(0);
                }
            }
            else {
                _this.entitySubject.next([]);
                _this.paginatorTotalSubject.next(0);
                // let message = '';
                // if(resultFromServer.error.message == null){
                // 	message = 'Lấy thông tin thất bại';
                // }else{
                // 	message = resultFromServer.error.message;
                // }
            }
        }), operators_1.catchError(function (err) { return rxjs_1.of(new crud_1.QueryResultsModel([], err)); }), operators_1.finalize(function () { return _this.loadingSubject.next(false); })).subscribe();
    };
    DM_DonViDataSource.prototype.loadDM_User_DonVis = function (queryParams) {
        var _this = this;
        this.productsService.lastFilter$.next(queryParams);
        this.loadingSubject.next(true);
        this.productsService.getData_User(queryParams)
            .pipe(operators_1.tap(function (resultFromServer) {
            // console.log("data", resultFromServer);
            if (resultFromServer.status == 1) {
                if (resultFromServer.data) {
                    _this.entitySubject.next(resultFromServer.data);
                }
                else {
                    _this.entitySubject.next([]);
                }
                if (resultFromServer.page != null) {
                    var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
                    _this.paginatorTotalSubject.next(totalCount);
                }
                else {
                    _this.paginatorTotalSubject.next(0);
                }
            }
            else {
                _this.entitySubject.next([]);
                _this.paginatorTotalSubject.next(0);
                // let message = '';
                // if(resultFromServer.error.message == null){
                // 	message = 'Lấy thông tin thất bại';
                // }else{
                // 	message = resultFromServer.error.message;
                // }
            }
        }), operators_1.catchError(function (err) { return rxjs_1.of(new crud_1.QueryResultsModel([], err)); }), operators_1.finalize(function () { return _this.loadingSubject.next(false); })).subscribe();
    };
    return DM_DonViDataSource;
}(crud_1.BaseDataSource));
exports.DM_DonViDataSource = DM_DonViDataSource;
//# sourceMappingURL=dm-don-vi.datasource.js.map