const express = require('express');
const bcrypt = require('bcryptjs');

const jwt = require('jsonwebtoken');
const config = require('../../config/index');
const { JWT_SECRET } = config;

const User = require('../../models/user');

const router = express.Router();

router.get('/check', async(req, res) =>{
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

module.exports = router;