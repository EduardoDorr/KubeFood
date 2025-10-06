# 1. Create a Resource Group
resource "azurerm_resource_group" "rg" {
  location = var.resource_group_location
  name     = var.application_name

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 2. Create a Log Analytics
resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.application_name}-logs"
  location            = var.resource_group_location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 7

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 3. Create a Container Apps Environment
resource "azurerm_container_app_environment" "env" {
  name                       = "${var.application_name}-env"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}

# 4. Create a Container App for Jaeger
module "inventory_app" {
  source = "./modules/container_app"

  application_name             = var.application_name
  app_suffix                   = "jaeger"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  container_name               = "jaeger"
  image                        = "jaegertracing/all-in-one:latest"
  cpu                          = "0.5"
  memory                       = "1Gi"
  target_port                  = 16686

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 5. Create a Container App for Seq
module "inventory_app" {
  source = "./modules/container_app"

  application_name             = var.application_name
  app_suffix                   = "seq"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  container_name               = "seq"
  image                        = "datalust/seq:latest"
  cpu                          = "0.5"
  memory                       = "1Gi"

  env_vars = {
    "ACCEPT_EULA" = "Y"
  }

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 6. Create a Container Apps for Catalog.API
module "inventory_app" {
  source = "./modules/container_app"

  application_name             = var.application_name
  app_suffix                   = "catalog-app"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  container_name               = "catalog-api"
  image                        = "${container_registry}/${application_name}-catalog-api:latest"
  cpu                          = "0.5"
  memory                       = "0.5Gi"

  env_vars = {
    "ConnectionStrings__catalogDbConnection" = "mongodb://catalogdb:27017"
    "RabbitMqConfiguration__HostName"        = "rabbitmq"
  }

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 7. Create a Container Apps for Order.API
module "inventory_app" {
  source = "./modules/container_app"

  application_name             = var.application_name
  app_suffix                   = "order-app"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  container_name               = "order-api"
  image                        = "${container_registry}/${application_name}-order-api:latest"
  cpu                          = "0.5"
  memory                       = "0.5Gi"

  env_vars = {
    "ConnectionStrings__orderDbConnection" = "mongodb://orderdb:27017"
    "RabbitMqConfiguration__HostName"      = "rabbitmq"
  }

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 8. Create a Container Apps for Inventory.API
module "inventory_app" {
  source = "./modules/container_app"

  application_name             = var.application_name
  app_suffix                   = "inventory-app"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  container_name               = "inventory-api"
  image                        = "${container_registry}/${application_name}-inventory-api:latest"
  cpu                          = "0.5"
  memory                       = "0.5Gi"

  env_vars = {
    "ConnectionStrings__InventoryDbConnection" = "mongodb://inventorydb:27017"
    "RabbitMqConfiguration__HostName"          = "rabbitmq"
  }

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}