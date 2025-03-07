# Ginsen.Net8.Async.Milestone

Milestone .Net Core 8 pour la création de package.

# Lancement des implémentations d'exemple du package

Lancer la commande :

`docker compose up -d`

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
