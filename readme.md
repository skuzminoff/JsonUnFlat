# JsonUnFlat

Json Flatter / Unflatter for C# based on Newtonsoft.Json object model.
Allows you to flat hierarchical json objects and make hierachical json from the flat one.

## Examples

### Flat json

```
{
    "prop1": {
        "prop2": "textprop"
    },
    "prop3": {
        "prop4": "2017-09-08"
    },
    "prop5": [
        "2017-09-05"
    ],
    "prop6": {
        "prop7": 3
    }
}
```
using the

```
var flatter = new Flatter();
var flatJson = flatter.Flat(json);
```

becomes
```
{
    "prop1.prop2":"textprop",
    "prop3.prop4":"2017-09-08",
    "prop5[0]: "2017-09-05",
    "prop6.prop7":3
}
```

### Unflat json

And backwards

```
{
    "prop1.prop2":"textprop",
    "prop3.prop4":"2017-09-08",
    "prop5[0]: "2017-09-05",
    "prop6.prop7":3
}
```

using the

```
var unflatter = new Unflatter();
var json = unflatter.Unflat(flatJson);
```

becomes

```
{
    "prop1": {
        "prop2": "textprop"
    },
    "prop3": {
        "prop4": "2017-09-08"
    },
    "prop5": [
        "2017-09-05"
    ],
    "prop6": {
        "prop7": 3
    }
}
```

## Running the tests
Execute 
```
dotnet test
```
from the root repository directory.

## Built With

* [Newtonsoft.Json](https://www.newtonsoft.com/json)

