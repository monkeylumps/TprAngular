/* */ 
(function(process) {
  try {
    var EventEmitter = require('events').EventEmitter;
  } catch (err) {
    var Emitter = require('component-emitter');
  }
  function noop() {}
  module.exports = Batch;
  function Batch() {
    if (!(this instanceof Batch))
      return new Batch;
    this.fns = [];
    this.concurrency(Infinity);
    this.throws(true);
    for (var i = 0,
        len = arguments.length; i < len; ++i) {
      this.push(arguments[i]);
    }
  }
  if (EventEmitter) {
    Batch.prototype.__proto__ = EventEmitter.prototype;
  } else {
    Emitter(Batch.prototype);
  }
  Batch.prototype.concurrency = function(n) {
    this.n = n;
    return this;
  };
  Batch.prototype.push = function(fn) {
    this.fns.push(fn);
    return this;
  };
  Batch.prototype.throws = function(throws) {
    this.e = !!throws;
    return this;
  };
  Batch.prototype.end = function(cb) {
    var self = this,
        total = this.fns.length,
        pending = total,
        results = [],
        errors = [],
        cb = cb || noop,
        fns = this.fns,
        max = this.n,
        throws = this.e,
        index = 0,
        done;
    if (!fns.length)
      return cb(null, results);
    function next() {
      var i = index++;
      var fn = fns[i];
      if (!fn)
        return;
      var start = new Date;
      try {
        fn(callback);
      } catch (err) {
        callback(err);
      }
      function callback(err, res) {
        if (done)
          return;
        if (err && throws)
          return done = true, cb(err);
        var complete = total - pending + 1;
        var end = new Date;
        results[i] = res;
        errors[i] = err;
        self.emit('progress', {
          index: i,
          value: res,
          error: err,
          pending: pending,
          total: total,
          complete: complete,
          percent: complete / total * 100 | 0,
          start: start,
          end: end,
          duration: end - start
        });
        if (--pending)
          next();
        else if (!throws)
          cb(errors, results);
        else
          cb(null, results);
      }
    }
    for (var i = 0; i < fns.length; i++) {
      if (i == max)
        break;
      next();
    }
    return this;
  };
})(require('process'));
