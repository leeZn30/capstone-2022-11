const express = require('express');
const Music = require('../../models/music');
const User = require('../../models/user');
const auth = require('../../middleware/auth');

const router = express.Router();

router.get('/', auth, async(req, res)=>{
    const id = req.user.id;

    User.findOne({id:id}).then((user)=>{
        res.status(200).json({follow:user.follow})
    })
})

router.get('/follower', auth, async(req, res)=>{
    const id = req.user.id;

    User.findOne({id:id}).then((user)=>{
        res.status(200).json({follower:user.follower})
    })
})

router.post('/', auth, async(req, res)=>{
    const {userId, userNickname} = req.body;
    const id = req.user.id;
    const nickname = req.user.nickname;
    let duplicate = false;

    await User.findOne({id:id}).then((user)=>{
        for (let i = 0; i < user.follow.length; i++){
            if (user.follow[i][0] === userId) {
                duplicate = true;
                break;
            }
        }
    })

    if (duplicate) {
        res.status(450).json({msg:"already exist"})
    }
    else {
        await User.updateOne({id: id}, {$addToSet: { follow: [userId, userNickname]}});
        await User.updateOne({id: userId}, {$addToSet: { follower: [id, nickname]}});
        User.findOne({id:id}).then((user)=> {
            res.status(200).json({follow:user.follow})
        })
    }
})

router.post('/delete', auth, async(req, res)=>{
    const {userId, userNickname} = req.body;
    const id = req.user.id;
    const nickname = req.user.nickname;

    await User.updateOne({id: id}, {$pull: { follow: [userId, userNickname]}});
    await User.updateOne({id: userId}, {$pull: { follower: [id, nickname]}});
    User.findOne({id:id}).then((user)=> {
        res.status(200).json({follow:user.follow})
    })

})

module.exports = router;