const express = require('express');
const bcrypt = require('bcryptjs');
const auth = require('../../middleware/auth');
const jwt = require('jsonwebtoken');
const config = require('../../config/index');
const { JWT_SECRET } = config;

const Music = require('../../models/music');
const User = require('../../models/user');

const router = express.Router();

router.get('/checkid', async(req, res) =>{
    const {id, email} = req.body;

    if (id){
        try{
            User.findOne({id:id}).then((userExist) => {
                if (userExist)
                    res.status(400).json({idExist: true});
                else
                    res.status(200).json({idExist: false});
            })
        } catch (e) {
            console.log(e);
            res.status(400).json({msg:e.message});
        }
    }
    else if (email){
        try{
            User.findOne({email:email}).then((emailExist)=> {
                if (emailExist)
                    return res.status(400).json({emailExist: true});

                else
                    res.status(200).json({emailExist: false});
            })
        } catch(e) {
            console.log(e);
            res.status(400).json({msg: e.message});
        }
    }
})

router.get('/uploadList', auth, async(req,res) => {
    const id = req.user.id;
    let musicInfo = [];

    User.findOne({id:id}).then(async (user) => {
        for (let i = 0; i < user.uploadList.length; i++){
            await Music.findOne({id: user.uploadList[i].musicID}).then((music) => {
                if (music) {
                    musicInfo.push(music);
                }
                else {
                    User.updateOne({id: id}, {$pull: { uploadList: {musicID: musicId}}});
                }
            })
        }
        res.status(200).json({uploadList: musicInfo})
    })
})

router.get('/myList', auth, async(req,res) => {
    const id = req.user.id;
    let musicInfo = [];

    User.findOne({id:id}).then(async (user) => {
        for (let i = 0; i < user.myList.length; i++){
            await Music.findOne({id: user.myList[i].musicID}).then((music) => {
                if (music) {
                    musicInfo.push(music);
                }
                else{
                    User.updateOne({id: id}, {$pull: { uploadList: {musicID: musicId}}});
                }
            })
        }
        res.status(200).json({myList: musicInfo})
    })
})

router.get('/info', async(req, res)=>{
    const {userId} = req.body;

    const filter = [
        {$match : {id : userId}},
        {$project: {
                id: 1,
                nickname: 1,
                character: 1,
                followNum: {$cond: { if: {$isArray: "$follow"}, then: {$size: "$follow"}, else: 0}},
                followerNum: {$cond: { if: {$isArray: "$follower"}, then: {$size: "$follower"}, else: 0}},
                preferredGenres: 1,
                follow: 1
            }
        }];

    User.aggregate(filter).then((user)=>{
        console.log(user)
        res.status(200).json({user: user[0]})
    })
})

router.get('/search', auth, async(req, res)=>{
    const {userNickname} = req.body;
    const filter = [
        {$match : {nickname : {$regex: userNickname}}},
        {$project: {
                id: 1,
                nickname: 1,
                character: 1,
                preferredGenres: 1,
                followNum: {$cond: { if: {$isArray: "$follow"}, then: {$size: "$follow"}, else: 0}},
                followerNum: {$cond: { if: {$isArray: "$follower"}, then: {$size: "$follower"}, else: 0}},
                "length": {"$strLenCP": "$nickname"}
            }
        },
        {
            $sort: {
                length: 1
            }
        }];

    User.aggregate(filter).then((user)=>{
        console.log(user)
        res.status(200).json({user:user})
    })
})

router.post('/', async(req, res) => {
    const {id, email, password, nickname, character, preferredGenres} = req.body;

    const newUser = new User({
        id, email, password, nickname, character, preferredGenres
    })

    bcrypt.genSalt(10, (err, salt) => {
        bcrypt.hash(newUser.password, salt, (err, hash) => {
            if (err) throw err;
            newUser.password = hash;
            newUser.save().then((user) => {
                jwt.sign(
                    {id: user.id},
                    JWT_SECRET,
                    {expiresIn: 3600},
                    (err, token) => {
                        if (err) throw err;
                        res.json({
                            token,
                            user: {
                                id: user.id,
                                name: user.name,
                                email: user.email,
                                character: user.character,
                                preferredGenres: user.preferredGenres
                            }
                        })
                    }
                )
            })
        })
    })
})

router.post('/modifiedChar', auth, async(req, res) => {
    const id = req.user.id;
    const {value} = req.body;

    console.log(id)
    console.log(req.user.character);

    User.findOne({id: id}).then((user)=> {
        console.log(user);
        user.character = value;
        user.save();
        res.status(200).json({
            character : user.character
        })
    })
})

router.post('/addMyList', auth, async(req, res)=>{
    const {musicList} = req.body;
    const id = req.user.id;
    let musicInfo = [];

    for (let i = 0; i < musicList.length; i++){
        await User.updateOne({id: id}, {$push: { myList: {musicID: musicList[i]}}});
    }

    User.findOne({id:id}).then(async (user) => {
        for (let i = 0; i < user.myList.length; i++){
            await Music.findOne({id: user.myList[i].musicID}).then((music) => {
                if (music) {
                    musicInfo.push(music);
                }
                else {
                    User.updateOne({id: id}, {$pull: { uploadList: {musicID: musicId}}});
                }
            })
        }
        res.status(200).json({myList: musicInfo})
    })
})

router.post('/deleteUploadList', auth, async(req, res)=> {
    const {musicId} = req.body;
    const id = req.user.id;
    let musicInfo = [];

    await User.updateOne({id: id}, {$pull: { uploadList: {musicID: musicId}}});
    await Music.deleteOne({id:musicId});
    User.findOne({id:id}).then(async (user) => {
        for (let i = 0; i < user.uploadList.length; i++){
            await Music.findOne({id: user.uploadList[i].musicID}).then((music) => {
                if (music) {
                    musicInfo.push(music);
                }
                else{
                    User.updateOne({id: id}, {$pull: { uploadList: {musicID: musicId}}});
                }
            })
        }
        res.status(200).json({uploadList: musicInfo})
    })
})

router.post('/deletemyList', auth, async(req, res)=> {
    const {musicId} = req.body;
    const id = req.user.id;
    let musicInfo = [];

    await User.updateOne({id: id}, {$pull: { myList: {musicID: musicId}}});
    User.findOne({id:id}).then(async (user) => {
        for (let i = 0; i < user.myList.length; i++){
            await Music.findOne({id: user.myList[i].musicID}).then((music) => {
                if (music) {
                    musicInfo.push(music);
                }
                else {
                    User.updateOne({id: id}, {$pull: { uploadList: {musicID: musicId}}});
                }
            })
        }
        res.status(200).json({myList: musicInfo})
    })
})

module.exports = router;