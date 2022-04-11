const express = require("express");
const https = require("http");
const socket = require("socket.io");
const app = express();
const fs = require('fs');
const { join } = require("path");
const port = 8080;
const wrtc = require("wrtc");

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
let serverReceiverPCs = {};
let serverSenderPCs = [];
let userStreams = {};
let rooms = {};

let iceServers = {
    iceServers: [
      { urls: "stun:stun.services.mozilla.com" },
      { urls: "stun:stun.l.google.com:19302" },
    ],
};              

io.on('connection', function(socket) {
    console.log("User Connected :" + socket.id);

    socket.on('Test', function(msg){
        console.log(msg);
    });

    socket.on('joinRoom', function(userOption) {
        let userId = userOption.userId;
        let roomNum = userOption.roomNum;

        if (roomNum in rooms) {
            rooms[roomNum].push({'userId':userId});
            roomOption = {'roomNum':roomNum, 'userId':userId};
            socket.join(roomNum);
            socket.emit("joinRoom", roomOption);
        }
        else {
            rooms[roomNum] = [{'userId':userId}];
            roomOption = {'roomNum':roomNum, 'userId':userId};
            socket.join(roomNum);
            socket.emit('createRoom', roomOption);
        }
        console.log(rooms);
    });

    socket.on('senderOffer', async function(offer, userOption) {
        console.log("[SERVER]get Offer");
        try {
            socket.join(userOption.roomNum);
            serverReceiverPCs[userOption.roomNum] = {'senderPC':userOption.senderPC};
            let receiverPC = createReceiverPeerConnection(userOption);
            receiverPC.onicecandidate = event => {
                if (event.candidate) {
                    console.log("[SERVER]send Candi");
                    socket.emit("getCandidate", event.candidate, userOption.option);
                }
            }
            serverReceiverPCs[userOption.roomNum]['receivePC'] = receiverPC;
            receiverPC.setRemoteDescription(offer);
            receiverPC
            .createAnswer({
                offerToReceiveAudio: true,
                offerToReceiveVideo: true,
            })
            .then((answer) => {
                console.log("[SERVER]userID : " + userOption.userId);
                receiverPC.setLocalDescription(new wrtc.RTCSessionDescription(answer));
                socket.emit("getReceiverAnswer", answer);
            })
        } catch (error) {
            console.log(error);
        }
    });

    socket.on("joinRoomFromClient", function(userOption){
        socket.join(userOption.roomNum);
        let sendStream = userStreams[userOption.roomNum].stream;

        //create Sender PC
        let sendPC = createSenderPeerConnection(sendStream, userOption);
        sendPC.onicecandidate = event => {
            socket.emit("getCandidate", event.candidate, userOption.option);
        };
        serverSenderPCs.push({
                'receivePC':userOption.receivePC, 
                'senderPC':sendPC, 
                'id':userOption.userId
        });
        sendPC
        .createOffer({
          offerToReceiveAudio: true,
          offerToReceiveVideo: true,
        })
        .then((offer) => {
            console.log("[SERVER]sender Offer");
            sendPC.setLocalDescription(new wrtc.RTCSessionDescription(offer));
            socket.emit("senderOffer", offer, userOption);
        })
    });

    socket.on("getSenderCandidate", async function(candidate, userOption){
        console.log("[SERVER] get Sender Candi");
        let icecandidate = new wrtc.RTCIceCandidate(candidate);
        let rtcPC = serverReceiverPCs[userOption.roomNum].receivePC;
        await rtcPC.addIceCandidate(icecandidate);
        console.log("ServerCandi Perfect");
    });
    
    socket.on("getReceiverCandidate", async function(candidate, userOption){
        console.log("[SERVER]get Receiver Candi");
        let icecandidate = new wrtc.RTCIceCandidate(candidate);
        let rtcPC;
        for (let i = 0; i < serverSenderPCs.length; i++) {
            if (serverSenderPCs[i].id === userOption.userId) {
                rtcPC = serverSenderPCs[i].senderPC;
                await rtcPC.addIceCandidate(icecandidate);
                break;
            }
        }
    });

    socket.on("getReceiverAnswer", async function(answer, userOption){
        console.log("[SERVER]get Answer");
        let rtcPC;
        for (let i = 0; i < serverSenderPCs.length; i++) {
            if (serverSenderPCs[i].id == userOption.userId) {
                rtcPC = serverSenderPCs[i].senderPC;
                break;
            }
        }

        await rtcPC.setRemoteDescription(answer);
    });
});

function createSenderPeerConnection(userStream) {
    try{
        let rtcPeerConnection = new wrtc.RTCPeerConnection(iceServers);
        rtcPeerConnection.addTrack(userStream.getTracks()[0], userStream);
        rtcPeerConnection.addTrack(userStream.getTracks()[1], userStream);
        return rtcPeerConnection;
    }
    catch(e) {
        console.log(e);
    }
}

function createReceiverPeerConnection(senderOption) {
    try {
        let rtcPeerConnection = new wrtc.RTCPeerConnection(iceServers);
        rtcPeerConnection.ontrack = e => {
            userStreams[senderOption.roomNum] = 
                    {
                        id: senderOption.userId,
                        stream: e.streams[0],
                    };
            }
        return rtcPeerConnection;
    }   
    catch (e) {
        console.log(e);
    }
}