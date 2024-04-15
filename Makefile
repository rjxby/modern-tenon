IMAGE_NAME := modern-tenon-api-dev
DOCKER_RUN_OPTIONS := -p 8080:8080

add-migration:
	dotnet ef migrations add $(name) --startup-project ./Backend/Api/Host --project ./Backend/Api/Repositories/Implimentation

remove-migration:
	dotnet ef migrations remove --startup-project ./Backend/Api/Host --project ./Backend/Api/Repositories/Implimentation

apply-migrations:
	dotnet ef database update --startup-project ./Backend/Api/Host --project ./Backend/Api/Repositories/Implimentation

dev-run:
	docker build -t $(IMAGE_NAME) -f Backend/Api/Host/Dockerfile ./Backend && docker run $(DOCKER_RUN_OPTIONS) $(IMAGE_NAME)