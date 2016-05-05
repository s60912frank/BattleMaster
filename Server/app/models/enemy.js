// load the things we need
var mongoose = require('mongoose');

// define the schema for our enemy model
var enemySchema = mongoose.Schema({
  name: String,
  stamina: Number,
  attack: Number,
  defense: Number,
  evade: Number,
  skillCD: {
    ID: Number,
    CD: Number,
    params: Array
  }
});

// create the model for users and expose it to our app
module.exports = mongoose.model('Enemy', enemySchema);
