const mongoose = require('mongoose')
const Schema = mongoose.Schema;

var SevenLabs = new Schema({
    idx: Number,
    creator: String,
    type: String,
    weight: Number,
    color: String,
    count: Number,
});

SevenLabs.statics.findOneById = function (id) {
    return this.findOne({ idx: id }).exec();
}
module.exports = mongoose.model("SevelLabs", SevenLabs);