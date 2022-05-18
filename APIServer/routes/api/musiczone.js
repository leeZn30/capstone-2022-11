const express = require('express');
const auth = require('../../middleware/auth');

const router = express.Router();

global.musicTitle = []
global.musicList = []
global.musicTime = []
global.totalTime = []
global.startTime = []


router.get('/joinZone', auth, async (req, res)=> {
    const {zoneNumber} = req.body;
    let afterTime = Math.floor(new Date()/1000) - startTime[zoneNumber];
    let nowIndex = 0;

    if (afterTime < totalTime[zoneNumber]){
        for (let i = 0; i < musicTime[zoneNumber].length; i++){
            if (afterTime > musicTime[zoneNumber][i]) {
                nowIndex = i+1;
                afterTime -= musicTime[zoneNumber][i];
            }
            else break
        }

        res.status(200).json({locate : musicList[zoneNumber].slice(nowIndex),
            title : musicTitle[zoneNumber].slice(nowIndex),
            time : afterTime});
    }

    else {
        res.status(510).json({msg:"재생되는 노래가 없습니다."})
    }

})



router.post('/createZone', auth, async(req, res)=>{
    const {timeList, locateList, zoneNumber, titleList} = req.body;

    if (totalTime[zoneNumber]) {
        const nowTime = Math.floor(new Date()/1000);
        const startT = startTime[zoneNumber];

        if (totalTime[zoneNumber] < nowTime - startT) {
            musicList[zoneNumber] = locateList;
            musicTime[zoneNumber] = timeList;
            musicTitle[zoneNumber] = titleList;
            startTime[zoneNumber] = Math.floor(new Date()/1000);
            totalTime[zoneNumber] = timeList.reduce((a,b) => Number(a)+Number(b), 0);
            res.status(200).json({msg : "음원존이 생성되었습니다."})
        }
        else {
            res.status(410).json({msg : "현재 진행중인 음원존입니다."});
        }
    }
    else {
        musicList[zoneNumber] = locateList;
        musicTime[zoneNumber] = timeList;
        musicTitle[zoneNumber] = titleList;
        startTime[zoneNumber] = Math.floor(new Date()/1000);
        totalTime[zoneNumber] = timeList.reduce((a,b) => Number(a)+Number(b), 0);
        res.status(200).json({msg : "음원존이 생성되었습니다."})
    }
})


module.exports = router;