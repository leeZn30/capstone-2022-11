import app from './app';
import config from './config/index';

const {PORT} = config

app.listen(PORT, () => {
    console.log('%d PORT로 연결합니당.', PORT);
});