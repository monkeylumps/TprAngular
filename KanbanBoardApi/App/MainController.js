/// <reference path="../scripts/typings/angular-ui-bootstrap/angular-ui-bootstrap.d.ts" />
var Kandban;
(function (Kandban) {
    var MainController = (function () {
        function MainController(scope, apiService, $uibModal) {
            var _this = this;
            this.scope = scope;
            this.apiService = apiService;
            this.scope.greeting = "Welcome buddy";
            var boards = function (data) {
                _this.scope.boards = data.Items;
                _this.scope.state = 1;
            };
            var error = function (reason) {
                _this.scope.error = reason;
                _this.scope.state = 3;
            };
            var columns = function (data) {
                _this.scope.board.Columns = data.Items;
                var thing = scope.board.Columns;
                angular.forEach(thing, function (column) {
                    apiService.getTasks(scope.board.Slug, column.Slug, function (x) {
                        column.Tasks = x.Items;
                    }, error);
                });
            };
            var board = function (data) {
                _this.scope.board = data;
                _this.scope.state = 2;
                apiService.getColumns(data.Slug, columns, error);
            };
            this.scope.changeBoard = function (slug) {
                apiService.getBoard(slug, board, error);
                /*
                apiService.getColumns(slug, columns, error);
                console.log(scope);
                console.log(scope.board);
                console.log(scope.board.Columns);

                var thing =scope.board.Columns;
                angular.forEach(thing, (column) => {
                    column.Tasks = apiService.getTasks(scope.board.Slug, column.Slug, error);
                });*/
            };
            this.scope.openModal = function () {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalContent.html',
                    controller: 'ModalInstance',
                    scope: _this.scope
                });
            };
            this.scope.openTaskModal = function () {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalTaskContent.html',
                    controller: 'ModalInstance',
                    scope: _this.scope
                });
            };
            this.scope.$on('BoardCreated', function (event, args) {
                apiService.createBoard(args, error);
                _this.scope.boards.push(args);
            });
            this.scope.$on('ColumnCreated', function (event, args, board) {
                apiService.createColumn(args, board, error);
                _this.scope.board.Columns.push(args);
            });
            this.scope.$on('TaskCreated', function (event, args, board) {
                apiService.createTask(args, board, error);
                angular.forEach(_this.scope.board.Columns, function (column) {
                    if (column.Slug === args.BoardColumnSlug) {
                        column.Tasks.push(args);
                    }
                });
            });
            this.scope.back = function () {
                _this.scope.state = 1;
            };
            apiService.getBoards(boards, error);
        }
        return MainController;
    })();
    Kandban.MainController = MainController;
    angular.module("Kandban").controller("MainController", ["$scope", "ApiService", "$uibModal", MainController]);
    var ModalInstance = (function () {
        function ModalInstance(scope, apiService, $uibModalInstance) {
            var _this = this;
            this.scope = scope;
            this.apiService = apiService;
            this.scope.addBoard = function (board) {
                board.Slug = board.Name;
                _this.scope.$emit('BoardCreated', board);
                $uibModalInstance.close();
            };
            this.scope.addColumn = function (column, board) {
                column.Slug = column.Name;
                _this.scope.$emit('ColumnCreated', column, board);
                $uibModalInstance.close();
            };
            this.scope.addTask = function (task, column, board) {
                task.BoardColumnSlug = column.Slug;
                _this.scope.$emit('TaskCreated', task, board);
                $uibModalInstance.close();
            };
            this.scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
        return ModalInstance;
    })();
    Kandban.ModalInstance = ModalInstance;
    angular.module("Kandban").controller("ModalInstance", ["$scope", "ApiService", "$uibModalInstance", ModalInstance]);
})(Kandban || (Kandban = {}));
//# sourceMappingURL=MainController.js.map