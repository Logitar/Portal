# Portal

Identity provider system.

## Getting Started

You can start the Portal on your computed by following these simple steps:

1. Build the **Frontend** by executing the following commands in the `frontend` directory:

```sh
npm install
npm run build:staging
```

The built files will be into the `backend/src/Logitar.Portal.Web/wwwroot`, allowing the backend to serve the frontend under the `/app` route.

2. Start the **Backend** and its dependencies by executing the following commands in the root directory (same level as this file):

```sh
docker compose build
docker compose up -d
```

And voilà! Your Portal should be available at the `http://localhost:8087` endpoint. You can sign-in with the username `portal` and the password `P@s$W0rD`.

⚠️ It may take a while for your Portal to be available when launching it for the first time, since it needs to apply database migrations and initialize configuration. You may inspect the Docker container logs to get more details. When you see `Now listening on: http://[::]:8080`, you'll know your Portal is now ready!

## Local Development

⚠️ Avoid running multiple instances of the Portal Backend at the same time. The Portal is not yet a distributed application, so messages sent through the MassTransit interfaces will be consumed by each running instance of the Portal Backend that is listening to RabbitMQ. We suggest you stop the `Logitar.Portal_backend` Docker container when running the Portal Backend locally (through your preferred IDE).

### Developer Setup

ℹ️ We used the following stack to develop & test the Portal:

- Microsoft Windows 11 Professional (x64)
- Microsoft Visual Studio 2022 Community Edition
- Microsoft Visual Studio Code
- Microsoft Edge Web Browser
- Node.js 20.10.0
- Docker Desktop v4.28.0

Other stacks may be working fine, but we will not supported them.

### Backend Debug Only

You may debug the Portal Backend by launching it with Visual Studio. Without modifying the application settings, you'll still need the following dependencies in the `docker-compose.yml` file in order to run the Portal Backend:

- Logitar.Portal_mongo
- Logitar.Portal_rabbitmq
- Logitar.Portal_postgres
- Logitar.Portal_mssql

### Frontend Debug Only

You may debug the Portal Frontend by running the development/watch server. You may do so by executing the following commands in the `frontend` directory:

```sh
npm install
npm run dev
```

The frontend should be available at `http://localhost:7787`. It is configured to communicate with the backend located at `http://localhost:8087`, which is the backend configured in the `docker-compose.yml` file. This means you won't be able to debug the Backend through your favorite IDE with this setup.

### Backend & Frontend Debug

Debugging the Backend with the Frontend requires extra steps but is still fairly simple.

1. Start by following the setup steps in the `Backend Debug Only` section.
2. Take note of your backend Base URL, which should look like `http://localhost:5087`, `https://localhost:7005` or `https://localhost:32768`. It could be any port or Base URL, it does not really matter.
3. In the `frontend` directory, create a `.env.development.local` file and set the `VITE_APP_API_BASE_URL` variable. The file contents should look like this: `VITE_APP_API_BASE_URL={YOUR_BASE_URL_FROM_PREVIOUS_STEP}`.
4. Follow the setup steps in the `Frontend Debug Only` section.
