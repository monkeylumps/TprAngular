import {Hero} from './hero';

describe('hero tests', () => {
  it('has the id given in the constructor', function() {
      var hero = new Hero(1, 'Super Cat');
      expect(hero.id).toEqual(1);
    });
    
  it('has name given in the constructor', () => {
    let hero = new Hero(1, 'Super Cat');
    expect(hero.name).toEqual('Super Cat');
  });
});