const cluster = require('cluster');
const numCPUs = require('os').cpus().length;

if (cluster.isMaster) {
    for (let i = 0; i < numCPUs; i++){
        cluster.fork();
    }

    cluster.on('exit', function(worker, code, signal) {
        console.log("worker-" + worker.process.pid + ' 꺼짐');
    });
}

else{
    const app = require('./app');
    const config = require('./config/index');
    const {PORT} = config

    app.listen(PORT, () => {
        console.log('%d PORT로 API 서버 연결합니다.', PORT);
    });
}
