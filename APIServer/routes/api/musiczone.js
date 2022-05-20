const express = require('express');
const auth = require('../../middleware/auth');

const Zone = require('../../models/zone');

const router = express.Router();

router.get('/joinZone', auth, async (req, res)=> {
    const {zoneNumber} = req.body;
    let totalTime = 0;
    let startTime = 0;

    Zone.findOne({id:zoneNumber}).then(async (zone)=>{
        console.log(zone);
        if (!zone) {
            res.status(510).json({msg:"재생되는 노래가 없습니다."})
        }
        else {
            totalTime = zone.totalTime;
            startTime = zone.startTime;

            let afterTime = Math.floor(new Date()/1000) - startTime;
            let nowIndex = 0;

            if (afterTime < totalTime){
                musicTitle = zone.musicTitle;
                musicList = zone.musicList;
                musicTime = zone.musicTime;

                for (let i = 0; i < musicTime.length; i++){
                    if (afterTime > musicTime[i]) {
                        nowIndex = i+1;
                        afterTime -= musicTime[i];
                    }
                    else break;
                }

                res.status(200).json({locate : musicList.slice(nowIndex),
                    title : musicTitle.slice(nowIndex),
                    time : afterTime});
            }
            else {
                await Zone.deleteOne({id: zoneNumber});
                res.status(510).json({msg:"재생되는 노래가 없습니다."});
            }
        }
    })
})

router.post('/createZone', auth, async(req, res)=>{
    const {timeList, locateList, zoneNumber, titleList} = req.body;

    Zone.findOne({id:zoneNumber}).then(async (zone)=> {
        if (!zone) {
            const startT = Math.floor(new Date()/1000);

            const totalTime = timeList.reduce((a,b) => Number(a)+Number(b), 0);

            const newZone = new Zone({
                musicTitle: titleList,
                musicList: locateList,
                musicTime: timeList,
                totalTime: totalTime,
                startTime: startT,
                id: zoneNumber
            })
            newZone.save();
            res.status(200).json({msg : "음원존이 생성되었습니다."});
        }
        else {
            const nowTime = Math.floor(new Date()/1000);
            const startT = zone.startTime;

            if (zone.totalTime < nowTime - startT) {
                await Zone.deleteOne({id: zoneNumber});

                const totalTime = timeList.reduce((a,b) => Number(a)+Number(b), 0);

                const newZone = new Zone({
                    musicTitle: titleList,
                    musicList: locateList,
                    musicTime: timeList,
                    totalTime: totalTime,
                    startTime: nowTime,
                    id: zoneNumber
                })
                newZone.save();
                res.status(200).json({msg : "음원존이 생성되었습니다."});
            }
            else {
                res.status(410).json({msg : "현재 진행중인 음원존입니다."});
            }
        }
    })
})

module.exports = router;