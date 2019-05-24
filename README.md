# Run docker instance (Web API)
docker run --env ASPNETCORE_ENVIRONMENT = Development--env ASPNETCORE_URLS = http://+:5000 -p 5000:5000 -t --rm -it  traceipwebapi

# Docker compose
docker-compose -f docker-compose.yml up -d --build

# Test IPs
```
Afghanistan: 149.54.127.255
Aleria: 41.109.116.255
Antigua y Barbuda: 206.214.15.255
Argentina: 24.232.255.255
Australia: 1.159.255.255
Belgium: 	5.23.255.255
Brazil: 18.229.255.255, 2824kms, 2 hits
Bulgaria: 5.53.255.255
Colombia: 152.205.255.255
Cuba: 152.207.255.255
Croatia: 31.147.255.255
Japan: 1.79.255.255, 18018km
Iraq: 130.193.255.255
France: 5.51.255.255
Uruguay: 186.55.255.255, 750kms
Mexico: 132.248.255.255
China: 14.127.255.255, 19017kms, 2 hits
Cameroon: 41.244.255.255
Rusia: 2.95.255.255
Suecia: 2.255.247.255
UK: 3.11.255.255, 11454kms
