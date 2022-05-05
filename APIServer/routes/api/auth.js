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
            jwt.sign({id:user.id, nickname: user.nickname}, JWT_SECRET, {expiresIn: "1 hours"}, (err, token)=>{
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

module.exports = router;