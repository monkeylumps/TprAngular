/// <reference path="../scripts/typings/angular-ui-bootstrap/angular-ui-bootstrap.d.ts" />
module Kandban {
    export interface IMainScope extends ng.IScope {
        greeting: string;
        state: number;
        error: string;
        boards: IBoard[];
        board: IBoard;
        column: IColumn;
        task:ITask;

        openModal();
        openTaskModal();
        addBoard(board: IBoard);
        close();
        changeBoard(slug: string);

        back();

        addColumn(column: IColumn, board: IBoard);
        addTask(task: ITask, column: IColumn, board: IBoard);
    }

    export interface IBoard {
        Slug: string;
        Name: string;
        Columns: IColumn[];
    }

    export interface IColumn {
        Slug: string;
        Name: string;
        Order: number;
        Tasks: ITask[];
    }

    export interface ITask {
        Id: number;
        Name: string;
        BoardColumnSlug: string;
    }

    export class MainController {
        constructor(private scope: IMainScope, private apiService: IApiService, $uibModal) {
            this.scope.greeting = "Welcome buddy";

            const boards = data => {
                this.scope.boards = data.Items;
                this.scope.state = 1;
            };

            const error = reason => {
                this.scope.error = reason;
                this.scope.state = 3;
            };

            const columns = data => {
                this.scope.board.Columns = data.Items;

                var thing = scope.board.Columns;
                angular.forEach(thing, (column) => {
                    apiService.getTasks(scope.board.Slug, column.Slug, (x) => {
                        column.Tasks = x.Items;
                    }, error);
                });
            }

            const board = data => {
                this.scope.board = data;
                this.scope.state = 2;

                apiService.getColumns(data.Slug, columns, error);
            };

          
            this.scope.changeBoard = (slug: string) => {
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
            }

            this.scope.openModal = () => {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalContent.html',
                    controller: 'ModalInstance',
                    scope: this.scope
                });
            }

            this.scope.openTaskModal = () => {
                $uibModal.open({
                    animation: true,
                    templateUrl: 'myModalTaskContent.html',
                    controller: 'ModalInstance',
                    scope: this.scope
                });
            }

            this.scope.$on('BoardCreated', (event, args) => {
                apiService.createBoard(args, error);

                this.scope.boards.push(args);
            });

            this.scope.$on('ColumnCreated', (event, args: IColumn, board: IBoard) => {
                apiService.createColumn(args, board, error);

                this.scope.board.Columns.push(args);
            });

            this.scope.$on('TaskCreated', (event, args: ITask,  board: IBoard) => {
                apiService.createTask(args, board, error);

                angular.forEach(this.scope.board.Columns, (column) => {
                    if (column.Slug === args.BoardColumnSlug) {
                        column.Tasks.push(args);
                    }
                });
            });

            this.scope.back = () => {
                this.scope.state = 1;
            }

            apiService.getBoards(boards, error);
        }
    }

    angular.module("Kandban").controller("MainController", ["$scope", "ApiService", "$uibModal", MainController]);

    export class ModalInstance {
        constructor(private scope: IMainScope, private apiService: IApiService, $uibModalInstance) {
            this.scope.addBoard = (board: IBoard) => {
                board.Slug = board.Name;

                this.scope.$emit('BoardCreated', board);

                $uibModalInstance.close();
            }

            this.scope.addColumn = (column: IColumn, board: IBoard) => {
                column.Slug = column.Name;

                this.scope.$emit('ColumnCreated', column, board);

                $uibModalInstance.close();
            }

            this.scope.addTask = (task: ITask, column: IColumn, board: IBoard) => {

                task.BoardColumnSlug = column.Slug;

                this.scope.$emit('TaskCreated', task, board);

                $uibModalInstance.close();
            }

            this.scope.close = () => {
                $uibModalInstance.dismiss('cancel');
            }
        }
    }

    angular.module("Kandban").controller("ModalInstance", ["$scope", "ApiService", "$uibModalInstance", ModalInstance]);
}