const router = require('express').Router()
const multiparty = require('multiparty')
const url = require('url')
const config = require('../../config/index');
const AWS = require('aws-sdk');

const {AWS_BUCKET_URL} = config
const {AWS_BUCKET_ACCESS_ID} = config
const {AWS_BUCKET_ACCESS_KEY} = config


const BUCKET_NAME = 'bucket-bsn0zi'

AWS.config.update({
    region:         'ap-northeast-2',
  accessKeyId:    AWS_BUCKET_ACCESS_ID,
  secretAccessKey: AWS_BUCKET_ACCESS_KEY
})
router.get('/*', (req,res)=>{
    const {pathname} = url.parse(req.url, true)
    res.redirect(`https://${AWS_BUCKET_URL}${pathname}`)
})

router.post('/',function(req, res){
    const form = new multiparty.Form()
    // 에러 처리
    form.on('error', function(err){
        res.status(500).end()
    })
    // form 데이터 처리
    form.on('part', function(part){
        //파일 아니면 res.error
        if(!part.filename)
            return res.status(400).json({msg: "올바르지 않은 파일입니다."});
        //filename -> id+totalNum db에서 가져와서
        const filename = part.filename // 버킷에 올라갈 디렉토리+파일의 이름
        const params = { Bucket:BUCKET_NAME, Key:filename, Body:part, ContentType: 'audio/mpeg' }
        const upload = new AWS.S3.ManagedUpload({ params });
        upload.promise()

        part.on('end', function(){
            // 파일 업로드 후 실행할 추가 코드
        })
        part.on('error', function(err){
            console.log(err)
        })
    })
    // form 종료
    form.on('close', function(){
        // 모든 파일 업로드 후 실행할 추가 코드
        res.end()
    })
    form.parse(req)
})

module.exports = router