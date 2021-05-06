const express = require("express");
const app = express();

const mongoose = require("mongoose");

mongoose.set("useFindAndModify", false);

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

mongoose.connect("mongodb://localhost:27017/sevenlabs");

const db = mongoose.connection;

const Sevenlabs = require("./models");

db.on("error", (err) => console.log(err));
db.once("open", () => {
  console.log("Connected to MongoDB");
});

app.listen(3000, () => {
  console.log("SERVER is on");
});
app.get("/", (req, res) => res.send("OK"));
app.get("/data", (req, res) => {
  res.send("OK")
  console.log(req.query)
});
app.delete("/datas", (req, res) => {
  const { _id, creator, type, weight, color, count } = req.body;
  Sevenlabs.findOne({ _id: _id })
    .then((data) => {
      console.log(data);
      if (data.count - count > 0) {
        data.count = data.count - count;
        data
          .save()
          .then((result) =>
            res
              .status(200)
              .json({ message: "Success", data: updateObject(result.toObject()) })
          )
          .catch((err) => {
            console.log(err);
            res.status(404).json({
              message: " Error " + err,
            });
          });
      } else {
        Sevenlabs.deleteOne({ _id: _id })
          .lean()
          .then((result) =>
            res
              .status(200)
              .json({ message: "Success", data: updateObject(result) })
          )
          .catch((err) => {
            console.log(err);
            res.status(404).json({
              message: " Error " + err,
            });
          });
      }
    })
    .catch((err) => {
      console.log(err);
      res.status(404).json({
        message: "Failed to search with error : " + err,
      });
    });
});
app.put("/datas", async (req, res) => {
  const { _id, creator, type, weight, color, count } = req.body;
  if (_id === undefined) {
    res.status(404).json({
      message: "Id is required",
    });
    return;
  }
  const updated = {};
  if (creator !== undefined) {
    updated.creator = creator.toUpper();
  }
  if (type !== undefined) {
    updated.type = type.toUpper();
    if (updated.type === "PLA+") updated.type = "PLP";
    else if(updated.type === "FLIXABLE") updated.type = "TPU";
  }
  if (weight !== undefined) {
    updated.weight = weight;
  }
  if (color !== undefined) {
    updated.color = color.toUpper();
  }
  if (count !== undefined) {
    updated.count = count;
  }
  await Sevenlabs.findOneAndUpdate(
    { _id: _id },
    {
      $set: updated,
    }
  );
  Sevenlabs.findOne({ _id: _id })
  .lean()
    .then((result) =>
      res.status(200).json({
        message: "Success",
        data: updateObject(result),
      })
    )
    .catch((err) => {
      console.log(err);
      res.status(404).json({
        message: "Some error is detected",
      });
    });
});
// app.post("/test", async (req, res) => {
//   const { _id, creator, type, weight, color, count } = req.body;
//   console.log(finded);
//   res.send(finded || "asdf");
// });
app.post("/datas", async (req, res) => {
  console.log(req.body);
  let { creator, type, weight, color, count } = req.body;
  console.log(creator, type, weight, color, count);
  if (
    creator === undefined ||
    type === undefined ||
    weight === undefined ||
    count === undefined ||
    color === undefined
  ) {
    res.status(404).json({
      message: "some parameters is missing",
    });
  } else {
    if (type === 'PLA+') type = "PLP";
    else if (type === 'FLIXABLE') type = "TPU";
    let finded = await Sevenlabs.findOne({
      creator: creator,
      type: type,
      color: color,
      weight: weight,
    });
    
    let newSevenLabs = new Sevenlabs();
    newSevenLabs.count = 0;
    let labs = finded || newSevenLabs;
    if (finded === null || finded === undefined) {
      labs.creator = creator;
      labs.type = type;
      labs.color = color;
    }
    labs.count += count;
    labs.weight = weight;
    console.log(labs)
    
    labs
      .save()
      .then((result) => {
        res.status(200).json({
          message: " Success",
          data: updateObject(result.toObject()),
        });
      })
      .catch((err) => {
        console.log(err);
        res.status(404).json({
          message: "Some error detected : " + err,
        });
      });
  }
});
app.get("/datas", async (req, res) => {
  const { creator, type, color } = req.query;
  const query = {};

  if (creator !== undefined) {
    query.creator = creator;
  }
  if (type !== undefined) {
    query.type = type;
  }
  if (color !== undefined) {
    query.color = color;
  }
  try {
    const result = await Sevenlabs.find(query).sort().lean();

    res.status(200).json({
      message: "Success",
      data: result.map((a) => updateObject(a)),
    });
  } catch (err) {
    res.status(404).json({
      message: "Failed to search with error : " + err,
    });
  }
});

const updateObject = (a) => {
  //a._id = undefined;
  if (a.type === "PLA+") a.type = "PLP";
  else if (a.type === "FLIXABLE") a.type = "TPU";

  const { creator, type, color } = a;

  a.code = `${String(creator).substring(0, 3)}${String(type).substring(
    0,
    3
  )}${String(color).substring(0, 2)}`;
  return a;
};
