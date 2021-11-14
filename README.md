# DarkSoulsQueryServer
 
ASP.NET server that provides an API for clients to query the world state of the current running Dark Souls III process. Depends on [DarkSoulsMemoryReader](https://github.com/srogee/DarkSoulsMemoryReader), so clone that first! Currently used by [DarkSoulsQueryClient](https://github.com/srogee/DarkSoulsQueryClient).

## Supported Endpoints
* `/ReadArbitraryValues`
  * This POST endpoint takes in a JSON array of strings representing which values you want to read from the Dark Souls III process. A full list of value IDs can be found [here](https://github.com/srogee/DarkSoulsMemoryReader/blob/main/DarkSoulsMemoryReader/Games/DarkSouls3.cs) - each dictionary key is a value ID. Wildcards are supported - if a string ends in * then any values with IDs that start with that string will be included in the output.
  * The response will be a JSON array of objects. Each object will have the properties `id` and `value`.
