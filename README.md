# A Real-time Messaging Web Application

## Technical

### Start the app
* Currently, it's a bit tough :D
  * Start the database: ```cd backend && docker compose up -d```
  * Start the backend: from root run ```dotnet watch --project ChatApp.Presentation```
  * Start the frontend: from root run ```cd frontend && npm i && npm run dev:https```
  ```
  docker compose up
  ```

### Run tests
* To run the tests, run:

```
dotnet watch --project ChatApp.Presentation
```
* Note: while running the tests, you should have Docker running, and also, for AWS secrets manager integration test, you will have to create a secret on AWS.
### Setup AWS for tests
* Run ```aws configure``` and login to your AWS account
* For the AWS Secret Manager test, you need to
    * Create a secret in the eu-west-1 with id: chatapp-secrets
    * Set secret plain text to:
      ```
        {
          "KEY1": "Key 1",
          "KEY2": "Key 2",
        }
      ```
### ChatApp Features
* Register and log in using email and password or OpenID(currently only Google provider).
* Add users to friends list.
* Create chat rooms.
* Send texts to friends or chat rooms and receive them in real-time.

### Tech-stack
* Frontend: Next.js (it also supports backend, but used it for SSR and lower JS bundle sizes).
* Backend: ASP.NET Core, .NET 9.
* Database: MS SQL.

### Design
* Developed using partly clean architecture(.NET IdentityUser exists in Domain as I was too lazy to abstract it:D).
* Used a code-first approach to database design using EF Core.
* Used Identity API to set up authentication and authorization on the backend.
* Implemented OIDC auth by sending HTTP requests to OIDC provider servers.
* Implemented real-time messaging using WebSockets and created a background task queue on the backend to process sending WebSocket messages without blocking the main thread. 
* Testing was set up using xUnit, Moq, and TestContainers(for testing database operations).

### Cloud infrastructure
* Hosted the application on AWS and deployed using IaC tool Terraform.
* Set up a remote backend on AWS S3 for storing Terraform state.
* Set up a private subnet for ECS containers and exposed specific ports via Application load balancer to access the frontend or backend and enforce HTTPS.
* Set up a CI/CD pipeline to automatically deploy to ECR and ECS.
