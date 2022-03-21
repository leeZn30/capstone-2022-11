import mongoose from 'mongoose'

const MusicSchema = new mongoose.Schema({
    locate: {
        type: String,
        required: true,
    },
    title: {
        type: String,
        required: true,
    },
    id: {
        type: String,
        required: true,
    },
    userID: {
        type: String,
        required: true,
    },
    created: {
        type:Date,
        default:Date.now,
    },
    category: {
        type: String,
        required: true,
    }
});

const Music = mongoose.model("music", MusicSchema);

export default Music; 