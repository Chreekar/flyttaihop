# Flyttaihop
Sök på Hemnet och Google Maps Directions efter bostadsrätter samt Stockholms bostadsförmedling efter hyresrätter som matchar bådas krav på avstånd och egenskaper när ni flyttar ihop.

###### Installation
1. `npm install`
2. `dotnet restore`
3. `dotnet ef database update`
4. Sätt värdet på 'GoogleApiKey' i user secret store (`dotnet user-secrets set GoogleApiKey YOUR_KEY_HERE`), som en miljövariabel eller i appsettings.json 
5. (Om kör från kommandoraden) `set ASPNETCORE_ENVIRONMENT=Development`
6. (Om kör från kommandoraden) `dotnet run`