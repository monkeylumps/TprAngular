var Kandban;
(function (Kandban) {
    var ModalInstanceController = (function () {
        function ModalInstanceController(scope, apiService, $uibModalInstance) {
            var _this = this;
            this.scope = scope;
            this.apiService = apiService;
            this.scope.addBoard = function (board, isValid) {
                if (isValid) {
                    board.Slug = board.Name;
                    _this.scope.$emit('BoardCreated', board);
                    $uibModalInstance.close();
                }
            };
            this.scope.addColumn = function (column, board, isValid) {
                if (isValid) {
                    column.Slug = column.Name;
                    _this.scope.$emit('ColumnCreated', column, board);
                    $uibModalInstance.close();
                }
            };
            this.scope.addTask = function (task, column, board, isValid) {
                if (isValid) {
                    task.BoardColumnSlug = column.Slug;
                    _this.scope.$emit('TaskCreated', task, board);
                    $uibModalInstance.close();
                }
            };
            this.scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
        return ModalInstanceController;
    })();
    Kandban.ModalInstanceController = ModalInstanceController;
    angular.module("Kandban").controller("ModalInstanceController", ["$scope", "ApiService", "$uibModalInstance", ModalInstanceController]);
})(Kandban || (Kandban = {}));
//# sourceMappingURL=ModalInstanceController.js.map