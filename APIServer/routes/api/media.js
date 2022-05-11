const router = require('express').Router()
const multiparty = require('multiparty')
const url = require('url')
const config = require('../../config/index');
const AWS = require('aws-sdk');
const path = require("path");
const auth = require('../../middleware/auth');
const User = require('../../models/user');
const bodyParser = require('body-parser');
const express = require('express');
const app = express();
//body-Parser
app.use(bodyParser.urlencoded({extended:true}));
app.use(bodyParser.json());

//액세스 키 담긴 파일 로드
const dirPath = path.join(__dirname, '/aws.config.json')
AWS.config.loadFromPath(dirPath);
//config update 후 s3 생성
const s3 = new AWS.S3()

const { BUCKET_NAME } = config
const { AWS_BUCKET_URL } = config


router.get('/*', (req,res)=>{
    const {pathname} = url.parse(req.url, true)
    res.redirect(`https://${AWS_BUCKET_URL}${pathname}`)
})

//Upload
router.post('/', auth, function(req, res){
    const userid = req.user.id;
    let totalNum = 0;
    let musicLocate = "";
    let imageLocate = "";

    const form = new multiparty.Form()
    // 에러 처리
    form.on('error', function(err){
        res.status(500).end()
    })
    // form 데이터 처리
    form.on('part', function(part){
        User.findOne({id: userid}).then((user)=> {
            totalNum = user.totalNum;
            //파일 아니면 res.error
            if(!part.filename)
                return res.status(400).json({msg: "올바르지 않은 파일입니다."});
            //filename -> id+_+totalNum
            const filename = userid+"_"+totalNum
            
            //음악, 이미지 파일 다른 폴더로 업로드
            const extension = path.extname(part.filename);
            var params = {}
            //파일 확장자 확인
            if(extension === '.mp3' || extension === '.wav') {
                const musicKey = 'Music/' + filename + extension;
                params = {Bucket: BUCKET_NAME, Key: musicKey, Body: part, ContentType: 'audio/mpeg'};
                musicLocate = AWS_BUCKET_URL + "/" + musicKey;
            }
            else if(extension === '.jpg' || extension === '.png') {

                const imageKey = 'Image/' + filename + extension;

                params = {Bucket: BUCKET_NAME, Key: imageKey, Body: part, ContentType: 'image'};
                imageLocate = AWS_BUCKET_URL + "/" + imageKey;
            }
            else
                return res.status(400).json({msg: "올바르지 않은 파일입니다."});
            const upload = new AWS.S3.ManagedUpload({ params });
            upload.promise()

            part.on('end', function(){
                // 파일 업로드 후 실행할 추가 코드
                console.log("Uploaded!")
            })
            part.on('error', function(err){
                console.log(err)
            })

        });
    })
    // form 종료
    form.on('close', function(){
        // 모든 파일 업로드 후 실행할 추가 코드
        res.status(200).json({locate: musicLocate, imageLocate: imageLocate});
    })
    form.parse(req)

})

//Delete
router.post('/delete',(req,res) =>{
    //삭제할 file
    var delete_filename = req.body.locate;
    //파일 확장자
    delete_filename=path.basename(delete_filename);
    const extension = path.extname(delete_filename);
    var params = {}
    //파일 확장자 확인
    if(extension === '.mp3' || extension === '.wav') {
        params = {Bucket: BUCKET_NAME, Key: "Music/" + delete_filename};
    }
    else if(extension === '.jpg' || extension === '.png') {
        params = {Bucket: BUCKET_NAME, Key: "Image/" + delete_filename};
    }
    else
        return res.status(400).json({msg: "올바르지 않은 파일입니다."});
    //파일 삭제
    s3.deleteObject(params, function(err, data) {
    if (err) console.log(err, err.stack); 
    else     console.log("Delete Object Success!");
    });
    res.status(200).json({msg: "파일이 성공적으로 삭제되었습니다."})
})


module.exports = router;