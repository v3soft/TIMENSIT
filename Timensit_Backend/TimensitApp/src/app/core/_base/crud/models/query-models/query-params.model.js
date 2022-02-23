"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var QueryParamsModel = /** @class */ (function () {
    // constructor overrides
    function QueryParamsModel(_filter, _sortOrder, _sortField, _pageNumber, _pageSize, _filterGroup, _more) {
        if (_sortOrder === void 0) { _sortOrder = 'asc'; }
        if (_sortField === void 0) { _sortField = ''; }
        if (_pageNumber === void 0) { _pageNumber = 0; }
        if (_pageSize === void 0) { _pageSize = 10; }
        if (_filterGroup === void 0) { _filterGroup = []; }
        if (_more === void 0) { _more = false; }
        this.filter = _filter;
        this.sortOrder = _sortOrder;
        this.sortField = _sortField;
        this.pageNumber = _pageNumber;
        this.pageSize = _pageSize;
        this.filterGroup = _filterGroup;
        this.more = _more;
    }
    return QueryParamsModel;
}());
exports.QueryParamsModel = QueryParamsModel;
//# sourceMappingURL=query-params.model.js.map