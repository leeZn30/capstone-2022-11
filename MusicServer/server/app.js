import express from 'express';
import mongoose from "mongoose";
import config from './config/index';
import helmet from 'helmet';

import mediaRouter from './routes/api/media';
import musicRouter from './routes/api/musicDB';

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

export default app;