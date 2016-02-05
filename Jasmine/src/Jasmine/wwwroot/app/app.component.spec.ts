import {AppComponent} from './app.component';
import {HeroService} from './hero.service';

describe('app component tests', () => {
    let appComponent: AppComponent;
    let hs: HeroService;

    beforeEach(function () {
        hs = new HeroService();
        appComponent = new AppComponent(hs);
    });

    it('getHeroes returns hero list', () => {
        expect(appComponent.getHeroes()).not.toBeNull();
    });
});