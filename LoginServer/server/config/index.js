const dotenv = require('dotenv');
dotenv.config();

const obj = {};
obj.MONGO_URI = process.env.MONGO_URI;
obj.JWT_SECRET = process.env.JWT_SECRET;
obj.PORT = process.env.PORT;

module.exports = obj;