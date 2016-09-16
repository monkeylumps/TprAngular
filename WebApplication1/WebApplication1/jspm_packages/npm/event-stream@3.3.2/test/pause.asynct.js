/* */ 
var es = require('../index'),
    it = require('it-is'),
    d = require('ubelt');
exports['gate buffers when shut'] = function(test) {
  var hundy = d.map(1, 100, d.id),
      gate = es.pause(),
      ten = 10;
  es.connect(es.readArray(hundy), es.log('after readArray'), gate, es.map(function(num, next) {
    it(gate.paused).equal(false);
    console.log('data', num);
    if (!--ten) {
      console.log('PAUSE');
      gate.pause();
      d.delay(gate.resume.bind(gate), 10)();
      ten = 10;
    }
    next(null, num);
  }), es.writeArray(function(err, array) {
    console.log('eonuhoenuoecbulc');
    it(array).deepEqual(hundy);
    test.done();
  }));
  gate.resume();
};
require('./helper/index')(module);
