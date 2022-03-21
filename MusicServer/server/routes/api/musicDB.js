import express from 'express';
import Music from '../../models/music';
import User from '../../models/user';

const router = express.Router();

router.get('/', async(req, res) =>{
    const {title} = req.body;
    console.log(title)
    try{
        Music.find({title : title}).then((music) => {
            console.log(music)
            res.status(200).json(music);
        });

    } catch (e) {
        console.log(e);
        res.status(400).json({msg: e.message});
    }
})

router.post('/', async(req, res) => {
    const {locate, title, userID, category} = req.body;

    User.findOne({id: userID}).then((user)=> {
        const id = userID + user.totalNum;
        console.log(id)

        const newMusic = new Music({
            locate, title, id, userID, category
        });

        newMusic.save().then(()=> console.log("save success!!"));

        user.totalNum += 1;
        user.save();

        res.json({
            music: {
                locate : newMusic.locate,
                title :newMusic.title,
                id :newMusic.id,
                userID :newMusic.userID,
                category :newMusic.category
            }
        });
    })
})

export default router; 