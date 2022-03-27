const express = require("express");
const https = require("https");
const socket = require("socket.io");
const app = express();
const fs = require('fs');
const { join } = require("path");
const port = 8080;

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
let roomCreator = {};
let roomJoiner = {};
let userStreams = {};

let iceServers = {
    iceServers: [
      { urls: "stun:stun.services.mozilla.com" },
      { urls: "stun:stun.l.google.com:19302" },
    ],
};              

io.on('connection', function(socket) {
    console.log("User Connected :" + socket.id);

    socket.on('joinRoom', function(room) {
        let rooms = io.sockets.adapter.rooms;
        let userId = room[0];
        let roomNum = room[1];
        let roomDefine = rooms.get(roomNum);

        if (roomDefine == undefined) {
            console.log("Create Room :" + roomNum + " ID : " + userId);
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

    socket.on('senderOffer', function(offer, senderOption) {
        console.log("Server Get Sender Offer");
        try {
                let receiverPC = createReceiverPeerConnection(senderOption);
                receiverPC.setRemoteDescription(offer);
                let answer = receiverPC.createAnswer({
                    offerToReceiveAudio: true,
                    offerToReceiveVideo: true,
            });
            receiverPC.setLocalDescription(answer);
        } catch (error) {
            console.log(error);
        }
    })

    // socket.on('senderCandidate', function(candi, senderPC) {
    //     console.log("Candidate : " + senderPC);
    //     socket.broadcast.to(senderPC).emit("receiverPC", candi); //Sends Candidate to the other peer in the room.    
    // })

    socket.on('senderCandidate', function(candi, rtcPeerConnection){
        console.log("Candidate : " + senderPC);
        rtcPeerConnection.addIceCandidate(candi);
    })
});

function createReceiverPeerConnection(senderOption) {
    let rtcPeerConnection = new wrtc.RTCPeerConnection(iceServers);
    rtcPeerConnection.onicecandidate = OnIceCandidateFunction;
    rtcPeerConnection.ontrack = e => {
        if (userStreams[senderOption.roomNum]) {
            if (!isIncluded(userStreams[senderOption.roomNum], senderOption.userId)) {
                userStreams[senderOption.roomNum].push({
                    id: senderOption.userId,
                    stream: e.streams[0],
                });
            } else return;
        } else {
            userStreams[senderOption.roomNum] = [
                {
                    id: senderOption.userId,
                    stream: e.streams[0],
                },
            ];
        }
    return rtcPeerConnection;
    }   
}

function OnIceCandidateFunction(event) {
    if (event.candidate) {
      socket.emit("getSenderCandidate", event.candidate);
    }
}