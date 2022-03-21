import express from 'express';
import mongoose from "mongoose";
import config from './config';
import hpp from 'hpp';
import helmet from 'helmet';
import cors from 'cors';
import morgan from "morgan";

// import postsRouters from './routes/api/post';
import usersRouters from './routes/api/user';
import authRouters from './routes/api/auth';

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
// app.use('/api/post', postsRouters);
app.use('/api/user', usersRouters);
app.use('/api/auth', authRouters);

export default app;