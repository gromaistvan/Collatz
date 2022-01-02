'use strict';

const { describe, it } = require('mocha');
const assert = require('assert');
const collatz = require('../collatz');

describe('Collatz viewer', function () {
    it('should create an image', function (done) {
        assert.ok(collatz.createImage(4, [4, 2, 1]));
        done();
    });
    it('should create an HTML', function (done) {
        assert.ok(collatz.createHtml(1, 'Nothing.'));
        done();
    });
});
