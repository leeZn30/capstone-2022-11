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
        type: Array,
        default: []
    },
    startTime : {
        type: Array,
        default: []
    },
    id : {
        type: Number
    }
});

const Zone = mongoose.model("zone", ZoneSchema);

module.exports = Zone;