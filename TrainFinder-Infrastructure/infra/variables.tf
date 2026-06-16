variable "rg_name" {
  type = string
}

variable "location" {
  type    = string
  default = "norwayeast"
}

variable "environment" {
  type = string
}

variable "tags" {
  type    = map(string)
  default = {}
}

variable "sql" {
  type = object({
    server_name        = string
    db_name            = string
    sku_name           = string
    admin_login        = string
    max_size_gb        = optional(number, 30)
    geo_backup_enabled = optional(bool, true)
  })
}

variable "key_vault_name" {
  type = string
}

variable "storage_account_name" {
  type = string
}

variable "app_service" {
  type = object({
    plan_name = string
    app_name  = string
    sku_name  = string
  })
}

variable "secret_azuread_callback_path" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_azuread_client_id" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_azuread_client_secret" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_azuread_instance" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_azuread_tenant_id" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_connection_string" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_storage_connection_string" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_smtp_username" {
  type      = string
  sensitive = true
  default   = ""
}

variable "secret_smtp_password" {
  type      = string
  sensitive = true
  default   = ""
}
