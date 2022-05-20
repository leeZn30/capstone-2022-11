const mongoose = require('mongoose');

const ZoneSchema = new mongoose.Schema({
    musicTitle : {
        type: Array,
        required: true
    },
    musicList : {
        type: Array,
        required: true
    },
    musicTime : {
        type: Array,
        required: true
    },
    totalTime : {
        type: Number,
        required: true
    },
    startTime : {
        type: Number,
        required: true
    },
    id : {
        type: String,
        required: true,
        unique: true
    }
});

const Zone = mongoose.model("zone", ZoneSchema);

module.exports = Zone;