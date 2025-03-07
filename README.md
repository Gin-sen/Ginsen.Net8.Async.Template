# Ginsen.Net8.Async.Template

Template .Net Core 8 pour la création de package.

# Lancement des implémentations d'exemple du package

Lancer la commande :

`docker compose up -d`

## Utiliser le templating de dotnet

Pour installer cette solution en tant que template,
lancer la commande dans le dossier racine du projet :

```bash
dotnet new install .
```

Pour créer un projet en utilisant cette template :

```bash
cd ..
rm -rf My.Brand.New.Solution
dotnet new ginsen-async-tpl -p:i=true -in=false -n My.Brand.New.Solution
```

Pour désinstaller la template (en cas d'amélioration par exemple),
lancez cette commande à la racine du répertoire :

```bash
dotnet new uninstall .
```
