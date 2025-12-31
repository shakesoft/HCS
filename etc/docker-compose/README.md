## How to deploy on docker
The application provides the related `Dockerfiles` and `docker-compose` file with scripts. You can build the docker images and run them using docker-compose. The necessary database, DbMigrator, and the application will be running on docker with health checks in an isolated docker network.

### Creating the docker images
Navigate to _etc/docker-compose_ folder and run the `build-images-locally.ps1` script. You can examine the script to set **image tag** for your images. It is `latest` by default.

### Running the docker images using docker-compose
Navigate to _etc/docker-compose_ folder and run the appropriate script for your platform:
- **Windows/macOS**: Run `run-docker.ps1` (PowerShell script)
- **Linux**: Run `run-docker.sh` (Bash script)

Both scripts will:
1. Generate developer certificates (if they don't exist already) with `dotnet dev-certs` command to use HTTPS
2. Create the certificate in `./certs/localhost.pfx` folder with password `27a19a3a-9aff-42c5-b8c9-56517f1a626d`
3. Run the docker-compose file in detached mode

The certificate is mounted into containers via volume `./certs:/app/certs`, so containers can access it at `/app/certs/localhost.pfx`.

> **Note**: Developer certificate is only valid for **localhost** domain. If you want to deploy to a real DNS in a production environment, use Let's Encrypt or similar tools to generate production certificates.
