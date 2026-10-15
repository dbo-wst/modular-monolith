# Wolfgang Steubing AG – Repository Template

Use this repository template to create a foundation for your project.

## Git necessary configs

```
git config --global core.autocrlf false
git config --global core.eol lf
git config --global init.defaultBranch main
```


## VS Code necessary settings

```
"files.eol": "\n",
"files.insertFinalNewline": true,
"editorconfig": true

```
## Repository Layout

- `.github/` - issue templates, pull request template, and configuration.
- `operations/` - build and deployment.
- `doc/` - Documentation area.
- `doc/architecture/` - Architecture specific documentation area.
- `doc/operations/` - Operative  documentation area (e.g. monitoring, build and deployment etc.).
- `src/` - code: `BuildingBlocks/`, `Modules/<Module>/{Domain,Application,Infrastructure,Contracts}`, `Protocol/`, `Host/`.
- `test/` - unit and integration tests.
- `.gitignore` - dotnet and python. adjust when nessecesary
- `.editorconfig` - consistent coding styles
- `.adr-dir` - some repos maintain the architecture decisions in doc-folder. remove if not necessary
