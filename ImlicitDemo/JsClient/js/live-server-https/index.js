'use strict';

const fs = require('fs');
const path = require('path');

module.exports = {
  cert: fs.readFileSync(path.join(__dirname, '/server.crt')),
  key: fs.readFileSync(path.join(__dirname, '/server.key')),
  password: 'live-server rulez'
};
