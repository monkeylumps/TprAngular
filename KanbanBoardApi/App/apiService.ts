module Kandban {
    export interface IApiService {
        createBoard(board: any, success: any, failure: any);
        getBoards(success: any, failure: any);
        getBoard(slug: string, success: any, failure: any);
    }

    export class ApiService implements IApiService {
        constructor(private http: ng.IHttpService) {
        }

        createBoard(board, success, failure) {
            this.http.post("http://localhost:2943/boards", board).success(success).error(failure);
        }

        getBoards(success, failure) {
            this.http.get("http://localhost:2943/boards").success(success).error(failure);
        }

        getBoard(slug: string, success, failure) {
            this.http.get("http://localhost:2943/boards/" +  slug).success(success).error(failure);
        }
    }

    angular.module("Kandban").service("ApiService", ["$http", ApiService]);
}