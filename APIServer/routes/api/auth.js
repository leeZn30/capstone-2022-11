const express = require('express');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const auth = require('../../middleware/auth');
const config = require('../../config/index');
const nodemailer = require('nodemailer');

const { JWT_SECRET, EMAIL_PASS } = config;

const User = require('../../models/user');

const router = express.Router();

const transporter = nodemailer.createTransport({
    service: 'gmail',
    auth:{
        user: 'metabusking@gmail.com',
        pass: EMAIL_PASS
    }
});

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

router.post('/email', (req, res)=>{
    const email = req.body.email
    const key = req.body.key

    let mailOptions = {
        from: 'metabusking@gmail.com',
        to: email,
        subject: "메타버스킹 인증번호",
        text: "요청하신 인증번호는 " + key + "입니다."
    }

    transporter.sendMail(mailOptions, function (error, info){
        if (error) {
            console.log(error);
            res.status(500).json({message:error});
        }
        else{
            res.status(200).json({message:"OK!"});
        }
    })
})

// client에서 처리해주는 방식
// router.post('/logout', (req, res)=> {
//     res.json("로그아웃 했습니다.")
// })

module.exports = router;