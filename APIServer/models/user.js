const mongoose = require('mongoose');

const UserSchema = new mongoose.Schema({
    id: {
        type: String,
        required: true
    },
    email: {
        type: String,
        required: true,
        unique: true
    },
    password: {
        type: String,
        required: true
    },
    nickname: {
        type: String,
        required: true
    },
    character: {
        type: Number,
        default: 0
    },
    musicList: {
        type: Array,
        default: []
    },
    myList: {
        type: Array,
        default: []
    },
    preferredGenres: {
        type: Array,
        default: []
    },
    totalNum: {
        type: Number,
        default: 0
    },
    created: {
        type: Date,
        default: Date.now
    }
});

const User = mongoose.model("user", UserSchema);

module.exports = User;