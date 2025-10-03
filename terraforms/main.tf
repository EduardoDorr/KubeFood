# 1. Create a Resource Group
resource "azurerm_resource_group" "rg" {
  location = var.resource_group_location
  name     = var.application_name

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

# 2. Create Container Registry (ACR)
resource "azurerm_container_registry" "acr" {
  name                = "${var.application_name}acr"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Standard"
  admin_enabled       = false

  tags = {
    environment = var.environment
    project     = var.application_name
  }
}

resource "azurerm_role_assignment" "acr_push_permission" {
  principal_id         = var.app_registration_object_id
  role_definition_name = "AcrPush"
  scope                = azurerm_container_registry.acr.id
}

# 3. Create a Log Analytics
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

# 4. Create a Container Apps Environment
resource "azurerm_container_app_environment" "env" {
  name                       = "${var.application_name}-env"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}

# 5. Create a Container Apps for Catalog.API
resource "azurerm_container_app" "catalog" {
  name                         = "${var.application_name}-catalog-app"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  tags = {
    environment = var.environment
    project     = var.application_name
  }

  template {
    container {
      name   = "catalog-api"
      image  = "${azurerm_container_registry.acr.login_server}/catalog-api:latest"
      cpu    = "0.5"
      memory = "0.5Gi"

      env {
        name  = "ConnectionStrings__CatalogDbConnection"
        value = "mongodb://catalogdb:27017"
      }

      env {
        name  = "RabbitMqConfiguration__HostName"
        value = "rabbitmq"
      }
    }

    max_replicas = 2
    min_replicas = 1
  }

  ingress {
    external_enabled = true
    target_port      = 80
    traffic_weight {
      percentage = 100
    }
  }
}

# 6. Create a Container Apps for Order.API
resource "azurerm_container_app" "order" {
  name                         = "${var.application_name}-order-app"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  tags = {
    environment = var.environment
    project     = var.application_name
  }

  template {
    container {
      name   = "order-api"
      image  = "${azurerm_container_registry.acr.login_server}/order-api:latest"
      cpu    = "0.5"
      memory = "0.5Gi"

      env {
        name  = "ConnectionStrings__OrderDbConnection"
        value = "mongodb://Orderdb:27017"
      }

      env {
        name  = "RabbitMqConfiguration__HostName"
        value = "rabbitmq"
      }
    }

    max_replicas = 2
    min_replicas = 1
  }

  ingress {
    external_enabled = true
    target_port      = 80
    traffic_weight {
      percentage = 100
    }
  }
}

# 5. Create a Container Apps for Inventory.API
resource "azurerm_container_app" "inventory" {
  name                         = "${var.application_name}-inventory-app"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  tags = {
    environment = var.environment
    project     = var.application_name
  }

  template {
    container {
      name   = "inventory-api"
      image  = "${azurerm_container_registry.acr.login_server}/inventory-api:latest"
      cpu    = "0.5"
      memory = "0.5Gi"

      env {
        name  = "ConnectionStrings__InventoryDbConnection"
        value = "mongodb://inventorydb:27017"
      }

      env {
        name  = "RabbitMqConfiguration__HostName"
        value = "rabbitmq"
      }
    }

    max_replicas = 2
    min_replicas = 1
  }

  ingress {
    external_enabled = true
    target_port      = 80
    traffic_weight {
      percentage = 100
    }
  }
}