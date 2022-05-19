const mongoose = require('mongoose');

const ZoneSchema = new mongoose.Schema({
    musicTitle : {
        type: Array,
        default: []
    },
    musicList : {
        type: Array,
        default: []
    },
    musicTime : {
        type: Array,
        default: []
    },
    totalTime : {
        type: Number
    },
    startTime : {
        type: Number
    },
    id : {
        type: String
    }
});

const Zone = mongoose.model("zone", ZoneSchema);

module.exports = Zone;