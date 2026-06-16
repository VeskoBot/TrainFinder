variable "rg_name" {
  type = string
}

variable "location" {
  type = string
}

variable "server_name" {
  type = string
}

variable "db_name" {
  type = string
}

variable "sku_name" {
  type    = string
  default = "S0"
}

variable "max_size_gb" {
  type    = number
  default = 30
}

variable "geo_backup_enabled" {
  type    = bool
  default = true
}

variable "collation" {
  type    = string
  default = "SQL_Latin1_General_CP1_CI_AS"
}

variable "admin_login" {
  type    = string
  default = "sqladmin"
}

variable "tags" {
  type    = map(string)
  default = {}
}
