const express = require('express');
const bcrypt = require('bcryptjs');
const auth = require('../../middleware/auth');
const jwt = require('jsonwebtoken');
const config = require('../../config/index');
const { JWT_SECRET } = config;

const User = require('../../models/user');

const router = express.Router();

router.get('/check', async(req, res) =>{
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

router.get('/musicListName', auth, async(req,res) => {
    const id = req.user.id;

    User.findOne({id:id}).then(async (user) => {
        res.status(200).json({listName: user.listName})
    })
})

router.get('/musicList', auth, async(req,res) => {
    const id = req.user.id;
    const {listName} = req.body;

    User.findOne({id:id}).then(async (user) => {
        res.status(200).json({musicList: user[listName]})
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
        // console.log(user)
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
        // console.log(user)
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

    // console.log(id)
    // console.log(req.user.character);

    User.findOne({id: id}).then((user)=> {
        // console.log(user);
        user.character = value;
        user.save();
        res.status(200).json({
            character : user.character
        })
    })
})

router.post('/makeList', auth, async (req, res)=>{
    const {listName} = req.body;
    const id = req.user.id;
    let duplicate = false;

    const filter = [
        {$match : {id : id}},
        {
            $addFields: {
                [listName] : []
            }
        }];

    const getListName = [
        {$match : {id : id}},
        {$project: {
                listName: 1
            }
        }];

    let tmp = {};
    tmp[listName] = []

    await User.aggregate(getListName).then((user)=>{
        if (user[0].listName.includes(listName)) {
            duplicate = true;
        }
    })

    if (duplicate) {
        res.status(450).json({msg:"already exist"})
    }
    else {
        await User.updateOne({id: id}, {$push: { listName: listName}});
        await User.updateOne({id: id}, tmp);

        User.aggregate(filter).then((user) => {
            res.status(200).json({listName: user[0].listName})
        })
    }
})

router.post('/deleteList', auth, async (req, res)=> {
    const {listName} = req.body;
    const id = req.user.id;

    if (listName === "uploadList") {
        res.status(440).json({message:"업로드 리스트는 삭제 못함."})
    }
    else {
        await User.updateOne({id: id}, {$pull: { listName: listName}});
        await User.updateOne({id:id}, {$unset:{[listName] : ""}});

        User.findOne({id:id}).then((user) => {
            res.status(200).json({user: user})
        })
    }
})

router.post('/addSong', auth, async (req, res)=> {
    const {listName} = req.body;
    const {musicList} = req.body;
    const id = req.user.id;
    let checkList = [];

    const getList = [
        {$match : {id : id}},
        {$project: {
                [listName]: 1
            }
        }];

    await User.aggregate(getList).then((user)=>{
        checkList = user[0][listName];
    })

    for (let i = 0; i < musicList.length; i++) {
        if (!checkList.includes(musicList[i])){
            await User.updateOne({id: id}, {$push: { [listName]: musicList[i]}});
        }
    }

    User.findOne({id:id}).then((user) => {
        res.status(200).json({user: user})
    })
})

router.post('/deleteSong', auth, async (req, res)=> {
    const {listName} = req.body;
    const {musicId} = req.body;
    const id = req.user.id;

    await User.updateOne({id: id}, {$pull: { [listName]: musicId}});

    User.findOne({id:id}).then((user) => {
        res.status(200).json({user: user})
    })
})

module.exports = router;