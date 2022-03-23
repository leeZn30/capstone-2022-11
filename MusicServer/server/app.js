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

app.use('/media', mediaRouter);
app.use('/music', musicRouter);

module.exports = app;