# AddressProject
in this project , the general API was bulit. I Generated a basic .NET Core Web API with following features:
- the API uses a simple SQLite database
- database model of address information (street, house number, zip code, city, country)
- a controller with the following endpoints:
  - GET /addresses For retrieving multiple addresses
  - GET /addresses/{id} To retrieve a single address
  - POST /addresses To add a new address to the database.
  - PUT /addresses/{id} To edit an address in the database.
  - DELETE /addresses/{id} To remove an address from the database.
  - GET /addresses/Filters To search and sort the result by address fields
  - GET /addresses/Search To search and sort the result by keyword in all fields
  - GET /addresses/Distance To call geolocation API of each address and calculate distances between two addresses and retrieve the distance in kilometers.
