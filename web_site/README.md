# Projet AmourConnect - Frontend

Dating site to match with a man VS woman and look for his love❤️

# To start front

*If you have Docker*

```
docker-compose -f .\compose.yaml up -d
```


**Clean the caches if that doesn't work :**

```
docker builder prune --force
```

```
docker image prune --force
```

```
docker exec -it amourconnect_frontend-frontend-amourconnect-1 /bin/sh
```

**Otherwise do this manually if you don't have Docker**

```
npm install -g npm@latest && npm update && npm update --save-dev && npm install && npm run dev
```


# For production :

```
npm run build
```

```
npm start
```