# Prometheus

This web application has two goals:
1. Enable the collation and analysis of martial arts techniques, forms, practitioners, styles, and schools.
2. Enable me to have fun learning new techniques and technologies--EG. functional programming with F#--and brushing up on techniques I haven't used in a while

## Technologies

### Frontend
Angular with NgRx. However, I haven't started building the frontend yet, and I go back and forth on what tech to use here, so this is subject to change.

### Backend
ASP.NET Core Web API microservices written in F# and using Giraffe, using SQL Server for relational needs and neo4j for graph adventures on the database end.

### Infrastructure
Microservices containerised with Docker and orchestrated with Kubernetes. RabbitMQ is also in there.
