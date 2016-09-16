System.register(['./app.component', './hero.service'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var app_component_1, hero_service_1;
    return {
        setters:[
            function (app_component_1_1) {
                app_component_1 = app_component_1_1;
            },
            function (hero_service_1_1) {
                hero_service_1 = hero_service_1_1;
            }],
        execute: function() {
            describe('app component tests', function () {
                var appComponent;
                var hs;
                beforeEach(function () {
                    hs = new hero_service_1.HeroService();
                    appComponent = new app_component_1.AppComponent(hs);
                });
                it('getHeroes returns hero list', function () {
                    expect(appComponent.getHeroes()).not.toBeNull();
                });
            });
        }
    }
});
//# sourceMappingURL=app.component.spec.js.map