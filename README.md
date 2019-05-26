# TraceIp API

API para obtener información asociada a una IP

## Ejecución

La solución está armada utilizando Docker. En la misma se incluyen los archivos Dockerfile y docker-compose.yml. Para instanciarla, ubicarse en el root del proyecto y correr en un terminal el comando:

```
docker-compose -f docker-compose.yml up -d --build
```

Este comando inicia la API, escuchando en el puerto 5000, y una base Redis, utilizada por el servicio.

Para realizar el llamado al servicio principal, que devuelve la información del país asociada a una IP, ejecutar:

`HTTP GET https://localhost:5000/api/traceip?ip=23.17.255.25`

## Servicios API
La API expone 4 servicios definidos en .NET Core 2.2:

### GET api/traceip?ip={ip}
Recibe una dirección IP como parámetro y retorna un reporte con información asociada al país al que pertenece esa IP. Como este servicio recibe solo un parámetro, se lo envía como query string.

Ejemplo de respuesta:
```
    Country: Canada
    ISO Code: CA
    Language: English (en), French (fr), 
    Currency: CAD (1 CAD = 0.7431741181 U$S),
    Time: 09:04 (UTC-08:00), 10:04 (UTC-07:00), 11:04 (UTC-06:00), 12:04 (UTC-05:00), 13:04 (UTC-04:00), 14:34 (UTC-03:30), 
    Distance: 10840
    Hits: 1
```
### GET api/traceip/stats/nearest
Devuelve el reporte del pais mas cercano (en Kms) a la ciudad de Buenos Aires

NOTA: Si Argentina se encuentra en la base de reportes (representa la menor distancia), se la ignora para retornar el reporte de un país mas representativo (distancia > 0).

### GET api/traceip/stats/farest
Devuelve el reporte del pais mas lejano (en Kms) a la ciudad de Buenos Aires

### GET api/traceip/stats/average
Devuelve la distancia promedio de todas las ejecuciones que se hayan hecho al servicio. 
```
Distancia promedio de todas las ejecuciones: 10557
```

## Implementación

### Arquitectura
* Se armó un diseño en capas para manejar el flujo de la aplicación. Un `controller` (capa 1) para manejar la puerta de entrada y presentación de los datos. Un `servicio` (capa 2) para realizar toda la lógica de negocios necesaria. Y un `repositorio` para manejar todos los accesos a la base de datos.
* Se utilizó .NET Core para el desarrollo de la API REST.
* Se utilizó Redis como base de datos key/value, para rápido acceso a los datos requeridos. Esto permite un rápido acceso a los reportes que ya habían sido consultados anteriormente. La base Redis actua como la única base del servicio, manteniendo la persistencia de los datos.
Como las consultas estadísticas que requiere ofrecer la API son posibles de lograr sobre la base Redis, no se utilizó ningun otro motor de persistencia. Pero llegado el caso, la arquitectura permite agregar otro store con un manejo de consultas mas avanzado (MongoDB, SQL Server, etc) para manejar el guardado de reportes, y solo mantener Redis como una base "cache" para retornar los últimos reportes consutaldos o la distancia promedio.

* Se utilizaron APIs públicas para resolver la informacion del IP. Ademas de las recomendadas por el documento (Geolocalizacion e información de países), se utilizó un servicio diferente para la información de monedas: `https://api.exchangeratesapi.io/latest`. El servicio ofrecido por el documento (`https://fixer.io/`) no permitía cambiar la moneda base en el plan "free".

* Se armaron containers Docker para integrar toda la solución. Dentro del proyecto se puede ver el archivo ´docker-compose.yml´, el cual indica los containers que se van a generar para la ejecución:
  ** "webapi": En este container se genera una imagen con el ambiente dotnet:2.2, definido en un archivo Dockerfile. Se instancia el proyecto TraceIpWebApi.csproj (Web API) y se lo inicia, escuchando el puerto 5000.
  ** "redis": Container que inicia una instancia Redis en modo --appendonly yes para manejo de persistencia de datos.

### TBD
* Usar un archivo de recursos para albergar los textos utilizados en el código y permitir manejo de localización.

# Test 
Se incluyo un proyecto de Unit Test en la solución. Utiliza MSTest como librería de Unit Testing. Se pueden correr utilizando el ambiente de desarrollo (Visual Studio), para probar la funcionalidad minima del servicio.

También se dejan a continuación algunos direcciones IPs para realizar pruebas:

```
Afghanistan: 149.54.127.255
Aleria: 41.109.116.255
Antigua y Barbuda: 206.214.15.255
Argentina: 24.232.255.255
Australia: 1.159.255.255
Belgium: 	5.23.255.255
Brazil: 18.229.255.255
Bulgaria: 5.53.255.255
Colombia: 152.205.255.255
Cuba: 152.207.255.255
Croatia: 31.147.255.255
Japan: 1.79.255.255
Iraq: 130.193.255.255
France: 5.51.255.255
Uruguay: 186.55.255.255
Mexico: 132.248.255.255
China: 14.127.255.255
Cameroon: 41.244.255.255
Rusia: 2.95.255.255
Suecia: 2.255.247.255
UK: 3.11.255.255
```


