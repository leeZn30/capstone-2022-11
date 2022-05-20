const express = require('express');
const Music = require('../../models/music');
const User = require('../../models/user');
const auth = require('../../middleware/auth');

const router = express.Router();

router.get('/title', async(req, res) =>{
    const {title} = req.body;
    const filter = [
        {$match : {title : {$regex: title}}},
        {$project: {
                "locate": 1,
                "imageLocate": 1,
                "title": 1,
                "id" : 1,
                "userID" : 1,
                "userNickname" : 1,
                "category" : 1,
                "lyrics" : 1,
                "info" : 1,
                "created" : 1,
                "length": {"$strLenCP": "$title"},
                "playedNum": 1
            }
        },
        {
            $sort: {
                length: 1
            }
        }];

    try{
        Music.aggregate(filter).then((music) => {
            // console.log(music.length)
            res.status(200).json(music);
        });

    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message});
    }
})

router.get('/artist', async(req, res) =>{
    const {artist} = req.body;
    const filter = [
                    {$match : {userNickname : {$regex: artist}}},
                    {$project: {
                        "locate": 1,
                        "imageLocate": 1,
                        "title": 1,
                        "id" : 1,
                        "userID" : 1,
                        "userNickname" : 1,
                        "category" : 1,
                        "lyrics" : 1,
                        "info" : 1,
                        "created" : 1,
                        "length": {"$strLenCP": "$userNickname"}
                    }
                    },
                    {
                        $sort: {
                            length: 1
                        }
                    }];

    try{
        Music.aggregate(filter).then((music) => {
            // console.log(music)
            res.status(200).json(music);
        })

    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message});
    }
})

router.get('/category', async(req, res) =>{
    const {category} = req.body;

    const filter = [
        {$match : {category : category}},
        {
            $sort: {
                playedNum: -1
            }
        }];

    try{
        Music.aggregate(filter).then((music) => {
            res.status(200).json(music);
        })

    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message});
    }
})

router.get('/uploadList', async(req,res) => {
    const {userId} = req.body;
    let musicInfo = [];

    User.findOne({id:userId}).then(async (user) => {
        for (let i = 0; i < user.uploadList.length; i++){
            await Music.findOne({id: user.uploadList[i]}).then((music) => {
                if (!music) {
                    User.updateOne({id: userId}, {$pull: { uploadList: musicId}});
                }
                else {
                    musicInfo.push(music);
                }
            })
        }
        res.status(200).json({uploadList: musicInfo});
    })
})

router.get('/recent', async(req, res)=> {
    const recent = await Music.find().sort({"created" : -1}).limit(10);
    // console.log(recent);
    res.status(200).json({recent: recent});
})

router.get('/popular', async(req, res)=>{
    const popular = await Music.find().sort({playedNum : -1}).limit(20);
    res.status(200).json({popular: popular});
})

router.get('/personalGenre', auth, async(req, res)=>{
    const userId = req.user.id;

    User.findOne({id:userId}).then((user)=>{
        // console.log(user.preferredGenres);
        Music.find({category:{$in:user.preferredGenres}}).sort({playedNum: -1}).limit(20).then((music)=>{
            res.status(200).json({personalGenre:music})
        })
    })
})

router.post('/', auth, async(req, res) => {
    const {locate ,imageLocate, title, category, lyrics, info, time} = req.body;

    const userID = req.user.id;

    // console.log(userID);

    User.findOne({id: userID}).then((user)=> {
        // console.log(user);
        const id = userID + "_" + user.totalNum;
        const userNickname = user.nickname;
        // console.log(id);

        const newMusic = new Music({
            locate, imageLocate, title, id, userID, userNickname, lyrics, category, info, time
        });

        newMusic.save().then(()=> console.log("music save success!!"));

        user.totalNum += 1;
        user.uploadList.push(id);

        user.save();

        res.status(200).json({
            music: {
                locate : newMusic.locate,
                imageLocate: newMusic.imageLocate,
                title :newMusic.title,
                id :newMusic.id,
                userID :newMusic.userID,
                userNickName : newMusic.userNickname,
                category :newMusic.category,
                lyrics : newMusic.lyrics,
                info : newMusic.info
            },
            user: {
                totalNum: user.totalNum,
                musicList: user.musicList
            }
        });
    })
})

router.post('/play', async(req, res) => {
    const {musicId} = req.body;

    await Music.updateOne({id: musicId}, {$inc: { playedNum: 1}})
    Music.find({id:musicId}).then((music)=>{
        console.log(music)
    })
    res.status(200).json({"message":"OK"})
})

module.exports = router;