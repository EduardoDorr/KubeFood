variable "resource_group_name" {
  description = "Nome do resource group"
  type        = string
}

variable "application_name" {
  description = "Nome da aplicação base"
  type        = string
}

variable "app_suffix" {
  description = "Sufixo do nome do app"
  type        = string
}

variable "container_app_environment_id" {
  description = "ID do ambiente de container"
  type        = string
}

variable "tags" {
  description = "Tags a serem aplicadas"
  type        = map(string)
  default     = {}
}

variable "container_name" {
  description = "Nome do container"
  type        = string
}

variable "image" {
  description = "Imagem do container"
  type        = string
}

variable "cpu" {
  description = "CPU para o container"
  type        = string
}

variable "memory" {
  description = "Memória para o container"
  type        = string
}

variable "env_vars" {
  description = "Variáveis de ambiente para o container"
  type        = map(string)
  default     = {}
}

variable "min_replicas" {
  type    = number
  default = 1
}

variable "max_replicas" {
  type    = number
  default = 2
}

variable "external_enabled" {
  type    = bool
  default = true
}

variable "internal_enabled" {
  type    = bool
  default = true
}

variable "target_port" {
  type    = number
  default = 80
}