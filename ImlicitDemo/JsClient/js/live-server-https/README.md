# live-server-https

> `tls.createServer` options with SSL/TSL certificate for quick https setup for [live-server](https://github.com/tapio/live-server)

## Usage

The module is simply an options argument for `tls.createServer` that includes a valid certificate.

Install it with `npm`:

```sh
npm install --save live-server-https
```

And use it programatically:
```js
const tls = require('tls')
const https = require('live-server-https');

const server = tls.createServer(https, (socket) => {
  console.log('server connected',
              socket.authorized ? 'authorized' : 'unauthorized');
  socket.write('welcome!\n');
  socket.setEncoding('utf8');
  socket.pipe(socket);
});
server.listen(8000, () => {
  console.log('server bound');
});
```

## Usage with `live-server`

[`live-server`](https://github.com/tapio/live-server) expects a path to the module. So if you want to use the same instance everywhere, I'd suggest installing it globally:

```sh
npm install --global live-server-https
```

Then pass its directory to `live-server` under the `--https` flag:

```sh
live-server --https=/usr/local/lib/node_modules/live-server-https
```

If the above doesn't work, find the proper directory by running `npm -g ls live-server-https` and adding `node_modules` to the result.

You can also just install it locally and pass the directory path as `--https=./node_modules/live-server-https`.

Now your live-server instance works with `https`!

## Note about "Trustworthy Authority"

On first load, your browser will warn you that the certificate does not come from a trusted authority. This is good, because `live-server` is not a trusted certification authority. Depending on your browser, you will need to either 'unsafely proceed' or add an exception, both of which are usually under an advanced options in the prompt.

## How it works

The module just exports minimal options for a [tls.createServer](https://nodejs.org/api/tls.html#tls_tls_createserver_options_secureconnectionlistener) instance. It comes with a pregenerated HTTPS certificate [created with openssl](https://help.ubuntu.com/12.04/serverguide/certificates-and-security.html). The certificate is valid until the year 3017, so be sure to sign it again before then.

## License

This is by [Robert Pirtle](https://pirtle.xyz). It's license is [MIT](http://choosealicense.com/licenses/mit/).
