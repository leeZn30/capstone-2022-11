import express from 'express';
import bcrypt from 'bcryptjs';
import jwt from 'jsonwebtoken';
import auth from '../../middleware/auth';
import config from '../../config/index';
const { JWT_SECRET } = config;

import User from '../../models/user';

const router = express.Router();

router.post('/', (req, res)=> {
    const {id, password} = req.body;

    if (!id || !password){
        return res.status(400).json({msg: "모든 필드를 채워주세요."})
    }

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

router.post('/logout', (req, res)=> {
    res.json("로그아웃 했습니다.")
})

router.get('/user', auth, async(req, res)=>{
    try{
        let id = req.user.id
        const user = await User.findOne({id}).select("-password");
        if (!user) throw Error("유저가 존재하지 않습니다.");
        res.json(user);
    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message})
    }
})

export default router;