const express = require("express");
const https = require("https");
const socket = require("socket.io");
const app = express();
const fs = require('fs');
const port = 8000;

const option = { 
    key: fs.readFileSync('privkey.pem', 'utf8'), 
    cert: fs.readFileSync('Certificate.crt', 'utf8'), 
    passphrase:'yiduswn6140' };

const httpsServer = https.createServer(option, app);
httpsServer.listen(port, function () {
    console.log("HTTPS server listening on port " + port);
});
// 정적 파일 불러오기
app.use(express.static(__dirname + "/public"));

// 라우팅 정의
app.get("/", (req, res) => {
  res.sendFile(__dirname + "/webRTC.html");
});

let io = socket(httpsServer);

//Triggered when a client is connected.

io.on("connection", function (socket) {
  console.log("User Connected :" + socket.id);

  //Triggered when a peer hits the join room button.
 //client가 버튼을 클릭했을때, 방 생성(create) 인지 혹은 방 입장(join)인지
  socket.on('joinRoom', function(room) {
    let rooms = io.sockets.adapter.rooms;
    let userId = room[0];
    let roomNum = room[1];
    let roomDefine = rooms.get(roomNum);

    if (roomDefine == undefined) {
        console.log("Create Room :" + room[1] + " ID : " + room[0]);
        roomOption = {roomNum:roomNum, user:userId};
        socket.join(roomNum);
        socket.emit('create', roomOption);
    }
    else {
        console.log("Join Room :" + roomNum + " ID : " + userId);
        roomOption = {roomNum:roomNum, user:userId};
        socket.join(roomNum);
        socket.emit("join", roomOption);
    }

    console.log(rooms);
  });

  //Triggered when the person who joined the room is ready to communicate.
  socket.on("ready", function (roomOption) {
    socket.broadcast.to(roomOption.roomNum).emit("ready", roomOption); //Informs the other peer in the room.
  });

  //Triggered when server gets an icecandidate from a peer in the room.

  socket.on("candidate", function (candidate, roomNum) {
    console.log("Candidate : " + roomNum);
    socket.broadcast.to(roomNum).emit("candidate", candidate); //Sends Candidate to the other peer in the room.
  });

  //Triggered when server gets an offer from a peer in the room.

  socket.on("offer", function (offer, roomOption) {
    console.log("Offer : " + roomOption.user);
    socket.broadcast.to(roomOption.roomNum).emit("offer", offer, roomOption); //Sends Offer to the other peer in the room.
  });

  //Triggered when server gets an answer from a peer in the room.

  socket.on("answer", function (answer, roomOption) {
    console.log("Answer : " + roomOption.user);
    socket.broadcast.to(roomOption.roomNum).emit("answer", answer, roomOption); //Sends Answer to the other peer in the room.
  });
});