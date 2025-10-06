variable "resource_group_location" {
  type        = string
  default     = "northcentralus"
  description = "Location of the resource group."
}

variable "application_name" {
  type        = string
  default     = "kubefood"
  description = "Resource group name in your Azure subscription."
}

variable "environment" {
  type        = string
  default     = "development"
  description = "Environment's name."
}

variable "app_registration_object_id" {
  type        = string
  description = "App Registration's Object ID."
}

variable "container_registry" {
  type        = string
  description = "Container Registry's name."
}

variable "catalogdb_connection_string" {
  type        = string
  description = "CatalogApi's connection string."
}

variable "orderdb_connection_string" {
  type        = string
  description = "OrderApi's connection string."
}

variable "inventorydb_connection_string" {
  type        = string
  description = "InventoryApi's connection string."
}