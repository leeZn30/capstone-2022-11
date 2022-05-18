const express = require('express');
const {RtcTokenBuilder, RtcRole} = require('agora-access-token');
const res = require('express/lib/response');
require('dotenv').config();

// const APP_ID = process.env.APP_ID;
// const APP_CERTIFICATE = process.env.APP_CERTIFICATE; // secondary
const PORT = 8082;

const app = express();

const APP_ID = "ed5d27a64ca7451189266ef6703397bf";
const APP_CERTIFICATE = "8482a69696c94ec9ae8739d2616d1eb9";

var channels = {}

// 매번 새로운 토큰을 얻기 위해서 cache없애기
const nocache = (_, resp, next) => {
    resp.header('Cache-Control', 'private, no-cache, no-store, must-revalidate');
    resp.header('Expires', '-1');
    resp.header('Pragma', 'no-cache');
    next();
}

const checkChannel = (req, resp) =>
{
  console.log("----------Now Channels-----------");
  // Dictionary 출력 
  for (var key in channels) { 
    console.log("key : " + key +", value : " + channels[key]); 
  }
  console.log("-----------------------------------");

  let channelName = req.params.channel;
  let role = req.params.role;
  let uid = req.params.uid;

  console.log(channelName + " " + uid);

  if (role === 'publisher')
  {
    if (!(channelName in channels))
    {
      generateRTCToken(req, resp);
    }
    else {
      console.log("Exist channel!");
      return resp.json({ 'token': "not Token" });
    }
  }
  else if (role === 'audience')
  {
    if (channelName in channels)
    {
      generateRTCToken(req, resp);
    }
    else 
    {
      console.log("No channel!");
      return resp.json({ 'token': "not Token" });
    }
  }
}

const deleteChannel = (req, resp) => {
  const channelName = req.params.channel;

  if (channelName in channels)
  {
    delete channels[channelName];

    console.log("Delete channel " + channelName);

    return resp.json({'resp': 'confirm'});
  }
  else 
  {
    return resp.json({'resp': 'noChannel'});
  }
}

const generateRTCToken = (req, resp) => 
{ 
    console.log("generateToken Start!");
    resp.header('Access-Control-Allow-Origin', '*');
    const channelName = req.params.channel;
    if (!channelName) 
    {
        return resp.status(500).json({ 'error': 'channel is required' });
    }

    let uid = req.params.uid;
  if(!uid || uid === '') {
    return resp.status(500).json({ 'error': 'uid is required' });
  }

  // get role
  let role;
  if (req.params.role === 'publisher') {
    role = RtcRole.PUBLISHER;
  } else if (req.params.role === 'audience') {
    role = RtcRole.SUBSCRIBER
  } else {
    return resp.status(500).json({ 'error': 'role is incorrect' });
  }

  // get the expire time
  let expireTime = req.query.expireTime;
  if (!expireTime || expireTime == '') {
    expireTime = 3600;
  } else {
    expireTime = parseInt(expireTime, 10);
  }
  // calculate privilege expire time
  const currentTime = Math.floor(Date.now() / 1000);
  const privilegeExpireTime = currentTime + expireTime;

  const token = RtcTokenBuilder.buildTokenWithUid(
    APP_ID, APP_CERTIFICATE, channelName, uid, role, privilegeExpireTime
    );

    console.log("ChannelName: " + channelName + "\nToken: " + token);

    channels[channelName] = uid;
    return resp.json({ 'token': token });
};

app.get('/rtc/:channel/:role/:tokentype/:uid', nocache , checkChannel);
app.get('/rtc/:channel/delete', deleteChannel);

app.listen(PORT, () => {
    console.log(`Listening on port: ${PORT}`);
  });