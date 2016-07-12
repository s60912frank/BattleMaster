// load the things we need
var mongoose = require('mongoose');

// define the schema for our enemy model
var mapSchema = mongoose.Schema({
  xTile: Number,
  yTile: Number,
  zoom: Number,
  enemies:[{ name: String }]
});

// create the model for users and expose it to our app
module.exports = mongoose.model('Map', mapSchema);
