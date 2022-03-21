import mongoose from 'mongoose'

const UserSchema = new mongoose.Schema({
    id: {
        type: String,
        required: true,
    },
    email: {
        type: String,
        required: true,
        unique: true,
    },
    password: {
        type: String,
        required: true,
    },
    nickname: {
        type: String,
        required: true,
    },
    character: {
        type: Number,
        required: true,
    },
    created: {
        type:Date,
        default:Date.now
    }
});

const User = mongoose.model("user", UserSchema);

export default User;