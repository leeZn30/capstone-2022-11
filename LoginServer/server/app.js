const express = require('express');
const mongoose = require('mongoose');
const hpp = require('hpp');
const helmet = require('helmet');
const cors = require('cors');
const morgan = require('morgan');

const config = require('./config/index');

const usersRouters = require('./routes/api/user');
const authRouters = require('./routes/api/auth');

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

app.use('/api/user', usersRouters);
app.use('/api/auth', authRouters);

module.exports = app;