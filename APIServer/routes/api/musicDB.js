const express = require('express');
const Music = require('../../models/music');
const User = require('../../models/user');
const auth = require('../../middleware/auth');

const router = express.Router();

router.get('/', async(req, res) =>{
    const {title} = req.body;

    try{
        Music.find({title : {$regex: title}}).then((music) => {
            console.log(music)
            res.status(200).json(music);
        });

    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message});
    }
})
router.get('/recent', async(req, res)=> {
    const recent = await Music.find().sort({"created" : -1}).limit(10)
    console.log(recent);
    res.status(200).json({recent: recent});
})

router.post('/', auth, async(req, res) => {
    const {locate ,imageLocate, title, category, lyrics, info} = req.body;

    const userID = req.user.id;
    console.log(userID);

    User.findOne({id: userID}).then((user)=> {
        console.log(user);
        const id = userID + "_" + user.totalNum;
        const userNickname = user.nickname;
        console.log(id)

        const newMusic = new Music({
            locate, imageLocate, title, id, userID, userNickname, lyrics, category, info
        });

        newMusic.save().then(()=> console.log("music save success!!"));

        user.totalNum += 1;
        user.uploadList.push({musicID:id});

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

module.exports = router;