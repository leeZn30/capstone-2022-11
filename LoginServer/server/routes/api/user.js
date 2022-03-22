import express from 'express';
import bcrypt from 'bcryptjs';

import jwt from 'jsonwebtoken';
import config from '../../config/index';
const { JWT_SECRET } = config;

import User from '../../models/user';

const router = express.Router();

// @routes Get all user
router.get('/', async(req, res) =>{
    try{
        const users = await User.find();
        if (!users) throw Error("No users");
        res.status(200).json(users);

    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message});
    }
})

router.get('/checked', async(req, res) =>{
    const {id, email, nickname} = req.body;

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
    else if (nickname){
        try{
            User.findOne({nickname:nickname}).then((nicknameExist)=> {
                if (nicknameExist)
                    res.status(400).json({nicknameExist: true});

                else
                    res.status(200).json({nicknameExist: false});
            })
        } catch (e) {
            console.log(e);
            res.status(400).json({msg: e.message});
        }
    }
})

router.post('/', async(req, res) => {
    const {id, email, password, nickname, character} = req.body;

    const newUser = new User({
        id, email, password, nickname, character
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
                            }
                        })
                    }
                )
            })
        })
    })
})

export default router;