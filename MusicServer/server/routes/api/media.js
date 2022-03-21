import Music from '../../models/music';

const router = require('express').Router()
const multiparty = require('multiparty')
const url = require('url')
const fs = require('fs')


router.post('/upload', (req,res)=>{
    const form = new multiparty.Form()
    form.on('error', err => res.status(500).end())
    form.on('part', part => {
        // file이 아닌 경우 skip
        if(!part.filename)
            return res.status(400).json({msg: "올바르지 않은 파일입니다."});

        const filestream = fs.createWriteStream(`./server/resource/${part.filename}`)
        part.pipe(filestream)
    })
    form.on('close', ()=>res.end())
    form.parse(req)
})


router.get('/*',(req,res)=>{
    const {pathname} = url.parse(req.url, true)
    console.log(pathname)
    const realpath = decodeURI(pathname)
    console.log(realpath)
    const filepath = `./server/resource${realpath}`
    console.log(filepath)

    const stat = fs.statSync(filepath)
    const fileSize = stat.size
    const header = {
        'Accept-Ranges': 'bytes',
        'Content-Type'  : 'audio/mpeg',
        'Content-Length': fileSize,
    }
    res.writeHead(200, header);

    const readStream = fs.createReadStream(filepath);
    readStream.pipe(res);
})

export default router;