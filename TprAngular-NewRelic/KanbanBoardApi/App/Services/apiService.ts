module Kandban {
    export interface IApiService {
        createBoard(board: IBoard, failure: any);
        getBoards(success: any, failure: any);
        getBoard(slug: string, success: any, failure: any);

        createColumn(column: IColumn, board: IBoard, failure: any);
        getColumns(boardSlug: string, success: any, failure: any);

        getTasks(boardSlug: string, boardColumnSlug: string, success: any, failure: any);
        createTask(task: ITask, board: IBoard, failure: any);
    }

    export class ApiService implements IApiService {
        constructor(private http: ng.IHttpService) {
        }

        createBoard(board, failure) {
            this.http.post("http://localhost:2943/boards", board).error(failure);
        }

        getBoards(success, failure) {
            this.http.get("http://localhost:2943/boards").success(success).error(failure);
        }

        getBoard(slug: string, success, failure) {
            this.http.get("http://localhost:2943/boards/" + slug).success(success).error(failure);
        }

        createColumn(column: IColumn, board: IBoard, failure: any) {
            this.http.post("http://localhost:2943/boards/" + board.Slug + "/columns", column).error(failure);
        }

        getColumns(boardSlug: string, success, failure) {
            this.http.get("http://localhost:2943/boards/" + boardSlug + "/columns/").success(success).error(failure);
        }

        getTasks(boardSlug: string, boardColumnSlug, success, failure) {
            return this.http.get("http://localhost:2943//boards/" + boardSlug + "/columns/" + boardColumnSlug + "/tasks").success(success).error(failure);
        }

        createTask(task: ITask, board: IBoard, failure: any) {
            this.http.post("http://localhost:2943/boards/"+ board.Slug +"/tasks", task).error(failure);
        }
    }

    angular.module("Kandban").service("ApiService", ["$http", ApiService]);
}