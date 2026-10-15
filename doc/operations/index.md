# Operations

## Environments

| Environment | Purpose                 | URL / Access Notes          |
|-------------|-------------------------|-----------------------------|
| `dev`       | Developer tests         | `https://dev.example.com`   |
| `stage`     | Quality Assurance tests | `https://stage.example.com` |
| `prod`      | Live system             | `https://example.com`       |

## Deployment

- CI: `operations/ci/build.sh <version>` builds the images for `core-api`, `api` and `docs`; `push.sh` pushes them.
- CD: `operations/cd/pull.sh <version>` and `run.sh <version>` pull and start the containers (Docker Compose).

## Monitoring

- Describe monitoring and alerting here.
