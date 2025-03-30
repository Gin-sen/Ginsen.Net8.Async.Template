# Ginsen.Net8.Async.Milestone

Milestone .Net Core 8 pour la création de package.

## Lancement des implémentations d'exemple du package

Créer un ficher `.env` :

```bash
cp .env.exemple .env
```

Lancer la commande :

`docker compose up -d --build`

## Utiliser le templating de dotnet

Pour installer cette solution en tant que milestone,
lancer la commande dans le dossier racine du projet :

```bash
dotnet new install .
```

Pour créer un projet en utilisant cette milestone :

```bash
cd ..
rm -rf My.Brand.New.Solution
dotnet new ginsen-async-tpl -p:i=true -in=false -n My.Brand.New.Solution
```

Pour désinstaller la milestone (en cas d'amélioration par exemple),
lancez cette commande à la racine du répertoire :

```bash
dotnet new uninstall .
```

## Déploiement Helm

```bash
helm repo add gin-sen https://gin-sen.github.io/Ginsen.Net8.Async.Template
helm repo list
helm repo update
helm show all gin-sen/ginsen-net8-async-milestone
kubectl create ns milestone
kubectl label ns milestone istio-injection=enabled
helm upgrade --install ginsen-net8-async-milestone gin-sen/ginsen-net8-async-milestone -n milestone
```


##  Docker Compose

| Usage  | Lien |
|--------|------|
| BackOffice | http://localhost:8080 |
| Api | http://localhost:8082/swagger |

| Outil | Lien | Usage |
|------|-------|-------|
| Grafana | http://localhost:3000 | Monitoring (Dashboards) |
| Prometheus | http://localhost:9090 | Scraping et stockage de métriques |
| Jaeger | http://localhost:16686 | Traces | 
| RabbitMQ | http://localhost:15672 | Interface de management |
