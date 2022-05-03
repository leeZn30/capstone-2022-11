const express = require('express');
const mongoose = require('mongoose');
const hpp = require('hpp');
const helmet = require('helmet');
const cors = require('cors');
const morgan = require('morgan');

const config = require('./config/index');

const usersRouters = require('./routes/api/user');
const authRouters = require('./routes/api/auth');
const mediaRouter = require('./routes/api/media');
const musicRouter = require('./routes/api/musicDB');
const followRouter = require('./routes/api/follow');

const app = express();
const { MONGO_URI } = config;

app.use(hpp());
app.use(helmet());

app.use(cors({origin: true, credentials: true}));
app.use(morgan("dev"));

// json형태로 데이터 받음?
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
      <form action="/api/media" enctype="multipart/form-data" method="post">
          <input type="file" name="file1" multiple="multiple">
          <input type="file" name="file2" multiple="multiple">
          <input type="submit" value="Upload file" />
      </form>
  </body>
</html>
`
    res.writeHead(200, {"Content-Type": "text/html"});
    res.write(body);
    res.end();
})

app.use('/api/user', usersRouters);
app.use('/api/auth', authRouters);
app.use('/api/media', mediaRouter);
app.use('/api/music', musicRouter);
app.use('/api/follow', followRouter);

module.exports = app;