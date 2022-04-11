const mongoose = require('mongoose');

const MusicSchema = new mongoose.Schema({
    locate: {
        type: String,
        required: true
    },
    imageLocate: {
        type: String,
        default: ""
    },
    title: {
        type: String,
        required: true
    },
    id: {
        type: String,
        required: true
    },
    userID: {
        type: String,
        required: true
    },
    userNickname: {
        type: String,
        required: true
    },
    category: {
        type: String,
        required: true
    },
    lyrics: {
        type: String,
        default: ""
    },
    info: {
        type: String,
        default: ""
    },
    created: {
        type:Date,
        default:Date.now
    }
});

const Music = mongoose.model("music", MusicSchema);

module.exports = Music;