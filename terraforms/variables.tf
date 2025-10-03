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
  default     = "19ab8758-2a6e-4ddf-bac2-e9d11a752175"
  description = "App Registration's Object ID."
}