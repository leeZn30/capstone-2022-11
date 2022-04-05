const express = require("express");
const https = require("https");
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
let serverReceiverPCs = {'room1':{'senderPC':'yannju'}};
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
    socket.on('disconnect', function(reason){
        console.log(`${socket.id}님이 ${reason}의 이유로 퇴장하셨습니다. `)
        leaveUser(socket);
    });

    socket.on('joinRoom', function(userOption) {
        let userId = userOption.userId;
        let roomNum = userOption.roomNum;

        if (roomNum in rooms) {
            rooms[roomNum].push({'userId':userId});
            roomOption = {'roomNum':roomNum, 'userId':userId};
            socket.emit("joinRoom", roomOption);
        }
        else {
            rooms[roomNum] = [{'userId':userId}];
            roomOption = {'roomNum':roomNum, 'userId':userId};
            socket.emit('createRoom', roomOption);
        }
        console.log(rooms);
    });

    socket.on('senderOffer', async function(offer, userOption) {
        console.log("[SERVER]get Offer");
        try {
            socket.join(userOption.roomNum);
            if (userOption.roomNum in serverReceiverPCs) {
                console.log("[SERVER ERROR-!] already create Room!")
                throw "roomErr";
            }
            serverReceiverPCs[userOption.roomNum] = {
                'senderPC':userOption.senderPC, 
                'senderId':userOption.userId,
                'receiverId':socket.id
            };
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
            if (error == "roomErr") {
                socket.emit("Error", error);
            }
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
                'roomNum':userOption.roomNum,
                'senderPC':sendPC, 
                'receivePC':userOption.receivePC, 
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
        try{
            console.log("[SERVER] get Sender Candi");
            let icecandidate = new wrtc.RTCIceCandidate(candidate);
            let rtcPC = serverReceiverPCs[userOption.roomNum].receivePC;
            await rtcPC.addIceCandidate(icecandidate);
            console.log("ServerCandi Perfect");
        }
        catch(err) {
            console.log(err);
            socket.emit('Error', err);
        }
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

function leaveUser(socket) {
    let Keys = Object.keys(rooms);
    for (let i = 0; i < Keys.length; i++) { //room 탐색
        let tmpRoom = rooms[Keys[i]] //ex tmpRoom == rooms[room1]
        for (let j = 0; j < tmpRoom.length; j++) {
            if (tmpRoom[j].userId == socket.id) { //leave user를 찾았을ㄸㅐ
                if (serverReceiverPCs[Keys[i]].senderId == socket.id) { //leave user가 버스커인 경우
                    delete rooms[Keys[i]];
                    delete serverReceiverPCs[Keys[i]];

                    for (let k = 0; k < serverSenderPCs.length; k++) {
                        if (serverSenderPCs[k].roomNum == Keys[i]){ //버스킹을 보는 유저에게 알림
                            socket.to(serverSenderPCs[k].id).emit("Error", "leaveUserSender");
                        }
                    }
                    serverSenderPCs = serverSenderPCs.filter(function(data) { //해당 룸을 다 지움
                        return data.roomNum != Keys[i];
                    });
                    break;
                }
                else { //leave user가 버스킹을 보는 유저일 때
                    rooms[Keys[i]] = rooms[Keys[i]].filter(function(data) { //해당 룸을 다 지움
                        return data.userId != socket.id;
                    });
                    serverSenderPCs = serverSenderPCs.filter(function(data) { //해당 룸을 다 지움
                        return data.id != socket.id;
                    });
                    socket.to(socket.id).emit("Error", "leaveUserReceiver");
                    break;
                }
            }
        }
        break;
    }
}