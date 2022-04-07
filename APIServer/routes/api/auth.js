const express = require('express');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const auth = require('../../middleware/auth');
const config = require('../../config/index');

const { JWT_SECRET } = config;

const User = require('../../models/user');

const router = express.Router();


router.post('/', (req, res)=> {
    const {id, password} = req.body;

    User.findOne({id}).then((user)=> {
        if(!user) return res.status(400).json({msg: "유저가 존재하지 않습니다."})

        bcrypt.compare(password, user.password).then((isMatch)=> {
            if (!isMatch) return res.status(400).json({msg: "비밀번호가 일치하지 않습니다."})
            jwt.sign({id:user.id}, JWT_SECRET, {expiresIn: "1 hours"}, (err, token)=>{
                if(err) throw err;
                res.json({
                    token,
                    user: {
                        id: user.id,
                        nickname: user.nickname,
                        email: user.email,
                        character: user.character
                    }
                })
            })
        })
    })
})

// client에서 처리해주는 방식
// router.post('/logout', (req, res)=> {
//     res.json("로그아웃 했습니다.")
// })

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

router.get('/musicList', auth, async(req,res) => {
    const id = req.user.id;

    User.findOne({id:id}).then((user) => {
        res.status(200).json({musicList:user.musicList})
    })
})

router.get('/myList', auth, async(req,res) => {
    const id = req.user.id;

    User.findOne({id:id}).then((user) => {
        res.status(200).json({myList:user.myList})
    })
})

module.exports = router;