resource "azurerm_container_app" "this" {
  name                         = "${var.application_name}-${var.app_suffix}"
  container_app_environment_id = var.container_app_environment_id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"

  tags = var.tags

  template {
    container {
      name   = var.container_name
      image  = var.image
      cpu    = var.cpu
      memory = var.memory

      dynamic "env" {
        for_each = var.env_vars
        content {
          name  = env.key
          value = env.value
        }
      }
    }

    max_replicas = var.max_replicas
    min_replicas = var.min_replicas
  }

  ingress {
    external_enabled = var.external_enabled
    target_port      = var.target_port
    transport        = "auto"

    traffic_weight {
      percentage = 100
    }
  }
}
