const app = require('./app');
const config = require('./config/index');

const {PORT} = config

app.listen(PORT, () => {
    console.log('%d PORT로 API 서버 연결합니다.', PORT);
});