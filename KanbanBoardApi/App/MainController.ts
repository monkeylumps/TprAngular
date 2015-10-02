module Kandban {
    export interface IMainModel {
        greeting: string;
        error: string;
        boards: any[];
        getboard(slug: string);
    }

    export class MainController implements IMainModel {
        greeting: string;
        error: string;
        boards: any[];
        board: string;
        getboard(slug: string) {
            this.apiService.getBoard(slug, data => {
                this.board = data.Name;
            }, reason => {
                this.error = reason;
            });
        }

        constructor(private apiService: IApiService) {
            this.greeting = "welcome buddy";
            const boardsss = data => {
                this.boards = data.Items;
            };

            const error = reason => {
                this.error = reason;
            };
            apiService.getBoards(boardsss, error);
        }
    }

    angular.module("Kandban").controller("MainController", ["ApiService", MainController]);
}