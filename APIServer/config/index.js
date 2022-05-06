const dotenv = require('dotenv');
dotenv.config();

const obj = {};
obj.MONGO_URI = process.env.MONGO_URI;
obj.JWT_SECRET = process.env.JWT_SECRET;
obj.PORT = process.env.PORT;
obj.BUCKET_NAME = process.env.BUCKET_NAME;
obj.AWS_BUCKET_URL = process.env.AWS_BUCKET_URL;
obj.AWS_BUCKET_ACCESS_ID = process.env.AWS_BUCKET_ACCESS_ID;
obj.AWS_BUCKET_ACCESS_KEY = process.env.AWS_BUCKET_ACCESS_KEY;
obj.EMAIL_PASS = process.env.EMAIL_PASS;

module.exports = obj;