const express = require('express');
const mongoose = require('mongoose');
const config = require('./config/index');
const helmet = require('helmet');

const mediaRouter = require('./routes/api/media');
const musicRouter = require('./routes/api/musicDB');

const app = express();
const { MONGO_URI } = config;

app.use(helmet());

app.use(express.json());

mongoose.connect(MONGO_URI, {
    useNewUrlParser: true,
})
    .then(() => console.log("MongoDB connecting Success!!"))
    .catch((e) => console.log(e));

//routes
app.get('/');
app.get('/upload',function(req, res){
    const body = `
<html>
  <head>
      <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
  </head>
  <body>
      <form action="/media" enctype="multipart/form-data" method="post">
          <input type="file" name="file1" multiple="multiple">
          <input type="submit" value="Upload file" />
      </form>
  </body>
</html>
`
	res.writeHead(200, {"Content-Type": "text/html"});
	res.write(body);
	res.end();
})
app.use('/media', mediaRouter);
app.use('/music', musicRouter);

module.exports = app;