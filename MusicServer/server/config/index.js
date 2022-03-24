const dotenv = require('dotenv');
dotenv.config();

const obj = {};
obj.MONGO_URI = process.env.MONGO_URI;
obj.PORT = process.env.PORT;

module.exports = obj;